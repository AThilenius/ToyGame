using System;
using System.Collections.Concurrent;
using System.Threading;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using OpenTK.Platform;
using ToyGame.Rendering.OpenGL;

namespace ToyGame.Rendering
{
  public class RenderContext
  {
    #region Fields / Properties

    public const string GpuThreadName = "OpenGL Render Thread";
    private readonly ConcurrentQueue<GLDrawCall> _staticDrawCalls = new ConcurrentQueue<GLDrawCall>();
    private readonly IWindowInfo _windowInfo;
    private readonly Thread _gpuThread;
    private IGraphicsContext _context;
    private ConcurrentQueue<Action> _backPreRenderBuffer = new ConcurrentQueue<Action>();
    private ConcurrentQueue<Action> _backResourceLoadBuffer = new ConcurrentQueue<Action>();
    private ConcurrentQueue<GLDrawCall> _backDrawCallBuffer = new ConcurrentQueue<GLDrawCall>();
    private volatile object _swapLock = new object();
    private volatile Action[] _frontPreRenderBuffer;
    private volatile Action[] _frontResourceLoadBuffer;
    private volatile GLDrawCall[] _frontDrawBuffer;
    private volatile GLDrawCall[] _finalizedDrawBuffer;
    private bool _shutdown;

    #endregion

    public RenderContext(IWindowInfo windowInfo)
    {
      _windowInfo = windowInfo;
      _gpuThread = new Thread(DoGpuThreadWork) {Name = GpuThreadName};
      _gpuThread.Start();
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
      _finalizedDrawBuffer = new GLDrawCall[_backDrawCallBuffer.Count + _staticDrawCalls.Count];
      _backDrawCallBuffer.CopyTo(_finalizedDrawBuffer, 0);
      _staticDrawCalls.CopyTo(_finalizedDrawBuffer, _backDrawCallBuffer.Count);
      Array.Sort(_finalizedDrawBuffer);
      // Generate drawing actions for all draw calls off-GPU thread.
      for (var i = 0; i < _finalizedDrawBuffer.Length; i++)
      {
        _finalizedDrawBuffer[i].GenerateBindAction(i > 0 ? _finalizedDrawBuffer[i - 1] : null);
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
        // Copy over PreRender calls. Note this 'could' be done with pointer swapping like the draw calls
        _frontPreRenderBuffer = new Action[_backPreRenderBuffer.Count];
        _backPreRenderBuffer.CopyTo(_frontPreRenderBuffer, 0);
        _backPreRenderBuffer = new ConcurrentQueue<Action>();
        _frontResourceLoadBuffer = new Action[_backResourceLoadBuffer.Count];
        _backResourceLoadBuffer.CopyTo(_frontResourceLoadBuffer, 0);
        _backResourceLoadBuffer = new ConcurrentQueue<Action>();
        // GPU thread is done, and waiting for a Monitor pule to the lock to start
        _frontDrawBuffer = _finalizedDrawBuffer;
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
      _finalizedDrawBuffer = new GLDrawCall[0];
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

    internal void AddPreRenderAction(Action action)
    {
      if (Thread.CurrentThread == _gpuThread)
      {
        // This was called recursivly by another pre-render action, just execute it directly.
        action();
      }
      else
      {
        _backPreRenderBuffer.Enqueue(action);
      }
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

    private void DoGpuThreadWork()
    {
      lock (_swapLock)
      {
        _context = new GraphicsContext(GraphicsMode.Default, _windowInfo);
        _context.MakeCurrent(_windowInfo);
        while (!_shutdown)
        {
          // For now, just load ALL resources at once. In the future, I'll throttle this to only use
          // the remaining draw time (16.7ms - drawTime) to buffer resources.
          if (_frontResourceLoadBuffer != null)
          {
            for (var i = 0; i < _frontResourceLoadBuffer.Length; i++)
            {
              _frontResourceLoadBuffer[i]();
              {
                var error = GL.GetError();
                if (error != ErrorCode.NoError)
                  throw new Exception(error.ToString());
              }
            }
          }
          // Pre-Render calls (probably global Uniform buffering)
          if (_frontPreRenderBuffer != null)
          {
            for (var i = 0; i < _frontPreRenderBuffer.Length; i++)
            {
              _frontPreRenderBuffer[i]();
              {
                var error = GL.GetError();
                if (error != ErrorCode.NoError)
                  throw new Exception(error.ToString());
              }
            }
          }
          if (_frontDrawBuffer != null)
          {
            for (var i = 0; i < _frontDrawBuffer.Length; i++)
            {
              // The custom action has already been built for the [gpu thread] to execute, meaning we
              // don't need to do any extra work in the [gpu thread], beautiful!
              _frontDrawBuffer[i].Draw();
              {
                var error = GL.GetError();
                if (error != ErrorCode.NoError)
                  throw new Exception(error.ToString());
              }
            }
          }
          _context.SwapBuffers();
          {
            var error = GL.GetError();
            if (error != ErrorCode.NoError)
              throw new Exception(error.ToString());
          }
          // Wait for a new front buffer to be loaded.
          Monitor.Wait(_swapLock);
        }
      }
    }
  }
}