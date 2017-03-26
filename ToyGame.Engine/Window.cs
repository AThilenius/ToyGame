using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using ToyGame.Gameplay;
using ToyGame.Rendering;

namespace ToyGame
{
  public class Window
  {
    #region Fields / Properties

    public readonly World World;
    public readonly NativeWindow NativeWindow;
    public event EventHandler<EventArgs> Resize;
    public event EventHandler<EventArgs> Closing;
    public ACamera GuiCamera;
    public ACamera MainCamera;
    private readonly RenderViewport _viewport;
    private static bool renderContextNeedsInit = true;

    #endregion

    internal Window(string title, int width, int height, World world = null)
    {
      World = world ?? new World();
      NativeWindow = new NativeWindow(width, height, title, GameWindowFlags.Default, GraphicsMode.Default,
        DisplayDevice.Default) {Visible = true};
      if (renderContextNeedsInit)
      {
        RenderContext.Initialize(NativeWindow.WindowInfo);
        renderContextNeedsInit = false;
      }
      _viewport = new RenderViewport(World, NativeWindow.WindowInfo);
      NativeWindow.Resize += (sender, args) =>
      {
        GuiCamera.Viewport = new Rectangle(0, 0, NativeWindow.Width, NativeWindow.Height);
        GuiCamera.OrthographicBounds = new Rectangle(0, 0, NativeWindow.Width, NativeWindow.Height);
        MainCamera.AspectRatio = (NativeWindow.Width/(float) NativeWindow.Height);
        MainCamera.Viewport = new Rectangle(0, 0, NativeWindow.Width, NativeWindow.Height);
        Resize?.Invoke(this, args);
      };
      NativeWindow.Closing += (sender, args) => Closing?.Invoke(this, args);
      GuiCamera = new ACamera
      {
        IsOrthographic = true,
        Viewport = new Rectangle(0, 0, width, height),
        OrthographicBounds = new Rectangle(0, 0, width, height),
        NearZPlane = -100,
        FarZPlane = 100
      };
      MainCamera = new ACamera
      {
        AspectRatio = (width/(float) height),
        Viewport = new Rectangle(0, 0, width, height)
      };
    }

    internal void Update()
    {
      NativeWindow.ProcessEvents();
      Input.UpdateState(NativeWindow.Focused ? Keyboard.GetState() : new KeyboardState(),
        NativeWindow.Focused ? Mouse.GetState() : new MouseState());
      World.Update();
    }

    internal void Render()
    {
      // Enqueues draw calls for the world bound to the RenderTarget
      _viewport.Render(MainCamera);
    }
  }
}