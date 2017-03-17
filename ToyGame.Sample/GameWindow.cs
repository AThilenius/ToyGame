using System;
using System.Diagnostics;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using ToyGame.Gameplay;
using ToyGame.Materials;
using ToyGame.Rendering;
using ToyGame.Resources;

namespace ToyGame.Sample
{
  internal sealed class GameWindow
  {
    #region Fields / Properties

    private readonly Stopwatch _stopWatch = Stopwatch.StartNew();
    private readonly NativeWindow _window;
    private ToyEngine _context;
    private World _world;
    private ACamera _camera;
    private AStaticMesh _staticMesh;
    private ResourceBundle _resourceBundle;
    private RenderViewport _renderViewport;

    #endregion

    public GameWindow()
    {
      _window = new NativeWindow(1680, 1050, "Toy Engine", GameWindowFlags.Default, GraphicsMode.Default,
        DisplayDevice.Default) {Visible = true};
    }

    public void Run()
    {
      Load();
      while (true)
      {
        _window.ProcessEvents();
        Render();
      }
    }

    private void Load()
    {
      _context = new ToyEngine(_window.WindowInfo);
      _world = new World();
      _renderViewport = new RenderViewport(_world, _window.WindowInfo);
      _camera = new ACamera
      {
        AspectRatio = (1680/(float) 1050),
        Viewport = new Rectangle(0, 0, 1680, 1050)
      };
      var material = new VoxelMaterial();
      _resourceBundle = ResourceBundleManager.Instance.AddBundleFromProjectPath(@"C:\Users\Alec\thilenius\ToyGame");
      var model =
        _resourceBundle.ImportResource(@"Assets\Models\build_blacksmith_01.fbx", @"ImportCache") as ModelResource;
      material.DiffuseTexture =
        _resourceBundle.ImportResource(@"Assets\Textures\build_building_01_a.tif", "ImportCache") as TextureResource;
      material.MetallicRoughnessTexture =
        _resourceBundle.ImportResource(@"Assets\Textures\build_building_01_sg.tif", "ImportCache") as TextureResource;
      _staticMesh = new AStaticMesh(model, material)
      {
        Transform =
        {
          Scale = new Vector3(0.1f),
          Position = new Vector3(0, -5, -200),
          Rotation = Quaternion.FromEulerAngles(0, 0, (float) -Math.PI/2.0f)
        }
      };

      var level = new Level();
      _world.AddLevel(level);
      level.AddActor(_staticMesh);
      level.AddActor(_camera);
      // Release the context to the rendering thread
    }

    private void Render()
    {
      //_staticMesh.Transform.Rotation = Quaternion.FromEulerAngles(0, _totalTime / 2.0f, (float) -Math.PI / 2.0f);
      _renderViewport.Render(_camera);
      RenderContext.Active.Synchronize();
    }
  }
}