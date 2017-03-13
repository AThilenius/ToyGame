using System;
using OpenTK.Graphics.OpenGL;

namespace ToyGame.Rendering.Shaders
{
  internal sealed class GLShaderStage : IDisposable
  {
    public GLShaderStage(ShaderType type, string code)
    {
      Handle = GL.CreateShader(type);
      // Compile vertex shader
      GL.ShaderSource(Handle, code);
      GL.CompileShader(Handle);
      string info;
      int statusCode;
      GL.GetShaderInfoLog(Handle, out info);
      GL.GetShader(Handle, ShaderParameter.CompileStatus, out statusCode);
      if (statusCode != 1)
      {
        throw new ApplicationException(info);
      }
    }

    public int Handle { get; }

    public void Dispose()
    {
      GL.DeleteShader(Handle);
    }
  }
}