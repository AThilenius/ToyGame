using ToyGame.Rendering;

namespace ToyGame.Gameplay
{
  public interface IRenderable
  {
    void EnqueueDrawCalls(RenderCore renderCore, ACamera camera);
  }
}