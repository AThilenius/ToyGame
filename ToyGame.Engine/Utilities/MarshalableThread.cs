using System;
using System.Collections.Concurrent;
using System.Threading;

namespace ToyGame.Utilities
{
  /// <summary>
  ///   Wraps a thread in an action-pump, allowing actions to be marshaled between
  ///   threads. This class works in concert with and EventStream for cross-thread
  ///   messages.
  /// </summary>
  public class MarshalableThread
  {
    #region Fields / Properties

    public static readonly MarshalableThread FileIoThread = new MarshalableThread();
    public static readonly MarshalableThread GpuInterfaceThread = new MarshalableThread();
    protected readonly BlockingCollection<Action> _actions = new BlockingCollection<Action>();

    #endregion

    public MarshalableThread()
    {
      var thread = new Thread(() =>
      {
        foreach (var action in _actions.GetConsumingEnumerable())
        {
          action();
        }
      });
      thread.Start();
    }

    /// <summary>
    ///   Marshals a single action to the thread at the end of queue.
    /// </summary>
    /// <param name="action"></param>
    public void Marshal(Action action)
    {
      _actions.Add(action);
    }

    /// <summary>
    ///   Shuts down the MarshalableThread gracefully (waiting for all pending actions to finish)
    /// </summary>
    public void Shutdown()
    {
      _actions.CompleteAdding();
    }
  }
}