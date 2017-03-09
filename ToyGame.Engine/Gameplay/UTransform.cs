﻿using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToyGame
{
  public class UTransform
  {

    public AActor Actor;
    public Vector3 Position = Vector3.Zero;
    public Quaternion Rotation = Quaternion.Identity;
    public Vector3 Scale = Vector3.One;

    public UTransform(AActor parent)
    {
      Actor = parent;
    }

    public Matrix4 GetLocalMatrix()
    {
      return Matrix4.CreateScale(Scale) * Matrix4.CreateFromQuaternion(Rotation) * Matrix4.CreateTranslation(Position);
      //Matrix4 model = Matrix4.Identity;
      //model = Matrix4.Mult(model, Matrix4.Scale(Scale));
      //model = Matrix4.Mult(model, Matrix4.CreateTranslation(Position));
      //model = Matrix4.Mult(model, Matrix4.Rotate(Rotation));
      //return model;
    }

    public Matrix4 GetWorldMatrix()
    {
      return Actor.Parent != null ? GetLocalMatrix() * Actor.Parent.Transform.GetWorldMatrix() : GetLocalMatrix();
    }

  }
}
