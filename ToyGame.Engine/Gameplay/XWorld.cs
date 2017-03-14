using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Platform;
using ToyGame.Rendering;

namespace ToyGame.Gameplay
{
  /// <summary>
  ///   This is a dynamic object, created to host a XLevel (or several levels for streaming).
  ///   Worlds are not serialized, and only one world will exist in a game, but several exist
  ///   in the editor to host different viewports with different contents.
  /// </summary>
  public class XWorld
  {
    #region Fields / Properties

    public readonly RenderContext RenderContext;
    public ACamera MainCamera;
    public readonly IWindowInfo WindowInfo;
    private readonly List<XLevel> _levels = new List<XLevel>();

    #endregion

    public XWorld(IWindowInfo windowInfo)
    {
      WindowInfo = windowInfo;
      RenderContext = new RenderContext(windowInfo);
    }

    public void AddLevel(XLevel level)
    {
      _levels.Add(level);
      level.World = this;
    }

    public void EnqueueDrawCalls(RenderContext renderContext)
    {
      if (MainCamera == null)
      {
        // Just use the first camera we can find. At some point this will all
        // be gutted for a 'Pawn' based control system.
        MainCamera = _levels.SelectMany(level => level.GetInstancesOf<ACamera>()).FirstOrDefault();
        if (MainCamera == null)
        {
          Console.WriteLine(@"No camera was added to any XLevel. Skipping rendering.");
          return;
        }
      }
      MainCamera.PreRender();
      _levels.ForEach(level => level.EnqueueDrawCalls(renderContext, MainCamera));
    }
  }
}