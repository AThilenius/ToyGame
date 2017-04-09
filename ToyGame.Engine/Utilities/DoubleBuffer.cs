namespace ToyGame.Utilities
{
  /// <summary>
  ///   Represents a Double Buffer used in a rendering pipline (only thread safe when the swap is performed during a hault,
  ///   like the Syncronize for rendering)
  /// </summary>
  public class DoubleBuffer<T>
  {
    #region Fields / Properties

    /// <summary>
    ///   The buffer that can be freely edited.
    /// </summary>
    public T BackBuffer;

    private T _frontBuffer;

    #endregion

    public DoubleBuffer(T frontBuffer, T backBuffer)
    {
      _frontBuffer = frontBuffer;
      BackBuffer = backBuffer;
    }

    /// <summary>
    ///   Swaps buffers and returns the new front buffer for rendering.
    /// </summary>
    /// <returns></returns>
    public T SwapBuffers()
    {
      var swap = _frontBuffer;
      _frontBuffer = BackBuffer;
      BackBuffer = swap;
      return _frontBuffer;
    }

    public T GetFrontBuffer()
    {
      return _frontBuffer;
    }
  }
}