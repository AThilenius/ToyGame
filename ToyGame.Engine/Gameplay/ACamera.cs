using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace ToyGame.Gameplay
{
  public class ACamera : AActor
  {
    #region Fields / Properties

    /// <summary>
    ///   Only used for Perpective Cameras
    /// </summary>
    public float AspectRatio = 16.0f/9.0f;

    /// <summary>
    ///   Only used for Perpective Cameras, default 75 deg
    /// </summary>
    public float FieldOfView = 1.309f;

    /// <summary>
    ///   Only used for an Orthographic camera. These represent the coordinates for points on the screen. Defaults to -1, 1 for
    ///   both X and Y.
    /// </summary>
    public Rectangle OrthographicBounds = new Rectangle(-1, -1, 1, 1);

    public bool IsOrthographic;
    public Rectangle Viewport = new Rectangle(0, 0, 100, 100);
    public Color4 ClearColor = Color4.CornflowerBlue;
    public float NearZPlane = 1f;
    public float FarZPlane = 10000f;
    public ClearBufferMask ClearBufferMast = ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit;
    public Matrix4 ProjectionMatrix { get; private set; }
    public Matrix4 ViewMatrix { get; private set; }

    #endregion

    /// <summary>
    ///   This is just a hack until there is AActor ticking
    /// </summary>
    public void PreRender()
    {
      ProjectionMatrix = IsOrthographic
        ? Matrix4.CreateOrthographicOffCenter(OrthographicBounds.X, OrthographicBounds.Width,
          OrthographicBounds.Y, OrthographicBounds.Height, NearZPlane, FarZPlane)
        : Matrix4.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, NearZPlane, FarZPlane);
      Transform.Scale = Vector3.One;
      ViewMatrix = Matrix4.Invert(Transform.GetWorldMatrix());
    }
  }
}