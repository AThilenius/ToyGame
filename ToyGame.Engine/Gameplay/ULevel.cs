using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToyGame
{
  public class ULevel
  {

    public IReadOnlyCollection<AActor> Actors { get { return actors.AsReadOnly(); } }
    public UWorld World { get; internal set; }

    private List<AActor> actors = new List<AActor>();

    public void AddActor (AActor actor)
    {
      actors.Add(actor);
      actor.Level = this;
    }

    public List<T> GetInstancesOf<T>() where T : AActor
    {
      List<T> instances = new List<T>();
      foreach (AActor actor in actors)
      {
        if (typeof(T).IsAssignableFrom(actor.GetType()))
        {
          instances.Add((T) actor);
        }
        instances.AddRange(actor.GetInstancesOf<T>());
      }
      return instances;
    }

    public void DrawAll(ACamera camera)
    {
      foreach (AActor actor in actors)
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
        foreach (AActor child in actor.Children)
        {
          DrawActor(child, camera, recursive);
        }
      }
    }

  }
}
