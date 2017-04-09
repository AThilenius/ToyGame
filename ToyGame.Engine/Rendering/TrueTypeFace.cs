using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using OpenTK;
using OpenTK.Graphics;
using SharpFont;
using ToyGame.Rendering.OpenGL;
using ToyGame.Utilities;

namespace ToyGame.Rendering
{
  /// <summary>
  ///   An instance represents a single Face, also manages the texture atlas for all loaded font faces.
  /// </summary>
  public class TrueTypeFace
  {
    #region Types

    private struct FaceDefinition
    {
      #region Fields / Properties

      public string Path;
      public int Size;

      #endregion
    }

    private struct CharacterMetric
    {
      #region Fields / Properties

      public bool IsDrawn;
      public Size Size;
      public Point Bearing;
      public float Advance;
      public Dictionary<char, float> KerningPairs;
      public Vector3[] PositionOffsets;
      public Vector2[] UV0;

      #endregion
    };

    #endregion

    #region Fields / Properties

    internal static GLDynamicTexture AtlasTexture;
    private const int AtlasSize = 1024;
    private const int Dpi = 96;


    private static readonly BoxPacker BoxPacker = new BoxPacker(AtlasSize, AtlasSize);
    private static readonly Dictionary<FaceDefinition, TrueTypeFace> LoadedFonts =
      new Dictionary<FaceDefinition, TrueTypeFace>();

    private static readonly Library Library = new Library();
    private readonly Face _face;
    private readonly Dictionary<char, CharacterMetric> _characterMetrics = new Dictionary<char, CharacterMetric>();

    #endregion

    private TrueTypeFace(string filePath, int size)
    {
      _face = new Face(Library, filePath);
      _face.SetCharSize(0, size, 0, Dpi);
      LoadCharacterSet(string.Join("", Enumerable.Range(32, 126 - 32).Select(v => ((char) v))));
    }

    public static TrueTypeFace GetFont(string path, int size)
    {
      var definition = new FaceDefinition {Path = path, Size = size};
      lock (Library)
      {
        TrueTypeFace font = null;
        if (LoadedFonts.TryGetValue(definition, out font)) return font;
        font = new TrueTypeFace(path, size);
        LoadedFonts.Add(definition, font);
        return font;
      }
    }

    internal GLMesh CreateMeshFromText(string text, Color4 color)
    {
      var pen = new Vector3();
      var positions = new List<Vector3>();
      var uvs = new List<Vector2>();
      var indicies = new List<uint>();
      // ReSharper disable once LoopCanBePartlyConvertedToQuery
      for (var i = 0; i < text.Length; i++)
      {
        var character = text[i];
        // For now, ignore unknown chacaters. It's not a great solution...
        if (!_characterMetrics.ContainsKey(character)) continue;
        var metric = _characterMetrics[character];
        if (metric.IsDrawn)
        {
          var indexStart = positions.Count;
          pen.X += i < (text.Length - 1) ? metric.KerningPairs[text[i + 1]] : 0;
          positions.AddRange(metric.PositionOffsets.Select(p => p + pen));
          // Filp UVs
          uvs.AddRange(metric.UV0.Select(uv => new Vector2(uv.X, 1.0f - uv.Y)));
          indicies.AddRange(new[]
          {
            (uint) indexStart + 0, (uint) indexStart + 1, (uint) indexStart + 2,
            (uint) indexStart + 0, (uint) indexStart + 3, (uint) indexStart + 1
          });
        }
        pen.X += metric.Advance;
      }
      var colors = positions.Select(p => color);
      return new GLMesh(positions.ToArray(), indicies.ToArray(), uv0: uvs.ToArray(), colors: colors.ToArray());
    }

    private void LoadCharacterSet(string characters)
    {
      using (var graphics = Graphics.FromImage(AtlasTexture.Bitmap))
      {
        graphics.CompositingQuality = CompositingQuality.HighQuality;
        graphics.SmoothingMode = SmoothingMode.HighQuality;
        // DEBUG
        using (Brush aGradientBrush = new LinearGradientBrush(new Point(0, 0), new Point(0, 1024), Color.Blue, Color.Green))
        {
          graphics.FillRectangle(aGradientBrush, 0, 0, 1024, 1024);
        }
        foreach (var character in characters)
        {
          var glyphIndex = _face.GetCharIndex(character);
          _face.LoadGlyph(glyphIndex, LoadFlags.Default, LoadTarget.Normal);
          _face.Glyph.RenderGlyph(RenderMode.Normal);
          // Get all Kerning pairs
          Dictionary<char, float> kerningPairs = null;
          if (_face.HasKerning)
          {
            kerningPairs = new Dictionary<char, float>();
            // Load all kerning pairs
            foreach (var otherCharacter in characters)
            {
              var kerning =
                (float) _face.GetKerning(glyphIndex, _face.GetCharIndex(otherCharacter), KerningMode.Default).X;
              kerningPairs.Add(otherCharacter, kerning);
            }
          }
          // Draw the image, if there is one (might not be, for example 'space')
          var ftbmp = _face.Glyph.Bitmap;
          if (ftbmp.Width > 0 && ftbmp.Rows > 0)
          {
            // Draw the Glyph to the atlas Bitmap
            var cBmp = ftbmp.ToGdipBitmap(Color.Red);
            var drawLocation = BoxPacker.TryPackingBox(cBmp.Size);
            if (!drawLocation.HasValue) throw new Exception("Failed to pack chacter");
            // Draw using the bottom left as (0, 0) not the top left.
            var flippedDrawLocation = new Point(drawLocation.Value.X,
              AtlasSize - drawLocation.Value.Y - cBmp.Height);
            graphics.DrawImageUnscaled(cBmp, flippedDrawLocation);
            // Some quick helper to keep Position and UV stuff a little cleaner.
            var positionLowerLeft = new Vector3(_face.Glyph.BitmapLeft, -(cBmp.Height - _face.Glyph.BitmapTop), 0);
            var uvLowerLeft = new Vector2(drawLocation.Value.X/(float) AtlasSize,
              drawLocation.Value.Y/(float) AtlasSize);
            var uvSize = new Vector2(cBmp.Width/(float) AtlasSize, cBmp.Height/(float) AtlasSize);
            _characterMetrics.Add(character, new CharacterMetric
            {
              IsDrawn = true,
              PositionOffsets = new[]
              {
                positionLowerLeft, positionLowerLeft + new Vector3(cBmp.Width, cBmp.Height, 0),
                positionLowerLeft + new Vector3(0, cBmp.Height, 0), positionLowerLeft + new Vector3(cBmp.Width, 0, 0)
              },
              UV0 = new[]
              {
                uvLowerLeft, uvLowerLeft + uvSize, new Vector2(uvLowerLeft.X, uvLowerLeft.Y + uvSize.Y),
                new Vector2(uvLowerLeft.X + uvSize.X, uvLowerLeft.Y)
              },
              Size = cBmp.Size,
              Bearing = new Point(_face.Glyph.BitmapLeft, _face.Glyph.BitmapTop),
              Advance = (int) _face.Glyph.Advance.X,
              KerningPairs = kerningPairs
            });
          }
          else
          {
            _characterMetrics.Add(character, new CharacterMetric
            {
              IsDrawn = false,
              Advance = (int) _face.Glyph.Advance.X,
              KerningPairs = kerningPairs
            });
          }
        }
      }
      // Update GPU texture
      if (AtlasTexture != null) return;
      AtlasTexture = new GLDynamicTexture(GLTextureParams.Default, AtlasSize, AtlasSize);
      AtlasTexture.GpuAllocateDeferred();
    }
  }
}