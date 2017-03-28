using OpenTK.Platform;
using ToyGame.Gameplay;
using ToyGame.Rendering.OpenGL;

namespace ToyGame.Rendering
{
  public class RenderViewport
  {
    #region Fields / Properties

    public readonly RenderPipeline[] RenderPipelines;
    private readonly GLRenderTargetViewport _renderTarget;

    #endregion

    public RenderViewport(IWindowInfo windowInfo, params RenderPipeline[] renderPipelines)
    {
      RenderPipelines = renderPipelines;
      _renderTarget = new GLRenderTargetViewport(windowInfo);
    }

    public void Render()
    {
      foreach (var pipline in RenderPipelines)
      {
        pipline.UpdateDrawCallBatches();
        RenderContext.Active.AddPipelineRun(pipline, _renderTarget);
      }
    }
  }
}