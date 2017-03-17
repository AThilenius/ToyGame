using System;
using System.Diagnostics;
using System.Threading;
using FreeImageAPI;
using OpenTK.Graphics.OpenGL;

namespace ToyGame.Rendering.OpenGL
{
  internal sealed class GLTexture : GLResource<GLTexture>
  {
    #region Fields / Properties

    public readonly uint Width;
    public readonly uint Height;
    public readonly GLTextureParams TextureParams;
    private readonly IntPtr _data;

    #endregion

    public GLTexture(GLTextureParams textureParams, FIBITMAP bitmap)
      : base(GL.GenTexture, GL.DeleteTexture)
    {
      Width = FreeImage.GetWidth(bitmap);
      Height = FreeImage.GetHeight(bitmap);
      TextureParams = textureParams;
      _data = FreeImage.GetBits(bitmap);
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
      GL.TexImage2D(TextureParams.Target, 0, PixelInternalFormat.Rgba, (int) Width, (int) Height, 0,
        PixelFormat.Bgra, PixelType.UnsignedByte, _data);
      FreeImage.FreeHbitmap(_data);
      if (TextureParams.GenerateMipMaps)
      {
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
      }
      GL.BindTexture(TextureParams.Target, 0);
    }
  }
}