using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ToyGame.OpenGL.Shaders
{
  internal abstract class GLShaderProgram : IDisposable
  {
    protected readonly Dictionary<string, int> AttributeLocations = new Dictionary<string, int>();
    protected readonly int Handle = GL.CreateProgram();
    protected readonly Dictionary<string, int> UniformLocations = new Dictionary<string, int>();
    public Matrix4 ModelMatrix = Matrix4.Identity;
    public Matrix4 ProjectionMatrix = Matrix4.Identity;
    public Matrix4 ViewMatrix = Matrix4.Identity;

    public void Dispose()
    {
      GL.DeleteProgram(Handle);
    }

    public int GetAttributeLocation(string name)
    {
      int location;
      return AttributeLocations.TryGetValue(name, out location) ? location : -1;
    }

    public int GetUniformLocation(string name)
    {
      int location;
      return UniformLocations.TryGetValue(name, out location) ? location : -1;
    }

    public void Use()
    {
      GL.UseProgram(Handle);
    }

    public virtual void BindUniforms()
    {
      GL.UniformMatrix4(GetUniformLocation("projectionMatrix"), false, ref ProjectionMatrix);
      GL.UniformMatrix4(GetUniformLocation("viewMatrix"), false, ref ViewMatrix);
      GL.UniformMatrix4(GetUniformLocation("modelMatrix"), false, ref ModelMatrix);
    }

    protected void Compile(GLShaderStage[] shaders, string[] attributes, string[] uniforms)
    {
      var allUniforms = new List<string>();
      allUniforms.AddRange(new[] {"projectionMatrix", "viewMatrix", "modelMatrix"});
      allUniforms.AddRange(uniforms);
      foreach (var shader in shaders)
      {
        GL.AttachShader(Handle, shader.Handle);
      }
      GL.LinkProgram(Handle);
      foreach (var shader in shaders)
      {
        GL.DetachShader(Handle, shader.Handle);
      }
      GL.UseProgram(Handle);
      var error = GL.GetError();
      if (error != ErrorCode.NoError)
        Console.WriteLine("Error after use: " + error);
      // Get all Attribute locations
      foreach (var attrib in attributes)
      {
        var location = GL.GetAttribLocation(Handle, attrib);
        if (location < 0)
        {
          Console.WriteLine("Failed to find the attribute [" + attrib + "] in shader GLSL.");
        }
        AttributeLocations.Add(attrib, location);
      }
      // Get all Uniform locations
      foreach (var uniform in allUniforms)
      {
        var location = GL.GetUniformLocation(Handle, uniform);
        if (location < 0)
        {
          Console.WriteLine("Failed to find the uniform [" + uniform + "] in shader GLSL.");
        }
        UniformLocations.Add(uniform, location);
      }
    }
  }
}