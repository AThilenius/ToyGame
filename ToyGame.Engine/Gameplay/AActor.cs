using System.Collections.Generic;

namespace ToyGame.Gameplay
{
  public class AActor
  {
    #region Fields / Properties

    public IReadOnlyCollection<AActor> Children
    {
      get { return _children.AsReadOnly(); }
    }

    public UTransform Transform;

    public AActor Parent
    {
      get { return _parent; }
      set { SetParent(value); }
    }

    public ULevel Level
    {
      get { return _level; }
      set
      {
        _level = value;
        _children.ForEach(c => c.Level = value);
      }
    }

    private readonly List<AActor> _children = new List<AActor>();
    private ULevel _level;
    private AActor _parent;

    #endregion

    public AActor()
    {
      Transform = new UTransform(this);
    }

    public List<T> GetInstancesOf<T>(bool recursive = true) where T : AActor
    {
      var instances = new List<T>();
      foreach (var child in _children)
      {
        if (typeof (T).IsAssignableFrom(child.GetType()))
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
      if (_parent != null)
      {
        _parent._children.Remove(actor);
      }
      _parent = actor;
      Level = _parent.Level;
      if (actor != null)
      {
        _parent._children.Add(this);
      }
    }
  }
}