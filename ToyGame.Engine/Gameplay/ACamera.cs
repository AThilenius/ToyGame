using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

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
    public ClearBufferMask ClearBufferMast = ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit;
    public Matrix4 ProjectionMatrix { get; private set; }
    public Matrix4 ViewMatrix { get; private set; }

    #endregion

    /// <summary>
    ///   This is just a hack until there is AActor ticking
    /// </summary>
    public void PreRender()
    {
      ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, 1f, 10000f);
      Transform.Scale = Vector3.One;
      ViewMatrix = Transform.GetWorldMatrix();
    }
  }
}