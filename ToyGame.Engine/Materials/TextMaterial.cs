using OpenTK.Graphics.OpenGL;
using ToyGame.Rendering.OpenGL;
using ToyGame.Rendering.Shaders;
using ToyGame.Resources;

namespace ToyGame.Materials
{
  public class TextMaterial : Material
  {
    #region Fields / Properties

    internal GLTexture GlyphAtlasTexture;
    private static GLTextShader _glTextShader;

    #endregion

    public TextMaterial()
    {
      if (_glTextShader == null)
      {
        _glTextShader = new GLTextShader();
        _glTextShader.GpuAllocateDeferred();
      }
      Program = _glTextShader;
    }

    internal override GLDrawCall.GLTextureBind[] GenerateTexturebinds()
    {
      return new[]
      {
        new GLDrawCall.GLTextureBind(GlyphAtlasTexture, TextureTarget.Texture2D, TextureUnit.Texture0)
      };
    }
  }
}