using OpenTK;
using OpenTK.Graphics;
using OpenTK.Platform;
using ProtoBuf.Meta;
using ToyGame.Rendering;
using ToyGame.Resources;
using ToyGame.Serialization.Surrogates;

namespace ToyGame
{
  public class ToyEngine
  {
    public ToyEngine(IWindowInfo windowInfo)
    {
      // Register ProtoBuf.Net Surrogates
      var model = RuntimeTypeModel.Default;
      model.Add(typeof (Vector2Surrogate), true);
      model.Add(typeof (Vector2), false).SetSurrogate(typeof (Vector2Surrogate));
      model.Add(typeof (Vector3Surrogate), true);
      model.Add(typeof (Vector3), false).SetSurrogate(typeof (Vector3Surrogate));
      model.Add(typeof (Color4Surrogate), true);
      model.Add(typeof (Color4), false).SetSurrogate(typeof (Color4Surrogate));
      model.Add(typeof (ResourceSurrogate), true);
      model.Add(typeof (Resource), false).SetSurrogate(typeof (ResourceSurrogate));
      // Bring the RenderContext up with the given OpenTK IGraphicsContext
      RenderContext.Initialize(windowInfo);
    }
  }
}