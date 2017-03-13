using System.Collections.Generic;
using ToyGame.Rendering;

namespace ToyGame.Gameplay
{
  public class ULevel
  {
    private readonly List<AActor> _actors = new List<AActor>();

    public IReadOnlyCollection<AActor> Actors => _actors.AsReadOnly();

    public UWorld World { get; internal set; }

    public void AddActor(AActor actor)
    {
      _actors.Add(actor);
      actor.Level = this;
    }

    public List<T> GetInstancesOf<T>() where T : AActor
    {
      var instances = new List<T>();
      foreach (var actor in _actors)
      {
        if (typeof (T).IsAssignableFrom(actor.GetType()))
        {
          instances.Add((T) actor);
        }
        instances.AddRange(actor.GetInstancesOf<T>());
      }
      return instances;
    }

    public void EnqueueDrawCalls(RenderCore renderCore, ACamera camera)
    {
      foreach (var actor in _actors)
      {
        EnqueueDrawCalls(renderCore, actor, camera);
      }
    }

    public void EnqueueDrawCalls(RenderCore renderCore, AActor actor, ACamera camera)
    {
      (actor as IRenderable)?.EnqueueDrawCalls(renderCore, camera);
      foreach (var child in actor.Children)
      {
        EnqueueDrawCalls(renderCore, child, camera);
      }
    }
  }
}