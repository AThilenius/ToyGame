using System;
using System.Collections.Concurrent;

namespace ToyGame.Rendering.OpenGL
{
  /// <summary>
  ///   Hosts a semi-thread safe double buffered draw call list for rendering. Adding to the queue is not thread safe during
  ///   a swap as it's assumed all threads will be waiting on the swap anyway. This can also be used for drawing static
  ///   objects by calling FinalizeFrame and SwapBuffers exactly once.
  /// </summary>
  public class GLDrawCallBatch
  {
    #region Types

    /// <summary>
    ///   Used for determining where in the draw call list a call be be batched. This is useful as draw calls are ordered to be
    ///   most performant, not by insert order.
    /// </summary>
    public enum CallOrder
    {
      Default = 2,
      PreRender = 1,
      Render = 2,
      PostRender = 3
    }

    private struct BatchedDrawCall : IComparable<BatchedDrawCall>
    {
      #region Fields / Properties

      public readonly GLDrawCall DrawCall;
      private readonly int _drawCallOrder;

      #endregion

      public BatchedDrawCall(int drawCallOrder, GLDrawCall drawCall)
      {
        _drawCallOrder = drawCallOrder;
        DrawCall = drawCall;
      }

      public int CompareTo(BatchedDrawCall other)
      {
        var orderDiff = _drawCallOrder.CompareTo(other._drawCallOrder);
        return orderDiff != 0 ? orderDiff : DrawCall.CompareTo(other.DrawCall);
      }
    }

    #endregion

    #region Fields / Properties

    internal GLDrawCall[] FrontBuffer;
    private GLDrawCall[] _finalizedBuffer;
    private ConcurrentQueue<BatchedDrawCall> _backBuffer = new ConcurrentQueue<BatchedDrawCall>();

    #endregion

    /// <summary>
    ///   On the calling thread, copies the draw calls over to a front buffer and sorts them. Once this function returns, the
    ///   DrawCallBuffer can be safely added to a RenderContext. Adding draw calls is not safe during the execution of this
    ///   function.
    /// </summary>
    public void FinalizeFrame()
    {
      // TODO: Look into reducing GC load here.
      // Copy all values to the finalize buffer, and sort it.
      var sortedBatchedDrawCalls = _backBuffer.ToArray();
      Array.Sort(sortedBatchedDrawCalls);
      _finalizedBuffer = new GLDrawCall[sortedBatchedDrawCalls.Length];
      for (var i = 0; i < sortedBatchedDrawCalls.Length; i++)
      {
        _finalizedBuffer[i] = sortedBatchedDrawCalls[i].DrawCall;
      }
      // Generate delta-binding actions for all draw calls.
      for (var i = 0; i < _finalizedBuffer.Length; i++)
      {
        _finalizedBuffer[i].GenerateBindAction(i > 0 ? _finalizedBuffer[i - 1] : null);
      }
      _backBuffer = new ConcurrentQueue<BatchedDrawCall>();
    }

    /// <summary>
    ///   Must be called after a call to FinalizeFrame, does a referance swap of the finalized and front buffers.
    /// </summary>
    internal void SwapDrawCallBuffers()
    {
      FrontBuffer = _finalizedBuffer;
    }

    internal void AddDrawCall(GLDrawCall drawCall, CallOrder callOrder = CallOrder.Default)
    {
      _backBuffer.Enqueue(new BatchedDrawCall((int) callOrder, drawCall));
    }
  }
}