using System.Drawing;
using System.Runtime.InteropServices;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using ToyGame.Rendering;
using ToyGame.Rendering.OpenGL;

namespace ToyGame.Gameplay
{
  public class ACamera : AActor
  {
    #region Fields / Properties

    public float AspectRatio = 16.0f/9.0f;
    // Default 75 degree FOV
    public float FieldOfView = 1.309f;
    public Rectangle Viewport = new Rectangle(0, 0, 100, 100);
    public Color4 ClearColor = Color4.CornflowerBlue;
    public Matrix4 ProjectionMatrix { get; private set; }
    public Matrix4 ViewMatrix { get; private set; }
    private readonly GLUniformBuffer _uniformBuffer;
    private readonly RenderContext _renderContext;

    #endregion

    public ACamera(RenderContext renderContext)
    {
      _renderContext = renderContext;
      _uniformBuffer = new GLUniformBuffer(renderContext, Marshal.SizeOf<Matrix4>()*2, 0);
      _uniformBuffer.GpuAllocate();
    }

    public void PreRender()
    {
      ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, 1f, 10000f);
      Transform.Scale = Vector3.One;
      ViewMatrix = Transform.GetWorldMatrix();
      _renderContext.AddPreRenderAction(() =>
      {
        GL.Viewport(Viewport);
        GL.ClearColor(ClearColor);
        GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        _uniformBuffer.BufferMatrix4(0, new[] {ProjectionMatrix, ViewMatrix});
      });
    }
  }
}