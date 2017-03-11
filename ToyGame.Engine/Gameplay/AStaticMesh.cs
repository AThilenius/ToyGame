using System;
using OpenTK.Graphics.OpenGL;
using ToyGame.Materials;
using ToyGame.OpenGL;
using ToyGame.Resources;

namespace ToyGame.Gameplay
{
  public class AStaticMesh : AActor, IRenderable
  {
    public Material Material;
    public ModelResource Model;

    public AStaticMesh(ModelResource mesh, Material material)
    {
      Model = mesh;
      Material = material;
    }

    public void Render(ACamera camera)
    {
      Material.Bind(camera, Transform.GetWorldMatrix());
      foreach (var glMesh in Model.GLMeshes)
      {
        GLVertexArrayObject.FromPair(glMesh, Material.Program).Bind();
        GL.DrawElements(PrimitiveType.Triangles, glMesh.IndexBuffer.BufferCount, DrawElementsType.UnsignedInt,
          IntPtr.Zero);
      }
    }
  }
}