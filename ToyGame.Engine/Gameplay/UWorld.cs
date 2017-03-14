using System;
using System.Collections.Generic;
using System.Linq;
using ToyGame.Rendering;

namespace ToyGame.Gameplay
{
  public class UWorld
  {
    #region Fields / Properties

    public IReadOnlyCollection<ULevel> Levels => _levels.AsReadOnly();
    public ACamera MainCamera;
    private readonly List<ULevel> _levels = new List<ULevel>();

    #endregion

    public void AddLevel(ULevel level)
    {
      _levels.Add(level);
      level.World = this;
    }

    public void EnqueueDrawCalls(RenderCore renderCore)
    {
      if (MainCamera == null)
      {
        // Just use the first camera we can find. At some point this will all
        // be gutted for a 'Pawn' based control system.
        MainCamera = _levels.SelectMany(level => level.GetInstancesOf<ACamera>()).FirstOrDefault();
        if (MainCamera == null)
        {
          Console.WriteLine(@"No camera was added to any level. Skipping rendering.");
          return;
        }
      }
      MainCamera.PreRender();
      _levels.ForEach(level => level.EnqueueDrawCalls(renderCore, MainCamera));
    }
  }
}