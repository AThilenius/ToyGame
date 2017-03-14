using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ToyGame.Rendering.OpenGL
{
  internal class GLUniformBuffer : IDisposable, IComparable<GLUniformBuffer>
  {
    #region Fields / Properties

    private readonly GLHandle _glHandle;

    #endregion

    public GLUniformBuffer(RenderCore renderCore, int size, int bindingBlock)
    {
      _glHandle = new GLHandle {RenderCore = renderCore};
      renderCore.AddResourceLoadAction(() =>
      {
        _glHandle.Handle = GL.GenBuffer();
        GL.BindBuffer(BufferTarget.UniformBuffer, _glHandle.Handle);
        GL.BufferData(BufferTarget.UniformBuffer, (IntPtr) size, IntPtr.Zero, BufferUsageHint.StaticDraw);
        GL.BindBuffer(BufferTarget.UniformBuffer, 0);
        GL.BindBufferRange(BufferRangeTarget.UniformBuffer, 0, _glHandle.Handle, IntPtr.Zero, (IntPtr) size);
      });
    }

    public int CompareTo(GLUniformBuffer other)
    {
      return _glHandle.CompareTo(other._glHandle);
    }

    public void Dispose()
    {
      _glHandle.RenderCore.AddResourceLoadAction(() => GL.DeleteTexture(_glHandle.Handle));
    }

    public void BufferMatrix4(int offset, Matrix4[] data)
    {
      var size = Marshal.SizeOf<Matrix4>()*data.Length;
      _glHandle.RenderCore.AddPreRenderAction(() =>
      {
        Debug.Assert(_glHandle.Handle != -1);
        GL.BindBuffer(BufferTarget.UniformBuffer, _glHandle.Handle);
        GL.BufferSubData(BufferTarget.UniformBuffer, (IntPtr) offset, (IntPtr) size, data);
        // GL.BindBuffer(BufferTarget.UniformBuffer, 0);
      });
    }
  }
}