using System;
using System.Collections.Concurrent;
using System.Threading;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using ToyGame.Rendering.OpenGL;
using ToyGame.Gameplay;
using ToyGame.Materials;
using ToyGame.Resources;
using System.Diagnostics;

namespace ToyGame.Rendering
{
  public class RenderCore
  {
    #region Fields / Properties

    public Color4 ClearColor = Color4.Purple;
    private readonly ConcurrentQueue<GLDrawCall> _staticDrawCalls = new ConcurrentQueue<GLDrawCall>();
    private readonly IWindowInfo _windowInfo;
    private IGraphicsContext _context;
    private ConcurrentQueue<GLDrawCall> _backDrawCallBuffer = new ConcurrentQueue<GLDrawCall>();
    private volatile object _swapLock = new object();
    private volatile GLDrawCall[] _frontBuffer;
    private volatile GLDrawCall[] _finalizedBuffer;
    private bool _shutdown;

    #endregion

    public RenderCore(IWindowInfo windowInfo)
    {
      _windowInfo = windowInfo;
      new Thread(DoGpuThreadWork) {Name = "OpenGL Render Thread"}.Start();
    }

    /// <summary>
    ///   On the calling thread, a sort is kicked off. The back draw buffer (dynamic) and static
    ///   object buffer are mixed and sorted together into a single draw buffer. After that an
    ///   Action is generated (but not executed) for all binding tasts the GPU thrad will need to
    ///   perform.
    /// </summary>
    public void FinalizeFrame()
    {
      // Merge and sort both static and dynamic draw calls
      _finalizedBuffer = new GLDrawCall[_backDrawCallBuffer.Count + _staticDrawCalls.Count];
      _backDrawCallBuffer.CopyTo(_finalizedBuffer, 0);
      _staticDrawCalls.CopyTo(_finalizedBuffer, _backDrawCallBuffer.Count);
      Array.Sort(_finalizedBuffer);
      // Generate drawing actions for all draw calls off-GPU thread.
      for (var i = 0; i < _finalizedBuffer.Length; i++)
      {
        _finalizedBuffer[i].GenerateBindAction(i > 0 ? _finalizedBuffer[i - 1] : null);
      }
    }

    /// <summary>
    ///   A lot goes on in this function:
    ///   - If the [GPU thread] is still busy rendering the last frame, [calling thread] will block
    ///   until a buffer swap can be done.
    ///   - If the [GPU thread] is alrady done then this function will return immediatly, and the
    ///   [GPU thread] will begin rendering the back draw buffer.
    ///   Important: During the execution of this function, nothing should be be queued to the render
    ///   core, it's not thread safe to do so. However, if the GPU thread was already done, then this
    ///   function will return very quickly, allowing another frame to start processing. It makes no
    ///   sense to start working on a frame 2 out anyway.
    /// </summary>
    public void SwapBuffers()
    {
      // Swap the buffer now
      // TODO: Look into reducing GC load.
      _backDrawCallBuffer = new ConcurrentQueue<GLDrawCall>();
      // Wait for the swap lock, blocking [calling thread] if [gpu thread] is still working
      lock (_swapLock)
      {
        // GPU thread is done, and waiting for a Monitor pule to the lock to start
        _frontBuffer = _finalizedBuffer;
        Monitor.Pulse(_swapLock);
      }
    }

    /// <summary>
    ///   Shuts down the GPU rendering thread gracefully (flushing buffers). [this thread] will block
    ///   until the [gpu thraed] finishes it's flush.
    /// </summary>
    public void ShutDown()
    {
      _shutdown = true;
      _finalizedBuffer = new GLDrawCall[0];
      SwapBuffers();
    }

    /// <summary>
    ///   Adds a static (doesn't change from frame-to-frame) draw call to the render queue.
    /// </summary>
    /// <param name="drawCall"></param>
    internal void AddStaticDrawCall(GLDrawCall drawCall)
    {
      _staticDrawCalls.Enqueue(drawCall);
      // I need to pull the view matrix out if I want to do this, and bind it to every draw
      // call reguardless.
      throw new NotImplementedException();
    }

    /// <summary>
    ///   Adds a dynamic draw call that will only execute once, on the next frame.
    /// </summary>
    /// <param name="drawCall"></param>
    internal void AddSingleFrameDrawCall(GLDrawCall drawCall)
    {
      _backDrawCallBuffer.Enqueue(drawCall);
    }


    // DEBUG
    private readonly Stopwatch _stopWatch = Stopwatch.StartNew();
    private readonly UWorld _world = new UWorld();
    private ACamera _camera;
    private AStaticMesh _staticMesh;
    private ResourceBundle _resourceBundle;

    private void DoGpuThreadWork()
    {
      lock (_swapLock)
      {
        _context = new GraphicsContext(GraphicsMode.Default, _windowInfo);
        _context.MakeCurrent(_windowInfo);


        // DEBUG
        _camera = new ACamera { AspectRatio = (16.0f / 9.0f) };
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
          Transform = { Scale = new Vector3(0.5f), Position = new Vector3(0, -5, -30) }
        };

        var level = new ULevel();
        _world.AddLevel(level);
        level.AddActor(_staticMesh);
        level.AddActor(_camera);

        // DEBUG


        while (!_shutdown)
        {
          // Render everything out.
          GL.ClearColor(ClearColor);
          GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
          if (_frontBuffer != null)
          {
            for (var i = 0; i < _frontBuffer.Length; i++)
            {
              // The custom action has already been built for the [gpu thread] to execute, meaning we
              // don't need to do any extra work in the [gpu thread], beautiful!
              _frontBuffer[i].Draw();
            }
          }
          _context.SwapBuffers();
          // Wait for a new front buffer to be loaded.

          // DEBUG
          _staticMesh.Transform.Rotation = Quaternion.FromEulerAngles(0,
            (float)_stopWatch.Elapsed.TotalMilliseconds / 2000.0f,
            (float)-Math.PI / 2.0f);
          _world.EnqueueDrawCalls(this);
          // DEBUG


          Monitor.Wait(_swapLock);
        }
      }
    }
  }
}