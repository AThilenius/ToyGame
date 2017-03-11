using System.Collections.Generic;

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

    public void DrawAll(ACamera camera)
    {
      foreach (var actor in _actors)
      {
        DrawActor(actor, camera);
      }
    }

    public void DrawActor(AActor actor, ACamera camera, bool recursive = true)
    {
      if (actor is IRenderable)
      {
        (actor as IRenderable).Render(camera);
      }
      if (recursive)
      {
        foreach (var child in actor.Children)
        {
          DrawActor(child, camera, recursive);
        }
      }
    }
  }
}