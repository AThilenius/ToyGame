using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Input;
using ToyGame.Gameplay;
using ToyGame.GUI;
using ToyGame.Rendering;
using ToyGame.Rendering.OpenGL;

namespace ToyGame
{
  public class Window
  {
    #region Fields / Properties

    public readonly World World;
    public readonly NativeWindow NativeWindow;
    public readonly float DpiScale;

    public Size UnscaledSize
      => new Size((int) Math.Round(NativeWindow.Width*DpiScale), (int) Math.Round(NativeWindow.Height*DpiScale));

    public event EventHandler<EventArgs> Resize;
    public event EventHandler<EventArgs> Closing;
    public ACamera GuiCamera;
    public ACamera MainCamera;
    public MainGuiElement MainGuiElement;
    private static bool _renderContextNeedsInit = true;
    private readonly RenderViewport _viewport;
    private readonly GLForwardPipeline _guiPipeline;

    #endregion

    internal Window(string title, int width, int height, World world = null)
    {
      // World Init
      World = world ?? new World();
      // Native window and Render Context init
      NativeWindow = new NativeWindow(width, height, title, GameWindowFlags.Default, GraphicsMode.Default,
        DisplayDevice.Default) {Visible = true, Location = new Point(50, 50)};
      DpiScale = NativeWindow.Width/(float) width;
      if (_renderContextNeedsInit)
      {
        RenderContext.Initialize(NativeWindow.WindowInfo);
        _renderContextNeedsInit = false;
      }
      // Unscaled Window
      var unscaledSize = UnscaledSize;
      // Cameras
      GuiCamera = new ACamera
      {
        IsOrthographic = true,
        Viewport = new Rectangle(0, 0, unscaledSize.Width, unscaledSize.Height),
        OrthographicBounds = new Rectangle(0, 0, unscaledSize.Width, unscaledSize.Height),
        NearZPlane = -100,
        FarZPlane = 100,
        ClearColor = Color.Transparent
      };
      MainCamera = new ACamera
      {
        AspectRatio = (unscaledSize.Width/(float) unscaledSize.Height),
        Viewport = new Rectangle(0, 0, unscaledSize.Width, unscaledSize.Height)
      };
      // Main Gui GuiElement
      MainGuiElement = new MainGuiElement(this);
      // Event Handler Registration
      RegisterEventHandlers();
      // Viewport
      _viewport = new RenderViewport(NativeWindow.WindowInfo, new GLForwardPipeline(World, MainCamera),
        new GLForwardPipeline(MainGuiElement, GuiCamera));
    }

    internal void Update()
    {
      Input.UpdateState(NativeWindow.Focused ? Keyboard.GetState() : new KeyboardState(),
        NativeWindow.Focused ? Mouse.GetState() : new MouseState());
      NativeWindow.ProcessEvents();
      World.Update();
    }

    internal void Render()
    {
      // Enqueues draw calls for the world and main gui bound to the RenderTarget
      _viewport.Render();
    }

    private void RegisterEventHandlers()
    {
      // Resize
      NativeWindow.Resize += (sender, args) =>
      {
        // Note that GUI stays in scaled coordinates
        var unscaledSize = UnscaledSize;
        GuiCamera.Viewport = new Rectangle(0, 0, unscaledSize.Width, unscaledSize.Height);
        GuiCamera.OrthographicBounds = new Rectangle(0, 0, NativeWindow.Width, NativeWindow.Height);
        MainCamera.AspectRatio = (unscaledSize.Width/(float) unscaledSize.Height);
        MainCamera.Viewport = new Rectangle(0, 0, unscaledSize.Width, unscaledSize.Height);
        MainGuiElement.Width = (SFixed) NativeWindow.Width;
        MainGuiElement.Height = (SFixed) NativeWindow.Height;
        Resize?.Invoke(this, args);
      };
      // Closing
      NativeWindow.Closing += (sender, args) => Closing?.Invoke(this, args);
    }
  }
}