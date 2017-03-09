using System;
using OpenTK.Graphics.OpenGL;

namespace ToyGame
{
  public class AStaticMesh : AActor, IRenderable
  {

    public ModelResource Model;
    public Material Material;

    public AStaticMesh(ModelResource mesh, Material material)
    {
      Model = mesh;
      Material = material;
    }

    public void Render(ACamera camera)
    {
      Material.Bind(camera, Transform.GetWorldMatrix());
      foreach (GLMesh glMesh in Model.GLMeshes)
      {
        GLVertexArrayObject.FromPair(glMesh, Material.Program).Bind();
        GL.DrawElements(PrimitiveType.Triangles, glMesh.IndexBuffer.BufferCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
      }
    }
  }
}
