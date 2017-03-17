using System;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using ToyGame.Gameplay;
using ToyGame.Rendering.OpenGL;
using ToyGame.Utilities;

namespace ToyGame.Rendering
{
  internal abstract class RenderPipeline
  {
    #region Fields / Properties

    public readonly World BoundWorld;
    private readonly GLUniformBuffer _uniformBuffer;

    #endregion

    protected RenderPipeline(World world)
    {
      BoundWorld = world;
      _uniformBuffer = new GLUniformBuffer(Marshal.SizeOf<Matrix4>()*2, 0);
      _uniformBuffer.GpuAllocateDeferred();
    }

    /// <summary>
    ///   Updates all needed GLDrawCallBuffers for the pipline from the bound World and finalizes them.
    /// </summary>
    public abstract void UpdateDrawCallBatches();

    /// <summary>
    ///   Called directly by the [GPU Thread] to render the full pipline using the given Camera into the given IRenderTarget
    /// </summary>
    public virtual void RenderImmediate(ACamera camera, IRenderTarget target)
    {
      // Update the Camera's View Projection matrecies
      camera.PreRender();
      DebugUtils.GLErrorCheck();
      GL.Viewport(camera.Viewport);
      GL.ClearColor(camera.ClearColor);
      GL.Clear(camera.ClearBufferMast);
      _uniformBuffer.BufferMatrix4(0, new[] {camera.ProjectionMatrix, camera.ViewMatrix});
      DebugUtils.GLErrorCheck();
    }

    internal abstract void SwapDrawCallBuffers();
  }
}