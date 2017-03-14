using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using OpenTK.Graphics.OpenGL;
using ToyGame.Rendering.OpenGL;

namespace ToyGame.Rendering.Shaders
{
  internal abstract class GLShaderProgram : IDisposable, IComparable<GLShaderProgram>
  {
    #region Fields / Properties

    public readonly GLHandle GLHandle;
    protected readonly Dictionary<string, int> AttributeLocations = new Dictionary<string, int>();
    protected readonly Dictionary<string, int> UniformLocations = new Dictionary<string, int>();

    #endregion

    protected GLShaderProgram(RenderCore renderCore)
    {
      GLHandle = new GLHandle {RenderCore = renderCore};
    }

    public int CompareTo(GLShaderProgram other)
    {
      return GLHandle.CompareTo(other.GLHandle);
    }

    public void Dispose()
    {
      GLHandle.RenderCore.AddResourceLoadAction(() => GL.DeleteProgram(GLHandle.Handle));
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

    /// <summary>
    ///   This must be called on the GPU thread.
    /// </summary>
    public void Use()
    {
      Debug.Assert(Thread.CurrentThread.Name == RenderCore.GpuThreadName);
      Debug.Assert(GLHandle.Handle != -1);
      GL.UseProgram(GLHandle.Handle);
    }

    protected void Compile(GLShaderStage[] shaders, string[] attributes, string[] uniforms)
    {
      GLHandle.RenderCore.AddResourceLoadAction(() =>
      {
        {
          var error = GL.GetError();
          if (error != ErrorCode.NoError)
            throw new Exception("Error after use: " + error);
        }
        GLHandle.Handle = GL.CreateProgram();
        var allUniforms = new List<string>();
        allUniforms.AddRange(new[] {"modelMatrix"});
        allUniforms.AddRange(uniforms);
        foreach (var shader in shaders)
        {
          GL.AttachShader(GLHandle.Handle, shader.GLHandle.Handle);
        }
        GL.LinkProgram(GLHandle.Handle);
        foreach (var shader in shaders)
        {
          GL.DetachShader(GLHandle.Handle, shader.GLHandle.Handle);
        }
        GL.UseProgram(GLHandle.Handle);
        {
          var error = GL.GetError();
          if (error != ErrorCode.NoError)
            throw new Exception("Error after use: " + error);
        }
        // Get all Attribute locations
        foreach (var attrib in attributes)
        {
          var location = GL.GetAttribLocation(GLHandle.Handle, attrib);
          if (location < 0)
          {
            throw new Exception("Failed to find the attribute [" + attrib + "] in shader GLSL.");
          }
          AttributeLocations.Add(attrib, location);
        }
        // Get all Uniform locations
        foreach (var uniform in allUniforms)
        {
          var location = GL.GetUniformLocation(GLHandle.Handle, uniform);
          if (location < 0)
          {
            throw new Exception("Failed to find the uniform [" + uniform + "] in shader GLSL.");
          }
          UniformLocations.Add(uniform, location);
        }
      });
    }
  }
}