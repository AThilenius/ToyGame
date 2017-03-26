using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Platform;
using ProtoBuf.Meta;
using ToyGame.Gameplay;
using ToyGame.Rendering;
using ToyGame.Resources;
using ToyGame.Serialization.Surrogates;

namespace ToyGame
{
  public class ToyEngine
  {
    #region Fields / Properties

    public IReadOnlyCollection<Window> Windows => _windows.AsReadOnly();
    public IReadOnlyCollection<World> Worlds => _worlds.AsReadOnly();
    public int TargetRenderFramerate = 60;
    private readonly List<Window> _windows = new List<Window>();
    private readonly List<World> _worlds = new List<World>();
    private bool _firstWindow = true;

    #endregion

    public ToyEngine()
    {
      // Register ProtoBuf.Net Surrogates
      var model = RuntimeTypeModel.Default;
      model.Add(typeof (Vector2Surrogate), true);
      model.Add(typeof (Vector2), false).SetSurrogate(typeof (Vector2Surrogate));
      model.Add(typeof (Vector3Surrogate), true);
      model.Add(typeof (Vector3), false).SetSurrogate(typeof (Vector3Surrogate));
      model.Add(typeof (Color4Surrogate), true);
      model.Add(typeof (Color4), false).SetSurrogate(typeof (Color4Surrogate));
      model.Add(typeof (ResourceSurrogate), true);
      model.Add(typeof (Resource), false).SetSurrogate(typeof (ResourceSurrogate));
    }

    public Window CreateWindow(string title, int width = 1680, int height = 1050, World world = null)
    {
      var window = new Window(title, width, height);
      window.Closing += (sender, args) => _windows.Remove(window);
      _windows.Add(window);
      if (!_worlds.Contains(window.World)) _worlds.Add(window.World);
      return window;
    }

    public void Run()
    {
      var timer = Stopwatch.StartNew();
      var lastFrameFinishTime = 0.0;
      while (_windows.Count > 0)
      {
        Time.UpdateTimes();
        // Update and Enqueue all render calls for all windows
        foreach (var window in _windows.ToArray()) window.Update();
        _windows.ForEach(window => window.Render());
        // Flush the render queue for the last frame and start rendering the next one
        RenderContext.Active.Synchronize();
        // Slow down framerate to try and match target framerate.
        var currentTime = timer.Elapsed.TotalSeconds;
        var deltaTime = currentTime - lastFrameFinishTime;
        var targetRenderTime = 1.0/TargetRenderFramerate;
        if (deltaTime < targetRenderTime)
        {
          Thread.Sleep((int) ((targetRenderTime - deltaTime)*1000));
        }
        lastFrameFinishTime = timer.Elapsed.TotalSeconds;
      }
      RenderContext.Active.Shutdown();
    }
  }
}