using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToyGame
{
  public class UWorld
  {

    public ACamera MainCamera;
    public IReadOnlyCollection<ULevel> Levels { get { return levels.AsReadOnly(); } }

    List<ULevel> levels = new List<ULevel>();

    public void AddLevel(ULevel level)
    {
      levels.Add(level);
      level.World = this;
    }

    public void Render()
    {
      if (MainCamera == null)
      {
        // Just use the first camera we can find. At some point this will all
        // be gutted for a 'Pawn' based control system.
        foreach (ULevel level in levels)
        {
          List<ACamera> cameras = level.GetInstancesOf<ACamera>();
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
      levels.ForEach(level => level.DrawAll(MainCamera));
    }

  }
}
