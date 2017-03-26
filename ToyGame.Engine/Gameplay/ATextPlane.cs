using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using ToyGame.Materials;
using ToyGame.Rendering;
using ToyGame.Rendering.OpenGL;
using ToyGame.Resources;

namespace ToyGame.Gameplay
{
  public class ATextPlane : AActor, IRenderable
  {
    #region Fields / Properties

    private readonly GLMesh _glMesh;
    private readonly TextMaterial _textMaterial = new TextMaterial();
    private GLVertexArrayObject _vertexArrayObject;

    #endregion

    public ATextPlane(TrueTypeFace fontFace, string text, Color4 color)
    {
      _glMesh = fontFace.CreateMeshFromText(text, color);
      _textMaterial.GlyphAtlasTexture = TrueTypeFace.AtlasTexture;
    }

    public void EnqueueDrawCalls(GLDrawCallBatch drawCallbatch)
    {
      if (_vertexArrayObject == null)
      {
        _vertexArrayObject = new GLVertexArrayObject(_glMesh, _textMaterial.Program);
        _vertexArrayObject.GpuAllocateDeferred();
      }
      drawCallbatch.AddDrawCall(new GLDrawCall(
        _textMaterial.Program,
        _textMaterial.GenerateTexturebinds(),
        _vertexArrayObject,
        _textMaterial.GenerateUniformBinds(Transform.GetWorldMatrix()),
        () =>
          GL.DrawElements(PrimitiveType.Triangles, _glMesh.IndexBuffer.BufferCount, DrawElementsType.UnsignedInt,
            IntPtr.Zero)));
    }
  }
}