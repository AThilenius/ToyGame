using System;
using ToyGame.Gameplay;
using ToyGame.Utilities;

namespace ToyGame.Rendering.OpenGL
{
  internal class GLForwardPipeline : RenderPipeline
  {
    #region Fields / Properties

    private readonly GLDrawCallBatch _drawCallBatch = new GLDrawCallBatch();

    #endregion

    public GLForwardPipeline(World world) : base(world)
    {
    }

    public override void RenderImmediate(ACamera camera, IRenderTarget target)
    {
      base.RenderImmediate(camera, target);
      // A forward renderer is pretty damn simple!
      target.Bind();
      // ReSharper disable once ForCanBeConvertedToForeach
      for (var i = 0; i < _drawCallBatch.FrontBuffer.Length; i++)
      {
        _drawCallBatch.FrontBuffer[i].Draw();
        DebugUtils.GLErrorCheck();
      }
      target.FinalizeRender();
    }

    public override void UpdateDrawCallBatches()
    {
      BoundWorld.EnqueueDrawCalls(_drawCallBatch);
      _drawCallBatch.FinalizeFrame();
    }

    internal override void SwapDrawCallBuffers()
    {
      _drawCallBatch.SwapDrawCallBuffers();
    }
  }
}