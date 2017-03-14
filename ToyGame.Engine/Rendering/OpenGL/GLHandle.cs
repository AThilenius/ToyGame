using System;

namespace ToyGame.Rendering.OpenGL
{
  /// <summary>
  ///   A GLHandle is tied to a specific RenderContext. This must be so that calling
  ///   code knows who to marshal the GL.DeleteBuffer / GL.DeleteTexture / ... calls to.
  /// </summary>
  internal class GLHandle : IComparable<GLHandle>
  {
    #region Fields / Properties

    public volatile int Handle = -1;
    public RenderContext RenderContext;

    #endregion

    public int CompareTo(GLHandle other)
    {
      return Handle.CompareTo(other.Handle);
    }
  }
}