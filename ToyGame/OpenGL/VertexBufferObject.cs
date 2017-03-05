using System;
using OpenTK.Graphics.OpenGL;
using System.Runtime.InteropServices;

namespace ToyGame
{
  sealed class VertexBufferObject : IDisposable
  {

    public readonly VertexAttribute[] VertexAttributes;
    public readonly int BufferCount;

    private readonly int handle = GL.GenBuffer();
    private readonly BufferTarget target;

    private VertexBufferObject(BufferTarget bufferTarget, VertexAttribute[] vertexAttributes, int bufferCount)
    {
      VertexAttributes = vertexAttributes;
      target = bufferTarget;
      BufferCount = bufferCount;
    }

    public static VertexBufferObject FromData<T>(T[] data, BufferTarget bufferTarget, BufferUsageHint bufferUsageHint, params VertexAttribute[] vertexAttributes) where T : struct
    {
      VertexBufferObject vbo = new VertexBufferObject(bufferTarget, vertexAttributes, data.Length);
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
