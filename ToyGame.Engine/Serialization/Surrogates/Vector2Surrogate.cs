using OpenTK;
using ProtoBuf;

namespace ToyGame.Serialization.Surrogates
{
  [ProtoContract]
  internal class Vector2Surrogate
  {
    #region Fields / Properties

    [ProtoMember(1)] public float X;
    [ProtoMember(2)] public float Y;

    #endregion

    public static implicit operator Vector2Surrogate(Vector2 vector)
    {
      return new Vector2Surrogate {X = vector.X, Y = vector.Y};
    }

    public static implicit operator Vector2(Vector2Surrogate surrogate)
    {
      return new Vector2(surrogate.X, surrogate.Y);
    }
  }
}