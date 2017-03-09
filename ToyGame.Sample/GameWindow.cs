using System;
using System.Linq;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace ToyGame.Sample
{
  sealed class GameWindow : OpenTK.GameWindow
  {

    private UWorld world = new UWorld();
    private AStaticMesh staticMesh;
    private float totalTime;

    public GameWindow()
        // set window resolution, title, and default behaviour
        : base(1680, 1050, new GraphicsMode(32, 24, 0, 8), "OpenTK Intro",
        GameWindowFlags.Default, DisplayDevice.Default,
        // Ask for an OpenGL 4.2 forward compatible context
        4, 2, GraphicsContextFlags.ForwardCompatible)
    {
      Console.WriteLine("gl version: " + GL.GetString(StringName.Version));
      // Enable Texturing
      GL.Enable(EnableCap.Texture2D);
      GL.Enable(EnableCap.Blend);
      GL.Enable(EnableCap.DepthTest);
      GL.Enable(EnableCap.Multisample);
      GL.CullFace(CullFaceMode.Back);
      GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
    }

    protected override void OnResize(EventArgs e)
    {
      GL.Viewport(0, 0, Width, Height);
    }

    protected override void OnLoad(EventArgs e)
    {
      VoxResource vox = ResourceManager.FromPath<VoxResource>(@"C:\Users\Alec\Desktop\tank\pbs_test\pbs_test.vox");
      ACamera camera = new ACamera { AspectRatio = (Width / (float) Height) };

      VoxelMaterial material = new VoxelMaterial();

      //ModelResource model = ResourceManager.FromPath<ModelResource>(@"C:\Users\Alec\thilenius\ToyGame\Assets\Models\MaterialPreview.fbx");
      //staticMesh = new AStaticMesh(model, material);
      //staticMesh.Transform.Position = new Vector3(0, -150, -300);

      //ModelResource model = ResourceManager.FromPath<ModelResource>(@"C:\Users\Alec\thilenius\ToyGame\Assets\Models\AKM\WPN_AKM.FBX");
      //staticMesh = new AStaticMesh(model, material);
      //staticMesh.Transform.Position = new Vector3(0, 0, -30);

      //ModelResource mesh = ResourceManager.FromPath<ModelResource>(@"C:\Users\Alec\thilenius\ToyGame\Assets\Models\build_crane_01.FBX");
      //staticMesh = new AStaticMesh(model, material);
      //staticMesh.Transform.Position = new Vector3(0, -500, -1300);

      //ModelResource model = ResourceManager.FromPath<ModelResource>(@"C:\Users\Alec\Desktop\sphere.fbx");
      //staticMesh = new AStaticMesh(model, material);
      //staticMesh.Transform.Scale = new Vector3(0.1f);
      //staticMesh.Transform.Position = new Vector3(0, 0, -20);

      ModelResource model = ResourceManager.FromPath<ModelResource>(@"C:\Users\Alec\Desktop\tank\pbs_test\sample.obj");
      staticMesh = new AStaticMesh(model, material);
      staticMesh.Transform.Position = new Vector3(0, 0, -20);
      staticMesh.Transform.Rotation = Quaternion.FromEulerAngles(0, 0, (float) Math.PI / 2.0f);

      ULevel level = new ULevel();
      world.AddLevel(level);
      level.AddActor(staticMesh);
      //level.AddActor(barracs);
      level.AddActor(camera);
      var error = GL.GetError();
      if (error != ErrorCode.NoError)
        Console.WriteLine("GLError: " + error.ToString());
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      totalTime += (float) e.Time;
      staticMesh.Transform.Rotation = Quaternion.FromEulerAngles(0, totalTime / 2.0f, (float) -Math.PI / 2.0f);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      GL.ClearColor(Color4.Purple);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
      world.Render();
      var error = GL.GetError();
      if (error != ErrorCode.NoError)
        Console.WriteLine("Error post new Renderer(): " + error.ToString());
      this.SwapBuffers();
    }

  }
}
