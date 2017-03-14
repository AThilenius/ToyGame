using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using OpenTK.Graphics.OpenGL;

namespace ToyGame.Rendering.OpenGL
{
  internal sealed class GLVertexBufferObject : GLResource<GLVertexBufferObject>
  {
    #region Fields / Properties

    public readonly int BufferCount;
    public readonly GLVertexAttribute[] VertexAttributes;
    private readonly BufferTarget _target;
    private Action _bufferAction;

    #endregion

    public GLVertexBufferObject(RenderContext renderContext, BufferTarget bufferTarget,
      GLVertexAttribute[] vertexAttributes, int bufferCount) : base(renderContext)
    {
      VertexAttributes = vertexAttributes;
      _target = bufferTarget;
      BufferCount = bufferCount;
    }

    public static GLVertexBufferObject FromData<T>(RenderContext renderContext, T[] data, BufferTarget bufferTarget,
      BufferUsageHint bufferUsageHint, params GLVertexAttribute[] vertexAttributes) where T : struct
    {
      var vertexBufferObject = new GLVertexBufferObject(renderContext, bufferTarget, vertexAttributes, data.Length);
      vertexBufferObject.SetData(data, bufferUsageHint);
      vertexBufferObject.GpuAllocate();
      return vertexBufferObject;
    }

    /// <summary>
    /// Sets the data for this VBO. This will take effect once the resource is first
    /// loaded into GPU memory, or (if it's already loaded) upon the next RenderContext
    /// resource cycle.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="data"></param>
    /// <param name="bufferUsageHint"></param>
    public void SetData<T>(T[] data, BufferUsageHint bufferUsageHint) where T : struct
    {
      // Must be captured in a closure because it's a generic type.
      _bufferAction = () =>
      {
        Bind();
        GL.BufferData(_target, (IntPtr) (Marshal.SizeOf(typeof (T))*data.Length), data, bufferUsageHint);
      };
      if (GLHandle != -1)
      {
        RenderContext.AddResourceLoadAction(_bufferAction);
      }
    }

    /// <summary>
    ///   This must be called on the GPU thread.
    /// </summary>
    public void Bind()
    {
      Debug.Assert(Thread.CurrentThread.Name == RenderContext.GpuThreadName);
      Debug.Assert(GLHandle != -1);
      GL.BindBuffer(_target, GLHandle);
    }

    protected override void LoadToGpu()
    {
      _bufferAction?.Invoke();
    }
  }
}