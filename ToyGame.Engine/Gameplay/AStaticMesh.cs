using System;
using System.Linq;
using OpenTK.Graphics.OpenGL;
using ToyGame.Materials;
using ToyGame.Rendering;
using ToyGame.Rendering.OpenGL;
using ToyGame.Resources;

namespace ToyGame.Gameplay
{
  public class AStaticMesh : AActor, IRenderable
  {
    #region Fields / Properties

    public readonly Material Material;
    public readonly ModelResource Model;
    private GLVertexArrayObject[] _vertexArrayObjects;

    #endregion

    public AStaticMesh(ModelResource mesh, Material material)
    {
      Model = mesh;
      Material = material;
    }

    void IRenderable.EnqueueDrawCalls(GLDrawCallBatch drawCallbatch)
    {
      if (_vertexArrayObjects == null)
      {
        _vertexArrayObjects =
          Model.GLMeshes.Select(glMesh =>
          {
            var vao = new GLVertexArrayObject(glMesh, Material.Program);
            vao.GpuAllocateDeferred();
            return vao;
          }).ToArray();
      }
      for (var i = 0; i < Model.GLMeshes.Length; i++)
      {
        var glMesh = Model.GLMeshes[i];
        drawCallbatch.AddDrawCall(new GLDrawCall(
          Material.Program,
          Material.GenerateTexturebinds(),
          _vertexArrayObjects[i],
          Material.GenerateUniformBinds(Transform.GetWorldMatrix()),
          () =>
            GL.DrawElements(PrimitiveType.Triangles, glMesh.IndexBuffer.BufferCount, DrawElementsType.UnsignedInt,
              IntPtr.Zero)));
      }
    }
  }
}