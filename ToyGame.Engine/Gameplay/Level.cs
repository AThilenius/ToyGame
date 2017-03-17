using System;
using System.Collections.Generic;
using ProtoBuf;
using ToyGame.Rendering;
using ToyGame.Rendering.OpenGL;

namespace ToyGame.Gameplay
{
  /// <summary>
  ///   This is the top level class for editing in the editor. The World is dynamicly
  ///   created just to host levels and in a game only one of them actually exists.
  /// </summary>
  [ProtoContract]
  public class Level
  {
    #region Fields / Properties

    public const string XLevelFileExtension = ".xlevel";

    public IReadOnlyCollection<AActor> Actors => _actors.AsReadOnly();
    public World World { get; internal set; }
    private readonly List<AActor> _actors = new List<AActor>();
    [ProtoMember(1)] public Guid Guid = Guid.NewGuid();

    [ProtoMember(2)] public string Name = "Unnamed";

    [ProtoMember(3)]
    private AActor[] ChildActors
    {
      get { return _actors.ToArray(); }
      set
      {
        foreach (var actor in value)
        {
          AddActor(actor);
        }
      }
    }

    #endregion

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
        var item = actor as T;
        if (item != null)
        {
          instances.Add(item);
        }
        instances.AddRange(actor.GetInstancesOf<T>());
      }
      return instances;
    }

    internal void EnqueueDrawCalls(GLDrawCallBatch drawCallBatch, AActor specificActor = null)
    {
      if (specificActor == null)
      {
        _actors.ForEach(a => EnqueueDrawCalls(drawCallBatch, a));
        return;
      }
      (specificActor as IRenderable)?.EnqueueDrawCalls(drawCallBatch);
      foreach (var child in specificActor.Children)
      {
        EnqueueDrawCalls(drawCallBatch, child);
      }
    }
  }
}