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

    public GameWindow()
        // set window resolution, title, and default behaviour
        : base(1280, 720, GraphicsMode.Default, "OpenTK Intro",
        GameWindowFlags.Default, DisplayDevice.Default,
        // ask for an OpenGL 3.0 forward compatible context
        3, 0, GraphicsContextFlags.ForwardCompatible)
    {
      Console.WriteLine("gl version: " + GL.GetString(StringName.Version));
      // Enable Texturing
      GL.Enable(EnableCap.Texture2D);
      GL.Enable(EnableCap.Blend);
      GL.Enable(EnableCap.DepthTest);
      GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
    }

    protected override void OnResize(EventArgs e)
    {
      GL.Viewport(0, 0, Width, Height);
    }

    protected override void OnLoad(EventArgs e)
    {
      // Material
      PBRMaterial material = new PBRMaterial();
      Matrix4 worldMatrix = Matrix4.CreateRotationY((float) (Math.PI / 2.0f));
      worldMatrix *= Matrix4.CreateTranslation(0, 0, -25);

      material.ProjectionMatrix = worldMatrix * Matrix4.CreatePerspectiveFieldOfView(MathHelper.PiOver2, Width / (float) Height, 0.1f, 1000f);
      // Geometry
      Mesh mesh = new WavefrontObjLoader().LoadFromFile(@"C:\Users\Alec\thilenius\ToyGame\Assets\Models\AKM\akm.obj");
      Geometry geometry = new Geometry(mesh);
      // StaticMesh
      staticMesh = new StaticMesh(geometry, material);
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      // this is called every frame, put game logic here
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      GL.ClearColor(Color4.Purple);
      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
      staticMesh.Draw();
      this.SwapBuffers();
    }

  }
}
