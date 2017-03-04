using System;
using System.Collections.Generic;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;

namespace GenericGamedev.OpenTKIntro
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
    }

    protected override void OnResize(EventArgs e)
    {
      GL.Viewport(0, 0, Width, Height);
    }

    protected override void OnLoad(EventArgs e)
    {
      // Material
      PBRMaterial material = new PBRMaterial();
      material.ProjectionMatrix = Matrix4.CreatePerspectiveFieldOfView(
          MathHelper.PiOver2, 16f / 9, 0.1f, 1000f);
      // Geometry
      Vector3[] verts = new Vector3[] { new Vector3(-1, -1, -1.5f), new Vector3(1, 1, -1.5f), new Vector3(1, -1, -1.5f) };
      Vector3[] normals = new Vector3[] { new Vector3(0, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 0, 1) };
      uint[] indexes = new uint[] { 0, 1, 2 };
      Geometry geometry = new Geometry(verts, normals, indexes);
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
