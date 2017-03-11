using System;
using OpenTK.Graphics.OpenGL;

namespace ToyGame.OpenGL
{
  internal sealed class GLTexture : IDisposable
  {
    private readonly int _handle = GL.GenTexture();

    private GLTexture(GLTextureParams textureParams)
    {
    }

    public void Dispose()
    {
      GL.DeleteTexture(_handle);
    }

    public void Bind(TextureUnit textureUnit)
    {
      GL.ActiveTexture(textureUnit);
      GL.BindTexture(TextureTarget.Texture2D, _handle);
    }

    internal static GLTexture LoadGLTexture(uint width, uint height, GLTextureParams textureParams, IntPtr data)
    {
      var texture = new GLTexture(textureParams);
      GL.BindTexture(textureParams.Target, texture._handle);
      if (textureParams.UseAnisotropicFiltering)
      {
        float maxAniso;
        GL.GetFloat((GetPName) ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt, out maxAniso);
        GL.TexParameter(textureParams.Target, (TextureParameterName) ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt,
          maxAniso);
      }
      GL.TexParameter(textureParams.Target, TextureParameterName.TextureMagFilter, (int) textureParams.MagFilter);
      GL.TexParameter(textureParams.Target, TextureParameterName.TextureMinFilter, (int) textureParams.MinFilter);
      GL.TexParameter(textureParams.Target, TextureParameterName.TextureWrapS, (int) textureParams.WrapS);
      GL.TexParameter(textureParams.Target, TextureParameterName.TextureWrapT, (int) textureParams.WrapT);
      GL.TexImage2D(textureParams.Target, 0, PixelInternalFormat.Rgba, (int) width, (int) height, 0,
        PixelFormat.Bgra, PixelType.UnsignedByte, data);
      if (textureParams.GenerateMipMaps)
      {
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
      }
      GL.BindTexture(textureParams.Target, 0);
      return texture;
    }
  }
}