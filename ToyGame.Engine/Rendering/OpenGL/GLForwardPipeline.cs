using ToyGame.Gameplay;
using ToyGame.Utilities;

namespace ToyGame.Rendering.OpenGL
{
  internal class GLForwardPipeline : RenderPipeline
  {
    #region Fields / Properties

    private readonly GLDrawCallBatch _drawCallBatch = new GLDrawCallBatch();

    #endregion

    public GLForwardPipeline(IRenderable renderable, ACamera camera) : base(renderable, camera)
    {
    }

    internal override void RenderImmediate()
    {
      base.RenderImmediate();
      // ReSharper disable once ForCanBeConvertedToForeach
      for (var i = 0; i < _drawCallBatch.FrontBuffer.Length; i++)
      {
        _drawCallBatch.FrontBuffer[i].Draw();
        DebugUtils.GLErrorCheck();
      }
    }

    public override void UpdateDrawCallBatches()
    {
      Renderable.EnqueueDrawCalls(_drawCallBatch);
      _drawCallBatch.FinalizeFrame();
    }

    internal override void SwapDrawCallBuffers()
    {
      _drawCallBatch.SwapDrawCallBuffers();
    }
  }
}