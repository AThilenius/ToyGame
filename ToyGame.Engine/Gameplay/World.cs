using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Platform;
using ToyGame.Rendering;
using ToyGame.Rendering.OpenGL;

namespace ToyGame.Gameplay
{
  /// <summary>
  ///   This is a dynamic object, created to host a Level (or several levels for streaming).
  ///   Worlds are not serialized, and only one world will exist in a game, but several exist
  ///   in the editor to host different viewports with different contents.
  /// </summary>
  public class World
  {
    #region Fields / Properties

    private readonly List<Level> _levels = new List<Level>();

    #endregion

    public void AddLevel(Level level)
    {
      _levels.Add(level);
      level.World = this;
    }

    internal void EnqueueDrawCalls(GLDrawCallBatch drawCallBatch)
    {
      _levels.ForEach(level => level.EnqueueDrawCalls(drawCallBatch));
    }
  }
}