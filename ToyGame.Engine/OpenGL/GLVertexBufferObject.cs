using System;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace ToyGame.OpenGL
{
  internal sealed class GLVertexBufferObject : IDisposable
  {
    public readonly int BufferCount;
    private readonly int _handle = GL.GenBuffer();
    private readonly BufferTarget _target;
    public readonly GLVertexAttribute[] VertexAttributes;

    private GLVertexBufferObject(BufferTarget bufferTarget, GLVertexAttribute[] vertexAttributes, int bufferCount)
    {
      VertexAttributes = vertexAttributes;
      _target = bufferTarget;
      BufferCount = bufferCount;
    }

    public void Dispose()
    {
      GL.DeleteBuffer(_handle);
    }

    public static GLVertexBufferObject FromData<T>(T[] data, BufferTarget bufferTarget, BufferUsageHint bufferUsageHint,
      params GLVertexAttribute[] vertexAttributes) where T : struct
    {
      var vbo = new GLVertexBufferObject(bufferTarget, vertexAttributes, data.Length);
      vbo.Bind();
      GL.BufferData(bufferTarget, (IntPtr) (Marshal.SizeOf(typeof (T))*data.Length), data, bufferUsageHint);
      return vbo;
    }

    public void Bind()
    {
      GL.BindBuffer(_target, _handle);
    }
  }
}