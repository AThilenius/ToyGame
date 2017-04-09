using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using ToyGame.Gameplay;
using ToyGame.Rendering.OpenGL;
using ToyGame.Utilities;

namespace ToyGame.Rendering
{
  public abstract class RenderPipeline
  {
    #region Fields / Properties

    public readonly IRenderable Renderable;
    public ACamera Camera;
    private static GLUniformBuffer _uniformBuffer;

    #endregion

    protected RenderPipeline(IRenderable renderable, ACamera camera)
    {
      Renderable = renderable;
      Camera = camera;
      if (_uniformBuffer != null) return;
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
    internal virtual void RenderImmediate()
    {
      // Update the Camera's View Projection matrecies
      Camera.PreRender();
      DebugUtils.GLErrorCheck();
      GL.Viewport(Camera.Viewport);
      GL.ClearColor(Camera.ClearColor);
      GL.Clear(Camera.ClearBufferMast);
      _uniformBuffer.BufferMatrix4Immediate(0, new[] {Camera.ProjectionMatrix, Camera.ViewMatrix});
      DebugUtils.GLErrorCheck();
    }

    internal abstract void SwapDrawCallBuffers();
  }
}