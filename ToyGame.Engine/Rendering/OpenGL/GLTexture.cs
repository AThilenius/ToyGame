using System;
using System.Diagnostics;
using System.Threading;
using OpenTK.Graphics.OpenGL;

namespace ToyGame.Rendering.OpenGL
{
  internal sealed class GLTexture : IDisposable, IComparable<GLTexture>
  {
    #region Fields / Properties

    private readonly GLHandle _glHandle;

    #endregion

    private GLTexture(RenderCore renderCore)
    {
      _glHandle = new GLHandle {RenderCore = renderCore};
    }

    public int CompareTo(GLTexture other)
    {
      return _glHandle.CompareTo(other._glHandle);
    }

    public void Dispose()
    {
      _glHandle.RenderCore.AddResourceLoadAction(() => GL.DeleteTexture(_glHandle.Handle));
    }

    public static GLTexture LoadGLTexture(RenderCore renderCore, uint width, uint height,
      GLTextureParams textureParams, IntPtr data)
    {
      var texture = new GLTexture(renderCore);
      renderCore.AddResourceLoadAction(() =>
      {
        texture._glHandle.Handle = GL.GenTexture();
        GL.BindTexture(textureParams.Target, texture._glHandle.Handle);
        if (textureParams.UseAnisotropicFiltering)
        {
          float maxAniso;
          GL.GetFloat((GetPName) ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt, out maxAniso);
          GL.TexParameter(textureParams.Target,
            (TextureParameterName) ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt,
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
      });
      return texture;
    }

    /// <summary>
    ///   This must be called on the GPU thread.
    /// </summary>
    /// <param name="textureUnit"></param>
    public void Bind(TextureUnit textureUnit)
    {
      Debug.Assert(Thread.CurrentThread.Name == RenderCore.GpuThreadName);
      Debug.Assert(_glHandle.Handle != -1);
      GL.ActiveTexture(textureUnit);
      GL.BindTexture(TextureTarget.Texture2D, _glHandle.Handle);
    }
  }
}