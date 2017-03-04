using OpenTK.Graphics.OpenGL;
using System;

namespace GenericGamedev.OpenTKIntro {
  sealed class Shader {

    private readonly int handle;

    public int Handle { get { return this.handle; } }

    public Shader(ShaderType type, string code) {
      handle = GL.CreateShader(type);
      // Compile vertex shader
      GL.ShaderSource(handle, code);
      GL.CompileShader(handle);
      string info;
      int status_code;
      GL.GetShaderInfoLog(handle, out info);
      GL.GetShader(handle, ShaderParameter.CompileStatus, out status_code);
      if (status_code != 1)
      {
        throw new ApplicationException(info);
      }
    }

  }
}
