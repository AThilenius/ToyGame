using System;
using System.Collections.Generic;
using System.Linq;
using ToyGame.Rendering;

namespace ToyGame.Gameplay
{
  public class UWorld
  {
    private readonly List<ULevel> _levels = new List<ULevel>();
    public ACamera MainCamera;

    public IReadOnlyCollection<ULevel> Levels => _levels.AsReadOnly();

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
        foreach (var level in _levels)
        {
          var cameras = level.GetInstancesOf<ACamera>();
          if (cameras.Count() > 0)
          {
            MainCamera = cameras[0];
            break;
          }
        }
        if (MainCamera == null)
        {
          Console.WriteLine("No camera was added to any level. Skipping rendering.");
          return;
        }
      }
      MainCamera.PreRender();
      _levels.ForEach(level => level.EnqueueDrawCalls(renderCore, MainCamera));
    }
  }
}