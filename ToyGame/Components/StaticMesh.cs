using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace ToyGame
{
  class StaticMesh
  {

    public readonly Geometry Geometry;
    public readonly Material Material;

    private readonly VertexArrayObject vertexArrayObject;

    public StaticMesh(Geometry geometry, Material material)
    {
      Geometry = geometry;
      Material = material;
      vertexArrayObject = new VertexArrayObject(geometry.VertexBuffers, geometry.IndexBuffer, material);
    }

    public void Draw()
    {
      Material.Bind();
      vertexArrayObject.Bind();
      GL.DrawElements(PrimitiveType.Triangles, Geometry.IndexBuffer.BufferCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
    }

  }
}
