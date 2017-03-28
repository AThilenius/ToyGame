using System;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Threading;
using FreeImageAPI;
using OpenTK.Graphics.OpenGL;
using ToyGame.Utilities;
using PixelFormat = OpenTK.Graphics.OpenGL.PixelFormat;

namespace ToyGame.Rendering.OpenGL
{
  internal sealed class GLTexture : GLResource<GLTexture>
  {
    #region Fields / Properties

    public readonly GLTextureParams TextureParams;
    public uint Width;
    public uint Height;
    private readonly IntPtr _data = IntPtr.Zero;
    private Bitmap _bitmap;

    #endregion

    public GLTexture(GLTextureParams textureParams, FIBITMAP bitmap)
      : base(GL.GenTexture, GL.DeleteTexture)
    {
      Width = FreeImage.GetWidth(bitmap);
      Height = FreeImage.GetHeight(bitmap);
      TextureParams = textureParams;
      _data = FreeImage.GetBits(bitmap);
    }

    public GLTexture(GLTextureParams textureParams, Image bitmap)
      : base(GL.GenTexture, GL.DeleteTexture)
    {
      Width = (uint) bitmap.Width;
      Height = (uint) bitmap.Height;
      TextureParams = textureParams;
      _bitmap = new Bitmap(bitmap);
    }

    /// <summary>
    ///   This must be called on the GPU thread.
    /// </summary>
    /// <param name="textureUnit"></param>
    public void Bind(TextureUnit textureUnit)
    {
      Debug.Assert(Thread.CurrentThread.Name == RenderContext.GpuThreadName);
      Debug.Assert(GLHandle != -1);
      GL.ActiveTexture(textureUnit);
      GL.BindTexture(TextureTarget.Texture2D, GLHandle);
    }

    public void Update(Image bitmap)
    {
      UpdateNeeded = true;
      Width = (uint) bitmap.Width;
      Height = (uint) bitmap.Height;
      _bitmap = new Bitmap(bitmap);
    }

    protected override void LoadToGpu()
    {
      GL.BindTexture(TextureParams.Target, GLHandle);
      if (TextureParams.UseAnisotropicFiltering)
      {
        float maxAniso;
        GL.GetFloat((GetPName) ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt, out maxAniso);
        GL.TexParameter(TextureParams.Target,
          (TextureParameterName) ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt,
          maxAniso);
      }
      GL.TexParameter(TextureParams.Target, TextureParameterName.TextureMagFilter, (int) TextureParams.MagFilter);
      GL.TexParameter(TextureParams.Target, TextureParameterName.TextureMinFilter, (int) TextureParams.MinFilter);
      GL.TexParameter(TextureParams.Target, TextureParameterName.TextureWrapS, (int) TextureParams.WrapS);
      GL.TexParameter(TextureParams.Target, TextureParameterName.TextureWrapT, (int) TextureParams.WrapT);
      // Buffer data
      // TODO: This is a fucking mess, clean this bull shit up. Damn textures are annoying.
      if (_data != IntPtr.Zero)
      {
        GL.TexImage2D(TextureParams.Target, 0, PixelInternalFormat.Rgba, (int) Width, (int) Height, 0,
          PixelFormat.Bgra, PixelType.UnsignedByte, _data);
        FreeImage.FreeHbitmap(_data);
      }
      else if (_bitmap != null)
      {
        var data = _bitmap.LockBits(new Rectangle(0, 0, _bitmap.Width, _bitmap.Height),
          ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
        GL.TexImage2D(TextureParams.Target, 0, PixelInternalFormat.Rgba, (int) Width, (int) Height, 0,
          PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
        _bitmap.UnlockBits(data);
        _bitmap.Dispose();
        _bitmap = null;
      }
      else Console.WriteLine(@"Trying to load a null GLTexture");
      if (TextureParams.GenerateMipMaps) GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
      GL.BindTexture(TextureParams.Target, 0);
      DebugUtils.GLErrorCheck();
    }
  }
}