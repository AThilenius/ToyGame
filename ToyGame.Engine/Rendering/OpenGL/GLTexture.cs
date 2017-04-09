using System.Diagnostics;
using System.Threading;
using OpenTK.Graphics.OpenGL;
using ToyGame.Utilities;

namespace ToyGame.Rendering.OpenGL
{
  internal abstract class GLTexture : GLResource<GLTexture>
  {
    #region Fields / Properties

    public readonly GLTextureParams TextureParams;

    #endregion

    protected GLTexture(GLTextureParams textureParams) : base(GL.GenTexture, GL.DeleteTexture)
    {
      TextureParams = textureParams;
    }

    /// <summary>
    ///   This must be called on the GPU thread.
    /// </summary>
    /// <param name="textureUnit"></param>
    public virtual void Bind(TextureUnit textureUnit)
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
      DebugUtils.GLErrorCheck();
    }
  }
}