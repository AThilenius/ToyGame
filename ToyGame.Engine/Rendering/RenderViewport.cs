using OpenTK.Platform;
using ToyGame.Gameplay;
using ToyGame.Rendering.OpenGL;

namespace ToyGame.Rendering
{
  public class RenderViewport
  {
    #region Fields / Properties

    public readonly World World;
    private readonly GLForwardPipeline _forwardPipeline;
    private readonly GLRenderTargetViewport _renderTarget;

    #endregion

    public RenderViewport(World world, IWindowInfo windowInfoHack)
    {
      World = world;
      _forwardPipeline = new GLForwardPipeline(world);
      _renderTarget = new GLRenderTargetViewport(windowInfoHack);
    }

    public void Render(ACamera camera)
    {
      _forwardPipeline.UpdateDrawCallBatches();
      RenderContext.Active.AddPipelineRun(_forwardPipeline, _renderTarget, camera);
    }
  }
}