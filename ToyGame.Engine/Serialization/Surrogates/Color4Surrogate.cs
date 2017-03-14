using OpenTK.Graphics;
using ProtoBuf;

namespace ToyGame.Serialization.Surrogates
{
  [ProtoContract]
  internal class Color4Surrogate
  {
    #region Fields / Properties

    [ProtoMember(4)] public float A;
    [ProtoMember(3)] public float B;
    [ProtoMember(2)] public float G;
    [ProtoMember(1)] public float R;

    #endregion

    public static implicit operator Color4Surrogate(Color4 color)
    {
      return new Color4Surrogate {R = color.R, G = color.G, B = color.B, A = color.A};
    }

    public static implicit operator Color4(Color4Surrogate surrogate)
    {
      return new Color4(surrogate.R, surrogate.G, surrogate.B, surrogate.A);
    }
  }
}