using System.Diagnostics;

namespace ToyGame.Gameplay
{
  public static class Time
  {
    #region Fields / Properties

    /// <summary>
    ///   How long it's been sense the last tick in game time (accounts for TimeScale)
    /// </summary>
    public static float DeltaTime => (float) _deltaTime;

    /// <summary>
    ///   How long the game has been running in real (human) time.
    /// </summary>
    public static float RealTime => (float) _realTime;

    /// <summary>
    ///   How long the game has been running in game time (accounts for TimeScale)
    /// </summary>
    public static float GameTime => (float) _gameTime;

    /// <summary>
    ///   How fast in-game times advances. Can be used for 'slow-mo' effects.
    /// </summary>
    public static float TimeScale { get; set; } = 1.0f;

    /// <summary>
    ///  Returns the number of frames thus far.
    /// </summary>
    public static int FrameCount { get; private set; }

    private static readonly Stopwatch Timer = Stopwatch.StartNew();
    private static double _realTime;
    private static double _gameTime;
    private static double _deltaTime;

    #endregion

    internal static void UpdateTimes()
    {
      // Advance delta time forward by [current frame time] - [last frame time] * scale
      var newTime = (float) Timer.Elapsed.TotalSeconds;
      _deltaTime = (newTime - _realTime)*TimeScale;
      _gameTime += _deltaTime;
      _realTime = newTime;
      FrameCount++;
    }
  }
}