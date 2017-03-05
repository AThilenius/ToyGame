using System;
using System.Linq;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using FileFormatWavefront;

namespace ToyGame
{
  sealed class GameWindow : OpenTK.GameWindow
  {

    private StaticMesh staticMesh;
    private StandardMaterial material;
    private float totalTime;

    public GameWindow()
        // set window resolution, title, and default behaviour
        : base(1280, 720, new GraphicsMode(32, 24, 0, 8), "OpenTK Intro",
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
      GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
    }

    protected override void OnResize(EventArgs e)
    {
      GL.Viewport(0, 0, Width, Height);
    }

    protected override void OnLoad(EventArgs e)
    {
      // Material
      material = new StandardMaterial();
      material.ModelMatrix = Matrix4.CreateRotationY((float) (Math.PI / 2.0f)) * Matrix4.CreateTranslation(0, 0, -25);
      material.ViewMatrix = Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver2, Width / (float) Height, 0.1f, 1000f);
      // Geometry
      Mesh mesh = new WavefrontObjLoader().LoadFromFile(@"C:\Users\Alec\thilenius\ToyGame\Assets\Models\AKM\akm.obj");
      Geometry geometry = new Geometry(mesh);
      // StaticMesh
      staticMesh = new StaticMesh(geometry, material);
      var error = GL.GetError();
      if (error != ErrorCode.NoError)
        Console.WriteLine("GLError: " + error.ToString());
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      totalTime += (float) e.Time;
      material.ModelMatrix = Matrix4.CreateRotationY(totalTime) * Matrix4.CreateTranslation(0, 0, -30);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      GL.ClearColor(Color4.Purple);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
      staticMesh.Draw();
      var error = GL.GetError();
      if (error != ErrorCode.NoError)
        Console.WriteLine("Error post new Renderer(): " + error.ToString());
      this.SwapBuffers();
    }

  }
}
