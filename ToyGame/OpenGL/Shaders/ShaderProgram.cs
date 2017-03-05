using OpenTK.Graphics.OpenGL;
using System;

namespace ToyGame
{
  sealed class ShaderProgram : IDisposable
  {
    private readonly int handle = GL.CreateProgram();

    public ShaderProgram(params ShaderStage[] shaders)
    {
      foreach (ShaderStage shader in shaders)
      {
        GL.AttachShader(handle, shader.Handle);
      }
      GL.LinkProgram(handle);
      foreach (ShaderStage shader in shaders)
      {
        GL.DetachShader(handle, shader.Handle);
      }
    }

    public int GetAttributeLocation(string name)
    {
      return GL.GetAttribLocation(handle, name);
    }

    public int GetUniformLocation(string name)
    {
      return GL.GetUniformLocation(handle, name);
    }

    public void Use()
    {
      GL.UseProgram(handle);
    }

    public void Dispose()
    {
      GL.DeleteProgram(handle);
    }

  }
}
