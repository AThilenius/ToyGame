using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ToyGame.Utilities
{
  /// <summary>
  /// Tracks statistics for a Window, things like framerate (low pass filtered), as well as max and min draw times.
  /// </summary>
  public class WindowStatistics
  {
    #region Fields / Properties

    public float AverageFramerate;
    public float MaxFrameTimeMs;
    public float AverageFrameTimeMs;
    public int FrameCount;
    public string DebugString => string.Format("Avg Framerate: {0:000.0}", AverageFramerate) +
      string.Format("Avg Frametime: {0:000.0} ms, ", AverageFrameTimeMs) +
      string.Format("Max Frametime: {0:000.0} ms", MaxFrameTimeMs);
    private readonly Stopwatch _timer = Stopwatch.StartNew();
    private float _lastTimeMs;

    #endregion

    public void Update()
    {
      FrameCount++;
      var elapsedMs = (float) _timer.Elapsed.TotalMilliseconds;
      var lastFrameTime = elapsedMs - _lastTimeMs;
      MaxFrameTimeMs = Math.Max(MaxFrameTimeMs, lastFrameTime);
      AverageFrameTimeMs = elapsedMs/FrameCount;
      AverageFramerate = 1000.0f/AverageFrameTimeMs;
      _lastTimeMs = elapsedMs;
    }
  }
}