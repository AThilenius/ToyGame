using System;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace ToyGame
{
  sealed class GLVertexBufferObject : IDisposable
  {

    public readonly GLVertexAttribute[] VertexAttributes;
    public readonly int BufferCount;

    private readonly int handle = GL.GenBuffer();
    private readonly BufferTarget target;

    private GLVertexBufferObject(BufferTarget bufferTarget, GLVertexAttribute[] vertexAttributes, int bufferCount)
    {
      VertexAttributes = vertexAttributes;
      target = bufferTarget;
      BufferCount = bufferCount;
    }

    public static GLVertexBufferObject FromData<T>(T[] data, BufferTarget bufferTarget, BufferUsageHint bufferUsageHint, params GLVertexAttribute[] vertexAttributes) where T : struct
    {
      GLVertexBufferObject vbo = new GLVertexBufferObject(bufferTarget, vertexAttributes, data.Length);
      vbo.Bind();
      GL.BufferData(bufferTarget, (IntPtr) (Marshal.SizeOf(typeof(T)) * data.Length), data, bufferUsageHint);
      return vbo;
    }

    public void Bind()
    {
      GL.BindBuffer(target, handle);
    }

    public void Dispose()
    {
      GL.DeleteBuffer(handle);
    }

  }
}
