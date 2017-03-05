using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace ToyGame
{
  abstract class ShaderProgram : IDisposable
  {
    protected readonly int handle = GL.CreateProgram();
    protected readonly Dictionary<string, int> attributeLocations = new Dictionary<string, int>();
    protected readonly Dictionary<string, int> uniformLocations = new Dictionary<string, int>();

    public int GetAttributeLocation(string name)
    {
      int location;
      return attributeLocations.TryGetValue(name, out location) ? location : -1;
    }

    public int GetUniformLocation(string name)
    {
      int location;
      return uniformLocations.TryGetValue(name, out location) ? location : -1;
    }

    public void Use()
    {
      GL.UseProgram(handle);
    }

    public abstract void BindUniforms();
    
    protected void Compile (ShaderStage[] shaders, string[] attributes, string[] uniforms)
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
      GL.UseProgram(handle);
      // Get all Attribute locations
      foreach (string attrib in attributes)
      {
        int location = GL.GetAttribLocation(handle, attrib);
        if (location < 0)
        {
          Console.WriteLine("Failed to find the attribute [" + attrib + "] in shader GLSL.");
        }
        attributeLocations.Add(attrib, location);
      }
      // Get all Uniform locations
      foreach (string uniform in uniforms)
      {
        int location = GL.GetUniformLocation(handle, uniform);
        if (location < 0)
        {
          Console.WriteLine("Failed to find the uniform [" + uniform + "] in shader GLSL.");
        }
        uniformLocations.Add(uniform, location);
      }
    }

    public void Dispose()
    {
      GL.DeleteProgram(handle);
    }

  }
}
