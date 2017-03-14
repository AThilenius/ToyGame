using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using OpenTK;
using OpenTK.Graphics;
using ToyGame.Gameplay;
using ToyGame.Materials;
using ToyGame.Rendering;
using ToyGame.Resources;

namespace ToyGame.Editor
{
  /// <summary>
  ///   Interaction logic for MainWindow.xaml
  /// </summary>
  public partial class MainWindow : Window
  {
    #region Fields / Properties

    private readonly Stopwatch _stopWatch = Stopwatch.StartNew();
    private readonly Timer _timer = new Timer {Enabled = true, Interval = 16};
    private XWorld _world;
    private ACamera _camera;
    private GLControl _glControl;
    private AStaticMesh _staticMesh;
    private ResourceBundle _resourceBundle;
    private ToyEngineContext _context = new ToyEngineContext();

    #endregion

    public MainWindow()
    {
      InitializeComponent();
    }

    private void WindowsFormsHost_Initialized(object sender, EventArgs e)
    {
      _glControl = new GLControl(GraphicsMode.Default);
      //_glControl.MakeCurrent();
      _glControl.Paint += GLControl_Paint;
      _glControl.Dock = DockStyle.Fill;
      _glControl.Resize += (resizeSender, resizeEvent) =>
      {
        if (_camera == null) return;
        _camera.AspectRatio = _glControl.Width/(float) _glControl.Height;
        _camera.Viewport = new Rectangle(0, 0, _glControl.Width, _glControl.Height);
      };
      ((WindowsFormsHost) sender).Child = _glControl;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
      //Console.Text += "gl version: " + GL.GetString(StringName.Version);
      _world = new XWorld(_glControl.WindowInfo);

      Console.Text += "\nWindow size: " + _glControl.Width + "x" + _glControl.Height;
      _camera = new ACamera(_world.RenderContext)
      {
        AspectRatio = (_glControl.Width/(float) _glControl.Height),
        Viewport = new Rectangle(0, 0, _glControl.Width, _glControl.Height)
      };
      var material = new VoxelMaterial(_world.RenderContext);
      _resourceBundle = ResourceBundleManager.Instance.AddBundleFromProjectPath(_world.RenderContext,
        @"C:\Users\Alec\thilenius\ToyGame");
      var model =
        _resourceBundle.ImportResource(@"Assets\Models\build_blacksmith_01.fbx", @"ImportCache") as ModelResource;
      material.DiffuseTexture =
        _resourceBundle.ImportResource(@"Assets\Textures\build_building_01_a.tif", "ImportCache") as TextureResource;
      material.MetallicRoughnessTexture =
        _resourceBundle.ImportResource(@"Assets\Textures\build_building_01_sg.tif", "ImportCache") as TextureResource;
      _staticMesh = new AStaticMesh(model, material)
      {
        Transform = {Scale = new Vector3(0.5f), Position = new Vector3(0, -5, -30)}
      };

      var level = new XLevel();
      _world.AddLevel(level);
      level.AddActor(_staticMesh);
      level.AddActor(_camera);

      _timer.Tick += (o, evnt) => { _glControl.Invalidate(); };
    }

    private void GLControl_Paint(object sender, PaintEventArgs e)
    {
      _staticMesh.Transform.Rotation = Quaternion.FromEulerAngles(0,
        (float) _stopWatch.Elapsed.TotalMilliseconds/2000.0f,
        (float) -Math.PI/2.0f);
      _world.EnqueueDrawCalls(_world.RenderContext);
      _world.RenderContext.FinalizeFrame();
      _world.RenderContext.SwapBuffers();
    }

    private void Window_Closing(object sender, CancelEventArgs e)
    {
      _world.RenderContext.ShutDown();
    }
  }
}