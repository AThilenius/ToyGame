using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace ToyGame.Rendering.OpenGL
{
  /// <summary>
  ///   Represents the set of VBO and IVO buffers needed to render a model (position, normal, uv, index...).
  /// </summary>
  internal sealed class GLMesh
  {
    public readonly GLVertexBufferObject IndexBuffer;
    public readonly GLVertexBufferObject[] VertexBuffers;

    public GLMesh(Vector3[] positions, uint[] indexes, Vector3[] normals = null, Vector2[] uv0 = null,
      Vector2[] uv1 = null, Color4[] colors = null, BufferUsageHint usage = BufferUsageHint.StaticDraw)
    {
      // Verts, normals, uv0, uv1 and colors must all be the same size, if they are used.
      Debug.Assert(
        (normals == null || positions.Length == normals.Length) && (uv0 == null || positions.Length == uv0.Length)
        && (uv1 == null || positions.Length == uv1.Length) && (colors == null || positions.Length == colors.Length),
        "There was a missmatch in the number of input types.");
      Debug.Assert(indexes.Length > 0 && indexes.Length%3 == 0, "Index count must be > 0 and a multiple of 3.");
      IndexBuffer = GLVertexBufferObject.FromData(indexes, BufferTarget.ElementArrayBuffer, usage);
      var vbos = new List<GLVertexBufferObject>();
      vbos.Add(GLVertexBufferObject.FromData(positions, BufferTarget.ArrayBuffer, usage,
        new GLVertexAttribute("position", 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0)));
      if (normals != null)
      {
        vbos.Add(GLVertexBufferObject.FromData(normals, BufferTarget.ArrayBuffer, usage,
          new GLVertexAttribute("normal", 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0)));
      }
      if (uv0 != null)
      {
        vbos.Add(GLVertexBufferObject.FromData(uv0, BufferTarget.ArrayBuffer, usage,
          new GLVertexAttribute("uv0", 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0)));
      }
      if (uv1 != null)
      {
        vbos.Add(GLVertexBufferObject.FromData(uv1, BufferTarget.ArrayBuffer, usage,
          new GLVertexAttribute("uv1", 2, VertexAttribPointerType.Float, false, Vector2.SizeInBytes, 0)));
      }
      if (colors != null)
      {
        vbos.Add(GLVertexBufferObject.FromData(colors, BufferTarget.ArrayBuffer, usage,
          new GLVertexAttribute("colors", 4, VertexAttribPointerType.Float, false, Marshal.SizeOf(typeof (Color4)), 0)));
      }
      VertexBuffers = vbos.ToArray();
    }
  }
}