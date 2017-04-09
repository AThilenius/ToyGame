using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenTK.Graphics;
using OpenTK.Platform;
using ToyGame.Utilities;

namespace ToyGame.Rendering
{
  public class RenderContext
  {
    #region Fields / Properties

    public const string GpuThreadName = "OpenGL Render Thread";
    public static RenderContext Active { get; private set; }
    public IWindowInfo MainWindowInfo;
    public GraphicsContext GLGraphicsContext;
    // TODO: This event shouldn't exist after Dispatcher is finished
    public event EventHandler OnSyncronize;
    public event EventHandler OnSyncronizeGpu;
    private readonly Thread _gpuThread;
    private readonly object _swapLock = new object();
    private readonly object _backBufferLock = new object();
    private ConcurrentQueue<Action> _backResourceLoadBuffer = new ConcurrentQueue<Action>();
    private Action[] _frontResourceLoadBuffer;
    private KeyValuePair<IRenderTarget, RenderPipeline[]>[] _frontPipelineRunsBuffer;
    private bool _shutdown;

    private Dictionary<IRenderTarget, List<RenderPipeline>> _backPipelineRunsBuffer =
      new Dictionary<IRenderTarget, List<RenderPipeline>>();

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
      _frontPipelineRunsBuffer = new KeyValuePair<IRenderTarget, RenderPipeline[]>[0];
      Synchronize();
    }

    public void Synchronize()
    {
      var newFrontResourcesBuffer = new Action[_backResourceLoadBuffer.Count];
      _backResourceLoadBuffer.CopyTo(newFrontResourcesBuffer, 0);
      _backResourceLoadBuffer = new ConcurrentQueue<Action>();
      KeyValuePair<IRenderTarget, RenderPipeline[]>[] newFrontDrawCallBatchBuffer;
      lock (_backBufferLock)
      {
        newFrontDrawCallBatchBuffer =
          _backPipelineRunsBuffer.ToArray()
            .Select(kvp => new KeyValuePair<IRenderTarget, RenderPipeline[]>(kvp.Key, kvp.Value.ToArray()))
            .ToArray();
        _backPipelineRunsBuffer = new Dictionary<IRenderTarget, List<RenderPipeline>>();
      }
      lock (_swapLock)
      {
        _frontResourceLoadBuffer = newFrontResourcesBuffer;
        _frontPipelineRunsBuffer = newFrontDrawCallBatchBuffer;
        // Swap all buffers in all enqueued GLDrawCallBuffers
        foreach (var pipeline in _frontPipelineRunsBuffer.SelectMany(kvp => kvp.Value)) pipeline.SwapDrawCallBuffers();
        OnSyncronize?.Invoke(this, EventArgs.Empty);
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

    internal void AddPipelineRun(RenderPipeline pipeline, IRenderTarget renderTarget)
    {
      lock (_backBufferLock)
      {
        List<RenderPipeline> runs = null;
        if (_backPipelineRunsBuffer.TryGetValue(renderTarget, out runs)) runs.Add(pipeline);
        else _backPipelineRunsBuffer.Add(renderTarget, new List<RenderPipeline>(new[] {pipeline}));
      }
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
              // Bind the IRenderTarget
              pipelineRun.Key.Bind();
              // Render all piplines
              // ReSharper disable once ForCanBeConvertedToForeach
              for (var j = 0; j < pipelineRun.Value.Length; j++)
              {
                pipelineRun.Value[j].RenderImmediate();
              }
              // Swap the buffers for the IRenderTarget
              pipelineRun.Key.FinalizeRender();
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