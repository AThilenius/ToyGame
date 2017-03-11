using System.IO;
using System.Reflection;
using OpenTK;
using OpenTK.Graphics;
using ProtoBuf.Meta;
using ToyGame.Serialization.Surrogates;

namespace ToyGame
{
  public class ToyEngineContext
  {

    public ToyEngineContext()
    {
      // Register ProtoBuf.Net Surrogates
      var model = RuntimeTypeModel.Default;
      model.Add(typeof(Vector2Surrogate), true);
      model.Add(typeof(Vector2), false).SetSurrogate(typeof(Vector2Surrogate));
      model.Add(typeof(Vector3Surrogate), true);
      model.Add(typeof(Vector3), false).SetSurrogate(typeof(Vector3Surrogate));
      model.Add(typeof(Color4Surrogate), true);
      model.Add(typeof(Color4), false).SetSurrogate(typeof(Color4Surrogate));
    }

  }
}