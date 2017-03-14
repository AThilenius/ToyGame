using System;
using System.Collections.Generic;
using ProtoBuf;
using ToyGame.Rendering;

namespace ToyGame.Gameplay
{
  /// <summary>
  ///   This is the top level class for editing in the editor. The XWorld is dynamicly
  ///   created just to host levels and in a game only one of them actually exists.
  /// </summary>
  [ProtoContract]
  public class XLevel
  {
    #region Fields / Properties

    public const string XLevelFileExtension = ".xlevel";

    public IReadOnlyCollection<AActor> Actors => _actors.AsReadOnly();
    public XWorld World { get; internal set; }
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
        if (typeof (T).IsAssignableFrom(actor.GetType()))
        {
          instances.Add((T) actor);
        }
        instances.AddRange(actor.GetInstancesOf<T>());
      }
      return instances;
    }

    public void EnqueueDrawCalls(RenderContext renderContext, ACamera camera)
    {
      foreach (var actor in _actors)
      {
        EnqueueDrawCalls(renderContext, actor, camera);
      }
    }

    public void EnqueueDrawCalls(RenderContext renderContext, AActor actor, ACamera camera)
    {
      (actor as IRenderable)?.EnqueueDrawCalls(renderContext, camera);
      foreach (var child in actor.Children)
      {
        EnqueueDrawCalls(renderContext, child, camera);
      }
    }
  }
}