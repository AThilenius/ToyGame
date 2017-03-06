using System;
using System.Collections.Generic;

namespace ToyGame
{
  public class AActor
  {

    public UTransform Transform;
    public AActor Parent { get { return parent; } set { SetParent(value); } }
    public IReadOnlyCollection<AActor> Children { get { return children.AsReadOnly(); } }
    public ULevel Level { get { return level; } set { level = value; children.ForEach(c => c.Level = value); } }

    private List<AActor> children = new List<AActor>();
    private ULevel level;

    private AActor parent;

    public AActor()
    {
      Transform = new UTransform(this);
    }

    public List<T> GetInstancesOf<T>(bool recursive = true) where T : AActor
    {
      List<T> instances = new List<T>();
      foreach (AActor child in children)
      {
        if (typeof(T).IsAssignableFrom(child.GetType()))
        {
          instances.Add((T) child);
        }
        if (recursive)
        {
          instances.AddRange(child.GetInstancesOf<T>());
        }
      }
      return instances;
    }

    private void SetParent(AActor actor)
    {
      if (parent != null)
      {
        parent.children.Remove(actor);
      }
      parent = actor;
      Level = parent.Level;
      if (actor != null)
      {
        parent.children.Add(this);
      }
    }

  }
}
