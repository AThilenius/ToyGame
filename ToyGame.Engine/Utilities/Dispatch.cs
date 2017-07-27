using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ToyGame.Utilities
{
  public enum DispatchPhase
  {
    Update = 0,
    DrawCallEnqueue = 1,
    Syncronized = 2,
    Finalize = 3
  }

  public enum DispatchOrder
  {
    Early = 0,
    Normal = 1,
    Late = 2
  }

  public static class DynamicDispatch
  {
    #region Fields / Properties

    private static readonly List<Action>[][] SingleShotDispatches = ArrayHelpers.Init2D(4, 3, () => new List<Action>());
    private static readonly List<Action>[][] PerFrameDispatches = ArrayHelpers.Init2D(4, 3, () => new List<Action>());

    #endregion

    public static void AddDispatch(DispatchPhase phase, bool perFrame, Action action)
      => AddDispatch(phase, perFrame, DispatchOrder.Normal, action);

    public static void AddDispatch(DispatchPhase phase, bool perFrame, DispatchOrder order, Action action)
    {
      (perFrame ? SingleShotDispatches : PerFrameDispatches)[(int) phase][(int) order].Add(action);
    }

    public static void Dispatch(DispatchPhase phase)
    {
      for (var i = 0; i < 3; i++)
      {
        var singleShot = SingleShotDispatches[(int) phase][i];
        // ReSharper disable once ForCanBeConvertedToForeach
        for (var j = 0; j < singleShot.Count; j++) singleShot[j]();
        singleShot.Clear();
        var perFrame = PerFrameDispatches[(int) phase][i];
        // ReSharper disable once ForCanBeConvertedToForeach
        for (var j = 0; j < perFrame.Count; j++) perFrame[j]();
      }
    }
  }

  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
  public class StaticDispatch : Attribute
  {
    #region Fields / Properties

    /// <summary>
    ///   The order (within the phase) to dispatch this action on.
    /// </summary>
    public DispatchOrder Order = DispatchOrder.Normal;

    /// <summary>
    ///   If this action should be dispatch only once (default) or once per frame.
    /// </summary>
    public bool PerFrame;

    /// <summary>
    ///   The phase to dispatch this action on.
    /// </summary>
    public DispatchPhase Phase;

    #endregion

    public StaticDispatch(DispatchPhase phase)
    {
      Phase = phase;
    }

    public static void AddAssembly(Assembly assembly)
    {
      foreach (var type in assembly.GetTypes())
      {
        foreach (var method in type.GetMethods())
        {
          foreach (var staticDispatch in method
            .GetCustomAttributes(typeof (StaticDispatch), true)
            .Select(a => (StaticDispatch) a))
          {
            var method1 = method;
            if (!method1.IsStatic)
            {
              Console.WriteLine(@"StaticDispatch attribute cannot be applied to non-static methods. Method: " +
                                method1.Name + @" in " + type.FullName);
              continue;
            }
            DynamicDispatch.AddDispatch(staticDispatch.Phase, staticDispatch.PerFrame, staticDispatch.Order,
              () => method1.Invoke(null, null));
          }
        }
      }
    }
  }
}