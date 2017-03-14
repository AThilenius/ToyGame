using System;
using OpenTK.Graphics.OpenGL;
using ToyGame.Rendering.OpenGL;

namespace ToyGame.Rendering.Shaders
{
  internal sealed class GLShaderStage : GLResource<GLShaderStage>
  {
    #region Fields / Properties

    private readonly string _code;

    #endregion

    public GLShaderStage(RenderContext renderContext, ShaderType type, string code)
      : base(renderContext, () => GL.CreateShader(type), GL.DeleteShader)
    {
      _code = code;
    }

    protected override void LoadToGpu()
    {
      // Compile vertex shader
      GL.ShaderSource(GLHandle, _code);
      GL.CompileShader(GLHandle);
      string info;
      int statusCode;
      GL.GetShaderInfoLog(GLHandle, out info);
      GL.GetShader(GLHandle, ShaderParameter.CompileStatus, out statusCode);
      if (statusCode != 1)
      {
        throw new ApplicationException(info);
      }
    }
  }
}