using System;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace ToyGame.Sample
{
  internal sealed class GameWindow : OpenTK.GameWindow
  {
    private ToyEngineContext _context;

    public GameWindow()
      // set window resolution, title, and default behaviour
      : base(1680, 1050, new GraphicsMode(32, 24, 0, 8), "OpenTK Intro",
        GameWindowFlags.Default, DisplayDevice.Default, 4, 2, GraphicsContextFlags.ForwardCompatible)
    {
    }

    protected override void OnResize(EventArgs e)
    {
      //GL.Viewport(0, 0, Width, Height);
    }

    protected override void OnLoad(EventArgs e)
    {
      _context = new ToyEngineContext();
      //var camera = new ACamera {AspectRatio = (Width/(float) Height)};

      //VoxelMaterial material = new VoxelMaterial();
      //ModelResource model = ResourceManager.FromPath<ModelResource>(@"C:\Users\Alec\Desktop\tank2\tank.obj");
      //material.DiffuseTexture = ResourceManager.FromPath<TextureResource>(@"C:\Users\Alec\Desktop\tank2\diffuse.png");
      //material.MetallicRoughnessTexture = ResourceManager.FromPath<TextureResource>(@"C:\Users\Alec\Desktop\tank2\metalic_roughness.png");
      //staticMesh = new AStaticMesh(model, material);
      //staticMesh.XTransform.Scale = new Vector3(0.5f);
      //staticMesh.XTransform.Position = new Vector3(0, -5, -20);

      //TextureResource texture = ResourceManager.FromPath<TextureResource>(@"C:\Users\Alec\thilenius\ToyGame\Assets\Models\AKM\textures\WPNT_AKM_BaseColor.tga");

      //var XLevel = new XLevel();
      //_world.AddLevel(XLevel);
      ////XLevel.AddActor(staticMesh);
      //XLevel.AddActor(camera);
      //var error = GL.GetError();
      //if (error != ErrorCode.NoError)
      //  Console.WriteLine("GLError: " + error);
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      //_totalTime += (float) e.Time;
      //staticMesh.XTransform.Rotation = Quaternion.FromEulerAngles(0, totalTime / 2.0f, (float) -Math.PI / 2.0f);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      //GL.ClearColor(Color4.Purple);
      //GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
      //_world.Render();
      //var error = GL.GetError();
      //if (error != ErrorCode.NoError)
      //  Console.WriteLine("Error post new Renderer(): " + error);
      //SwapBuffers();
    }
  }
}