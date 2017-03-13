using System;
using System.Collections.Concurrent;

namespace ToyGame.Utilities
{
  /// <summary>
  ///   Povides a stream of events in the form of lambda functions with data closures.
  ///   Events can also be marshed to a specific thread assuming the thread has been wrapped
  ///   in an MarshalableThread. This acts as both a thread scheduler and a messaging system
  ///   with efficient cross-thread marshalling/messaging. For raw marshaling, use a
  ///   MarshalableThread directly.
  /// </summary>
  public class EventStream
  {
    #region Types

    private class InvocationTarget
    {
      #region Fields / Properties

      private readonly Action<object> _action;
      private readonly MarshalableThread _marshalableThread;

      #endregion

      public InvocationTarget(Action<object> action, MarshalableThread marshalableThread)
      {
        _action = action;
        _marshalableThread = marshalableThread;
      }

      public void Invoke(Object obj)
      {
        if (_marshalableThread == null)
        {
          _action(obj);
        }
        else
        {
          _marshalableThread.Marshal(() => _action(obj));
        }
      }
    }

    #endregion

    #region Fields / Properties

    private readonly ConcurrentDictionary<Type, ConcurrentQueue<InvocationTarget>> _channels
      = new ConcurrentDictionary<Type, ConcurrentQueue<InvocationTarget>>();

    #endregion

    /// <summary>
    ///   Invokes all observers of typeof(T) on the calling thread, blocking
    ///   untill all observers have completed work.
    /// </summary>
    /// <typeparam name="T">The stream type to push the message to.</typeparam>
    /// <param name="message">The message to send to all observers.</param>
    public void Push<T>(T message) where T : struct
    {
      var targets = _channels.GetOrAdd(typeof (T), t => new ConcurrentQueue<InvocationTarget>());
      // Copy targets to a local array so we don't have to hold the lock the entire time we
      // are invoking all the handlers.
      foreach (var target in targets.ToArray())
      {
        target.Invoke(message);
      }
    }

    /// <summary>
    ///   Registers an observer for a specific stream type. This will be called on the same thread as
    ///   the one that called "Push"
    /// </summary>
    /// <typeparam name="T">The stream type to observe</typeparam>
    /// <param name="action">The action to perform when that type is pushed into the stream</param>
    public void Subscribe<T>(Action<T> action)
    {
      var targets = _channels.GetOrAdd(typeof (T), t => new ConcurrentQueue<InvocationTarget>());
      targets.Enqueue(new InvocationTarget(obj => action((T) obj), null));
    }

    /// <summary>
    ///   Does the same as Subscribe but will marshal the action over to the given MarshalableThread
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="thread"></param>
    /// <param name="action"></param>
    public void SubscribeOn<T>(MarshalableThread thread, Action<T> action)
    {
      var targets = _channels.GetOrAdd(typeof (T), t => new ConcurrentQueue<InvocationTarget>());
      targets.Enqueue(new InvocationTarget(obj => action((T) obj), thread));
    }
  }
}