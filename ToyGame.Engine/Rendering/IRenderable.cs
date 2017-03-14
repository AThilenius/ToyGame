using ToyGame.Gameplay;

namespace ToyGame.Rendering
{
  public interface IRenderable
  {
    void EnqueueDrawCalls(RenderContext renderContext, ACamera camera);
  }
}