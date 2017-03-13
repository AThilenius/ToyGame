using System;
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

    public Material Material;
    public ModelResource Model;

    #endregion

    public AStaticMesh(ModelResource mesh, Material material)
    {
      Model = mesh;
      Material = material;
    }

    public void EnqueueDrawCalls(RenderCore renderCore, ACamera camera)
    {
      foreach (var glMesh in Model.GLMeshes)
      {
        renderCore.AddSingleFrameDrawCall(new GLDrawCall(
          Material.Program,
          Material.GenerateTexturebinds(),
          GLVertexArrayObject.FromPair(glMesh, Material.Program),
          Material.GenerateUniformBinds(camera, Transform.GetWorldMatrix()),
          () =>
            GL.DrawElements(PrimitiveType.Triangles, glMesh.IndexBuffer.BufferCount, DrawElementsType.UnsignedInt,
              IntPtr.Zero)));
      }
    }
  }
}