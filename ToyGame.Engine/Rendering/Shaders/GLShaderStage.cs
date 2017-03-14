using System;
using OpenTK.Graphics.OpenGL;
using ToyGame.Rendering.OpenGL;

namespace ToyGame.Rendering.Shaders
{
  internal sealed class GLShaderStage : IDisposable
  {
    #region Fields / Properties

    public readonly GLHandle GLHandle;

    #endregion

    public GLShaderStage(RenderCore renderCore, ShaderType type, string code)
    {
      GLHandle = new GLHandle {RenderCore = renderCore};
      renderCore.AddResourceLoadAction(() =>
      {
        GLHandle.Handle = GL.CreateShader(type);
        // Compile vertex shader
        GL.ShaderSource(GLHandle.Handle, code);
        GL.CompileShader(GLHandle.Handle);
        string info;
        int statusCode;
        GL.GetShaderInfoLog(GLHandle.Handle, out info);
        GL.GetShader(GLHandle.Handle, ShaderParameter.CompileStatus, out statusCode);
        if (statusCode != 1)
        {
          throw new ApplicationException(info);
        }
      });
    }

    public void Dispose()
    {
      GLHandle.RenderCore.AddResourceLoadAction(() => GL.DeleteShader(GLHandle.Handle));
    }
  }
}