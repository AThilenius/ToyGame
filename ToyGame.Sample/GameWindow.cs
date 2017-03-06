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
      ACamera camera = new ACamera { AspectRatio = (Width / (float) Height) };
      StandardMaterial material = new StandardMaterial();
      MeshResource mesh = ResourceManager.FromPath<MeshResource>(@"C:\Users\Alec\thilenius\ToyGame\Assets\Models\AKM\akm.obj");
      staticMesh = new AStaticMesh(mesh, material);
      staticMesh.Transform.Position = new Vector3(0, 0, -50);
      ULevel level = new ULevel();
      world.AddLevel(level);
      level.AddActor(staticMesh);
      level.AddActor(camera);
      var error = GL.GetError();
      if (error != ErrorCode.NoError)
        Console.WriteLine("GLError: " + error.ToString());
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      totalTime += (float) e.Time;
      staticMesh.Transform.Rotation = Quaternion.FromEulerAngles(0, totalTime, 0);
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
