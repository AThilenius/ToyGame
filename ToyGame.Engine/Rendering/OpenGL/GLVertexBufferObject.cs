using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using OpenTK.Graphics.OpenGL;

namespace ToyGame.Rendering.OpenGL
{
  internal sealed class GLVertexBufferObject : IDisposable
  {
    #region Fields / Properties

    public readonly int BufferCount;
    public readonly GLVertexAttribute[] VertexAttributes;
    private readonly GLHandle _glHandle;
    private readonly BufferTarget _target;

    #endregion

    private GLVertexBufferObject(RenderCore renderCore, BufferTarget bufferTarget, GLVertexAttribute[] vertexAttributes,
      int bufferCount)
    {
      VertexAttributes = vertexAttributes;
      _target = bufferTarget;
      BufferCount = bufferCount;
      _glHandle = new GLHandle {RenderCore = renderCore};
    }

    public void Dispose()
    {
      _glHandle.RenderCore.AddResourceLoadAction(() => GL.DeleteBuffer(_glHandle.Handle));
    }

    public static GLVertexBufferObject FromData<T>(RenderCore renderCore, T[] data, BufferTarget bufferTarget,
      BufferUsageHint bufferUsageHint, params GLVertexAttribute[] vertexAttributes) where T : struct
    {
      var vbo = new GLVertexBufferObject(renderCore, bufferTarget, vertexAttributes, data.Length);
      renderCore.AddResourceLoadAction(() =>
      {
        vbo._glHandle.Handle = GL.GenBuffer();
        vbo.Bind();
        GL.BufferData(bufferTarget, (IntPtr) (Marshal.SizeOf(typeof (T))*data.Length), data, bufferUsageHint);
      });
      return vbo;
    }

    /// <summary>
    ///   This must be called on the GPU thread.
    /// </summary>
    public void Bind()
    {
      Debug.Assert(Thread.CurrentThread.Name == RenderCore.GpuThreadName);
      Debug.Assert(_glHandle.Handle != -1);
      GL.BindBuffer(_target, _glHandle.Handle);
    }
  }
}