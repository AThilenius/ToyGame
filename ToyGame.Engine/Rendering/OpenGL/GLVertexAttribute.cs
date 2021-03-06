﻿using System.Diagnostics;
using System.Threading;
using OpenTK.Graphics.OpenGL;
using ToyGame.Rendering.Shaders;

namespace ToyGame.Rendering.OpenGL
{
  internal sealed class GLVertexAttribute
  {
    #region Fields / Properties

    private readonly string _name;
    private readonly bool _normalize;
    private readonly int _offset;
    private readonly int _size;
    private readonly int _stride;
    private readonly VertexAttribPointerType _type;

    #endregion

    public GLVertexAttribute(string name, int size, VertexAttribPointerType type, bool normalize, int stride, int offset)
    {
      _name = name;
      _size = size;
      _type = type;
      _normalize = normalize;
      _stride = stride;
      _offset = offset;
    }

    /// <summary>
    ///   This must be called on the GPU thread.
    /// </summary>
    /// <param name="shaderProgram"></param>
    public void SetIfPresent(GLShaderProgram shaderProgram)
    {
      Debug.Assert(shaderProgram.GLHandle != -1);
      Debug.Assert(Thread.CurrentThread.Name == RenderContext.GpuThreadName);
      // Get location of attribute from shader program
      var index = shaderProgram.GetAttributeLocation(_name);
      if (index < 0) return;
      // Enable and set attribute
      GL.EnableVertexAttribArray(index);
      GL.VertexAttribPointer(index, _size, _type, _normalize, _stride, _offset);
    }
  }
}