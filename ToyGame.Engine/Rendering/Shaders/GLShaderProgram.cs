using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using OpenTK.Graphics.OpenGL;
using ToyGame.Rendering.OpenGL;

namespace ToyGame.Rendering.Shaders
{
  internal abstract class GLShaderProgram : GLResource<GLShaderProgram>
  {
    #region Fields / Properties

    protected readonly Dictionary<string, int> AttributeLocations = new Dictionary<string, int>();
    protected readonly Dictionary<string, int> UniformLocations = new Dictionary<string, int>();
    private readonly GLShaderStage[] _shaders;
    private readonly string[] _attributes;
    private readonly string[] _uniforms;

    #endregion

    protected GLShaderProgram(RenderContext renderContext, GLShaderStage[] shaders, string[] attributes,
      string[] uniforms) : base(renderContext, GL.CreateProgram, GL.DeleteProgram)
    {
      _shaders = shaders;
      _attributes = attributes;
      _uniforms = uniforms;
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
    public void Bind()
    {
      Debug.Assert(Thread.CurrentThread.Name == RenderContext.GpuThreadName);
      Debug.Assert(GLHandle != -1);
      GL.UseProgram(GLHandle);
    }

    protected override void LoadToGpu()
    {
      var allUniforms = new List<string>();
      allUniforms.AddRange(new[] {"modelMatrix"});
      allUniforms.AddRange(_uniforms);
      foreach (var shader in _shaders)
      {
        // Make sure the shader is compiled before continuing
        shader.GpuAllocate();
        GL.AttachShader(GLHandle, shader.GLHandle);
      }
      GL.LinkProgram(GLHandle);
      foreach (var shader in _shaders)
      {
        GL.DetachShader(GLHandle, shader.GLHandle);
      }
      GL.UseProgram(GLHandle);
      // Get all Attribute locations
      foreach (var attrib in _attributes)
      {
        var location = GL.GetAttribLocation(GLHandle, attrib);
        if (location < 0)
        {
          throw new Exception("Failed to find the attribute [" + attrib + "] in shader GLSL.");
        }
        AttributeLocations.Add(attrib, location);
      }
      // Get all Uniform locations
      foreach (var uniform in allUniforms)
      {
        var location = GL.GetUniformLocation(GLHandle, uniform);
        if (location < 0)
        {
          throw new Exception("Failed to find the uniform [" + uniform + "] in shader GLSL.");
        }
        UniformLocations.Add(uniform, location);
      }
    }
  }
}