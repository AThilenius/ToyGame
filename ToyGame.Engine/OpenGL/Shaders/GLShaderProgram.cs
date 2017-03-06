using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace ToyGame
{
  abstract class GLShaderProgram : IDisposable
  {

    public Matrix4 ProjectionMatrix = Matrix4.Identity;
    public Matrix4 ViewMatrix = Matrix4.Identity;
    public Matrix4 ModelMatrix = Matrix4.Identity;

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

    public virtual void BindUniforms()
    {
      GL.UniformMatrix4(GetUniformLocation("projectionMatrix"), false, ref ProjectionMatrix);
      GL.UniformMatrix4(GetUniformLocation("viewMatrix"), false, ref ViewMatrix);
      GL.UniformMatrix4(GetUniformLocation("modelMatrix"), false, ref ModelMatrix);
    }

    protected void Compile (GLShaderStage[] shaders, string[] attributes, string[] uniforms)
    {
      List<string> allUniforms = new List<string>();
      allUniforms.AddRange(new string[] { "projectionMatrix", "viewMatrix", "modelMatrix" });
      allUniforms.AddRange(uniforms);
      foreach (GLShaderStage shader in shaders)
      {
        GL.AttachShader(handle, shader.Handle);
      }
      GL.LinkProgram(handle);
      foreach (GLShaderStage shader in shaders)
      {
        GL.DetachShader(handle, shader.Handle);
      }
      GL.UseProgram(handle);
      var error = GL.GetError();
      if (error != ErrorCode.NoError)
        Console.WriteLine("Error after use: " + error.ToString());
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
      foreach (string uniform in allUniforms)
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
