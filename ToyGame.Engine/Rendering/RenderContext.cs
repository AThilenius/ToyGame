using System;
using System.Collections.Concurrent;
using System.Threading;
using OpenTK.Graphics;
using OpenTK.Platform;
using ToyGame.Gameplay;
using ToyGame.Utilities;

namespace ToyGame.Rendering
{
  public class RenderContext
  {
    #region Types

    private struct PiplineRun
    {
      #region Fields / Properties

      public RenderPipeline Pipeline;
      public IRenderTarget RenderTarget;
      public ACamera Camera;

      #endregion
    }

    #endregion

    #region Fields / Properties

    public const string GpuThreadName = "OpenGL Render Thread";
    public static RenderContext Active { get; private set; }
    public IWindowInfo MainWindowInfo;
    public GraphicsContext GLGraphicsContext;
    private readonly Thread _gpuThread;
    private readonly object _swapLock = new object();
    private ConcurrentQueue<Action> _backResourceLoadBuffer = new ConcurrentQueue<Action>();
    private ConcurrentQueue<PiplineRun> _backPipelineRunsBuffer = new ConcurrentQueue<PiplineRun>();
    private Action[] _frontResourceLoadBuffer;
    private PiplineRun[] _frontPipelineRunsBuffer;
    private bool _shutdown;

    #endregion

    internal RenderContext(IWindowInfo mainWindowInfo)
    {
      MainWindowInfo = mainWindowInfo;
      _gpuThread = new Thread(DoGpuThreadWork) {Name = GpuThreadName};
      _gpuThread.Start();
    }

    /// <summary>
    ///   Shuts down the GPU rendering thread gracefully (flushing buffers). [this thread] will block
    ///   until the [gpu thraed] finishes it's flush.
    /// </summary>
    public void ShutDown()
    {
      _shutdown = true;
      _frontPipelineRunsBuffer = new PiplineRun[0];
      Synchronize();
    }

    public void Synchronize()
    {
      var newFrontResourcesBuffer = new Action[_backResourceLoadBuffer.Count];
      _backResourceLoadBuffer.CopyTo(newFrontResourcesBuffer, 0);
      _backResourceLoadBuffer = new ConcurrentQueue<Action>();
      var newFrontDrawCallBatchBuffer = new PiplineRun[_backPipelineRunsBuffer.Count];
      _backPipelineRunsBuffer.CopyTo(newFrontDrawCallBatchBuffer, 0);
      _backPipelineRunsBuffer = new ConcurrentQueue<PiplineRun>();
      lock (_swapLock)
      {
        _frontResourceLoadBuffer = newFrontResourcesBuffer;
        _frontPipelineRunsBuffer = newFrontDrawCallBatchBuffer;
        // Swap all buffers in all enqueued GLDrawCallBuffers
        for (var i = 0; i < _frontPipelineRunsBuffer.Length; i++)
        {
          _frontPipelineRunsBuffer[i].Pipeline.SwapDrawCallBuffers();
        }
        Monitor.Pulse(_swapLock);
      }
    }

    internal static void Initialize(IWindowInfo mainWindowInfo)
    {
      Active = new RenderContext(mainWindowInfo);
    }

    internal void Shutdown()
    {
      _shutdown = true;
      lock (_swapLock) Monitor.Pulse(_swapLock);
    }

    internal void AddResourceLoadAction(Action action)
    {
      if (Thread.CurrentThread == _gpuThread)
      {
        // This was called recursivly by another loading resource, just execute it directly.
        action();
      }
      else
      {
        _backResourceLoadBuffer.Enqueue(action);
      }
    }

    internal void AddPipelineRun(RenderPipeline pipeline, IRenderTarget renderTarget, ACamera camera)
    {
      _backPipelineRunsBuffer.Enqueue(new PiplineRun {Pipeline = pipeline, RenderTarget = renderTarget, Camera = camera});
    }

    private void DoGpuThreadWork()
    {
      lock (_swapLock)
      {
        GLGraphicsContext = new GraphicsContext(GraphicsMode.Default, MainWindowInfo, 4, 2, GraphicsContextFlags.Debug);
        GLGraphicsContext.LoadAll();
        GLGraphicsContext.MakeCurrent(MainWindowInfo);
        while (!_shutdown)
        {
          // For now, just load ALL resources at once. In the future, I'll throttle this to only use
          // the remaining draw time (16.7ms - drawTime) to buffer resources.
          if (_frontResourceLoadBuffer != null)
          {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < _frontResourceLoadBuffer.Length; i++)
            {
              _frontResourceLoadBuffer[i]();
              DebugUtils.GLErrorCheck();
            }
          }
          if (_frontPipelineRunsBuffer != null)
          {
            // ReSharper disable once ForCanBeConvertedToForeach
            for (var i = 0; i < _frontPipelineRunsBuffer.Length; i++)
            {
              var pipelineRun = _frontPipelineRunsBuffer[i];
              pipelineRun.Pipeline.RenderImmediate(pipelineRun.Camera, pipelineRun.RenderTarget);
              DebugUtils.GLErrorCheck();
            }
          }
          DebugUtils.GLErrorCheck();
          // Wait for a new front buffer to be loaded.
          Monitor.Wait(_swapLock);
        }
      }
    }
  }
}