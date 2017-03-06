using System;
using OpenTK.Graphics.OpenGL;

namespace ToyGame
{
  public class AStaticMesh : AActor, IRenderable
  {

    public MeshResource Mesh;
    public Material Material;

    public AStaticMesh(MeshResource mesh, Material material)
    {
      Mesh = mesh;
      Material = material;
    }

    public void Render(ACamera camera)
    {
      Material.Bind(camera, Transform.GetWorldMatrix());
      GLVertexArrayObject.FromPair(Mesh, Material.Program).Bind();
      GL.DrawElements(PrimitiveType.Triangles, Mesh.GLGeometry.IndexBuffer.BufferCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
    }
  }
}
