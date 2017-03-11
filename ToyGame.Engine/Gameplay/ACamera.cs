using OpenTK;

namespace ToyGame.Gameplay
{
  public class ACamera : AActor
  {
    public float AspectRatio = 16.0f/9.0f;
    // Default 75 degree FOV
    public float FieldOfView = 1.309f;
    public Matrix4 ProjectionMatrix { get; private set; }
    public Matrix4 ViewMatrix { get; private set; }

    public void PreRender()
    {
      ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(FieldOfView, AspectRatio, 1f, 10000f);
      Transform.Scale = Vector3.One;
      ViewMatrix = Transform.GetWorldMatrix();
    }
  }
}