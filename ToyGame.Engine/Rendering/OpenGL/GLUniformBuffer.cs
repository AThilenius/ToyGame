using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace ToyGame.Rendering.OpenGL
{
  internal class GLUniformBuffer : GLResource<GLUniformBuffer>
  {
    #region Fields / Properties

    public readonly int Size;
    private readonly int _bindingBlock;

    #endregion

    public GLUniformBuffer(RenderContext renderContext, int size, int bindingBlock) : base(renderContext)
    {
      Size = size;
      _bindingBlock = bindingBlock;
    }

    /// <summary>
    /// Buffers an array of Matrix4s to this Uniform Buffer (copies data to GPU).
    /// </summary>
    /// <param name="offset"></param>
    /// <param name="data"></param>
    public void BufferMatrix4(int offset, Matrix4[] data)
    {
      var size = Marshal.SizeOf<Matrix4>()*data.Length;
      RenderContext.AddPreRenderAction(() =>
      {
        Debug.Assert(GLHandle != -1);
        GL.BindBuffer(BufferTarget.UniformBuffer, GLHandle);
        GL.BufferSubData(BufferTarget.UniformBuffer, (IntPtr) offset, (IntPtr) size, data);
      });
    }

    protected override void LoadToGpu()
    {
      GL.BindBuffer(BufferTarget.UniformBuffer, GLHandle);
      GL.BufferData(BufferTarget.UniformBuffer, (IntPtr) Size, IntPtr.Zero, BufferUsageHint.StaticDraw);
      GL.BindBuffer(BufferTarget.UniformBuffer, 0);
      GL.BindBufferRange(BufferRangeTarget.UniformBuffer, _bindingBlock, GLHandle, IntPtr.Zero, (IntPtr) Size);
    }
  }
}