using System;
using System.Collections.Generic;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Input;
using ToyGame.Gameplay;
using ToyGame.GUI;
using ToyGame.Rendering;
using ToyGame.Rendering.OpenGL;
using ToyGame.Utilities;

namespace ToyGame
{
  public class Window
  {
    #region Fields / Properties

    public readonly World World;
    public readonly NativeWindow NativeWindow;
    public readonly float DpiScale;

    /// <summary>
    ///   The true size of the window in pixels, after it has been stretched to fit the DPI scaling of the host OS.
    /// </summary>
    public Size UnscaledSize => new Size(NativeWindow.Width, NativeWindow.Height);

    public event EventHandler<EventArgs> Resize;
    public event EventHandler<EventArgs> Closing;
    public ACamera GuiCamera;
    public ACamera MainCamera;
    public MainGuiElement MainGuiElement;

    /// <summary>
    ///   The size of the window as it was requested, before any DPI scaling. For example, if the window was requested at
    ///   1680x1050, that will be that.
    /// </summary>
    public Size Size
    {
      get { return _requestedSize; }
      set { NativeWindow.Size = _requestedSize = value; }
    }

    private static bool _renderContextNeedsInit = true;
    private readonly RenderViewport _viewport;
    private readonly WindowStatistics _statistics = new WindowStatistics();
    private Size _requestedSize;

    #endregion

    internal Window(string title, int width, int height, World world = null)
    {
      _requestedSize = new Size(width, height);
      // World Init
      World = world ?? new World();
      // Native window and Render Context init
      NativeWindow = new NativeWindow(width, height, title, GameWindowFlags.Default, GraphicsMode.Default,
        DisplayDevice.Default)
      {Visible = true, Location = new Point(50, 50)};
      DpiScale = NativeWindow.Width/(float) width;
      if (_renderContextNeedsInit)
      {
        RenderContext.Initialize(NativeWindow.WindowInfo);
        _renderContextNeedsInit = false;
      }
      // Cameras
      GuiCamera = new ACamera
      {
        IsOrthographic = true,
        Viewport = new Rectangle(0, 0, UnscaledSize.Width, UnscaledSize.Height),
        OrthographicBounds = new Rectangle(0, 0, UnscaledSize.Width, UnscaledSize.Height),
        NearZPlane = -100,
        FarZPlane = 100,
        ClearBufferMast = ClearBufferMask.DepthBufferBit
      };
      MainCamera = new ACamera
      {
        AspectRatio = (UnscaledSize.Width/(float) UnscaledSize.Height),
        Viewport = new Rectangle(0, 0, UnscaledSize.Width, UnscaledSize.Height),
        ClearColor = Color.CornflowerBlue
      };
      // Main Gui GuiElement
      MainGuiElement = new MainGuiElement(this)
      {
        Children = new List<GuiElement>
        {
          new GuiElement
          {
            BackgroundColor = Color.DarkRed,
            Width = (SFixed) 300,
            MouseEnter = elem => elem.BackgroundColor = Color.White,
            MouseLeave = elem => elem.BackgroundColor = Color.DarkRed,
            ChildAlignment = ChildAlignments.Column,
            Children = new List<GuiElement>
            {
              new GuiElement
              {
                BackgroundColor = Color.DarkMagenta,
                Height = (SFixed) 200,
                MouseEnter = elem => elem.BackgroundColor = Color.White,
                MouseLeave = elem => elem.BackgroundColor = Color.DarkMagenta,
              }
            }
          },
          new GuiElement {Width = new SFill()},
          new GuiElement {BackgroundColor = Color.DarkGreen, Width = (SFixed) 400}
        }
      };
      MainGuiElement.LayoutChildren(new Rectangle(0, 0, NativeWindow.Width, NativeWindow.Height));
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
      _statistics.Update();
      World.Update();
      NativeWindow.Title = _statistics.DebugString;
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
        GuiCamera.Viewport = new Rectangle(0, 0, UnscaledSize.Width, UnscaledSize.Height);
        GuiCamera.OrthographicBounds = new Rectangle(0, 0, NativeWindow.Width, NativeWindow.Height);
        MainCamera.AspectRatio = (UnscaledSize.Width/(float) UnscaledSize.Height);
        MainCamera.Viewport = new Rectangle(0, 0, UnscaledSize.Width, UnscaledSize.Height);
        MainGuiElement.Width = (SFixed) NativeWindow.Width;
        MainGuiElement.Height = (SFixed) NativeWindow.Height;
        MainGuiElement.LayoutChildren(new Rectangle(0, 0, NativeWindow.Width, NativeWindow.Height));
        Resize?.Invoke(this, args);
      };
      // Closing
      NativeWindow.Closing += (sender, args) => Closing?.Invoke(this, args);
    }
  }
}