using System.Collections.Generic;
using OpenTK;

namespace ToyGame.Utilities
{
  public static class Extensions
  {
    #region Dictionary

    public static TValue GetValueOrDefault<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key,
      TValue defValue)
    {
      TValue outVal;
      return dictionary.TryGetValue(key, out outVal) ? outVal : defValue;
    }

    #endregion

    #region OpenTK <--> BulletSharp

    public static BulletSharp.Math.Vector3 ToBullet(this Vector3 otkVector3)
    {
      return new BulletSharp.Math.Vector3(otkVector3.X, otkVector3.Y, otkVector3.Z);
    }

    public static BulletSharp.Math.Matrix ToBullet(this Matrix4 m)
    {
      return new BulletSharp.Math.Matrix
      {
        M11 = m.M11,
        M12 = m.M12,
        M13 = m.M13,
        M14 = m.M14,
        M21 = m.M21,
        M22 = m.M22,
        M23 = m.M23,
        M24 = m.M24,
        M31 = m.M31,
        M32 = m.M32,
        M33 = m.M33,
        M34 = m.M34,
        M41 = m.M41,
        M42 = m.M42,
        M43 = m.M43,
        M44 = m.M44
      };
    }

    public static Vector3 ToOpenTk(this BulletSharp.Math.Vector3 bsVector3)
    {
      return new Vector3(bsVector3.X, bsVector3.Y, bsVector3.Z);
    }

    public static Matrix4 ToOpenTk(this BulletSharp.Math.Matrix m)
    {
      return new Matrix4
      {
        M11 = m.M11,
        M12 = m.M12,
        M13 = m.M13,
        M14 = m.M14,
        M21 = m.M21,
        M22 = m.M22,
        M23 = m.M23,
        M24 = m.M24,
        M31 = m.M31,
        M32 = m.M32,
        M33 = m.M33,
        M34 = m.M34,
        M41 = m.M41,
        M42 = m.M42,
        M43 = m.M43,
        M44 = m.M44
      };
    }

    #endregion
  }
}