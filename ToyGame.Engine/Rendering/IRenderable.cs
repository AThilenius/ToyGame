using ToyGame.Gameplay;
using ToyGame.Rendering.OpenGL;

namespace ToyGame.Rendering
{
  public interface IRenderable
  {
    void EnqueueDrawCalls(GLDrawCallBatch drawCallbatch);
  }
}