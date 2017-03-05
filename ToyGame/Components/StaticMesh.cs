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

    private readonly VertexArrayObject vertexArrayObject;
    private readonly Geometry geometry;
    private readonly Material material;

    public StaticMesh(Geometry geometry, Material material)
    {
      this.geometry = geometry;
      this.material = material;
      vertexArrayObject = new VertexArrayObject(geometry.VertexBuffers, geometry.IndexBuffer, material);
    }

    public void Draw()
    {
      material.Bind();
      vertexArrayObject.Bind();
      GL.DrawElements(PrimitiveType.Triangles, geometry.IndexBuffer.BufferCount, DrawElementsType.UnsignedInt, IntPtr.Zero);
    }

  }
}
