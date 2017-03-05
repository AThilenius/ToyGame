using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ToyGame
{
  class Geometry
  {

    public readonly VertexBufferObject[] VertexBuffers;
    public readonly VertexBufferObject IndexBuffer;

    private static Geometry activeGeometry;

    public Geometry(Mesh mesh, BufferUsageHint usage = BufferUsageHint.StaticDraw)
      : this(mesh.Positions, mesh.Normals, mesh.Indices, mesh.UV0, null, null, usage)
    {
    }

    public Geometry(Vector3[] positions, Vector3[] normals, uint[] indexes, Vector2[] uv0 = null,
      Vector2[] uv1 = null, Color4[] colors = null, BufferUsageHint usage = BufferUsageHint.StaticDraw)
    {
      // Verts, normals, uv0, uv1 and colors must all be the same size, if they are used.
      Debug.Assert(positions.Length == normals.Length && (uv0 == null || positions.Length == uv0.Length)
        && (uv1 == null || positions.Length == uv1.Length) && (colors == null || positions.Length == colors.Length),
        "There was a missmatch in the number of input types.");
      Debug.Assert(indexes.Length > 0 && indexes.Length % 3 == 0, "Index count must be > 0 and a multiple of 3.");
      IndexBuffer = VertexBufferObject.FromData(indexes, BufferTarget.ElementArrayBuffer, usage);
      List<VertexBufferObject> vbos = new List<VertexBufferObject>();
      vbos.Add(VertexBufferObject.FromData(positions, BufferTarget.ArrayBuffer, usage,
        new VertexAttribute("position", 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0)));
      vbos.Add(VertexBufferObject.FromData(normals, BufferTarget.ArrayBuffer, usage,
       new VertexAttribute("normal", 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0)));
      if (uv0 != null)
      {
        vbos.Add(VertexBufferObject.FromData(uv0, BufferTarget.ArrayBuffer, usage,
         new VertexAttribute("uv0", 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0)));
      }
      if (uv1 != null)
      {
        vbos.Add(VertexBufferObject.FromData(uv1, BufferTarget.ArrayBuffer, usage,
         new VertexAttribute("uv1", 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0)));
      }
      if (colors != null)
      {
        vbos.Add(VertexBufferObject.FromData(colors, BufferTarget.ArrayBuffer, usage,
         new VertexAttribute("colors", 4, VertexAttribPointerType.Float, false, Marshal.SizeOf(typeof(Color4)), 0)));
      }
      VertexBuffers = vbos.ToArray();
    }

  }
}
