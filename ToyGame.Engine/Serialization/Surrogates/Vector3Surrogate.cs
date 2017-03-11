using OpenTK;
using ProtoBuf;

namespace ToyGame.Serialization.Surrogates
{
  [ProtoContract]
  internal class Vector3Surrogate
  {
    [ProtoMember(1)] public float X;
    [ProtoMember(2)] public float Y;
    [ProtoMember(3)] public float Z;

    public static implicit operator Vector3Surrogate(Vector3 vector)
    {
      return new Vector3Surrogate {X = vector.X, Y = vector.Y, Z = vector.Z};
    }

    public static implicit operator Vector3(Vector3Surrogate surrogate)
    {
      return new Vector3(surrogate.X, surrogate.Y, surrogate.Z);
    }
  }
}