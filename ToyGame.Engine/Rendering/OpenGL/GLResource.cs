using System;
using OpenTK.Graphics.OpenGL;

namespace ToyGame.Rendering.OpenGL
{
  internal interface IGLResource
  {
    #region Fields / Properties

    int GLHandle { get; }

    #endregion

    void GpuAllocateDeferred();
    void GpuFreeDeferred();
  }

  internal abstract class GLResource<T> : IGLResource, IDisposable, IComparable<T> where T : IGLResource
  {
    #region Fields / Properties

    protected Func<int> GLAllocAction;
    protected Action<int> GLFreeAction;

    #endregion

    protected GLResource() : this(GL.GenBuffer, GL.DeleteBuffer)
    {
    }

    protected GLResource(Func<int> glAllocAction, Action<int> glFreeAction)
    {
      GLAllocAction = glAllocAction;
      GLFreeAction = glFreeAction;
    }

    /// <summary>
    ///   IComparable Implementation
    /// </summary>
    public int CompareTo(T other)
    {
      return GLHandle.CompareTo(other.GLHandle);
    }

    /// <summary>
    ///   IDisposable Implementation
    /// </summary>
    public void Dispose()
    {
      GpuFreeDeferred();
    }

    /// <summary>
    ///   IGLResource Implementation
    /// </summary>
    public int GLHandle { get; private set; } = -1;

    public void GpuAllocateDeferred()
    {
      RenderContext.Active.AddResourceLoadAction(GpuAllocateImmediate);
    }

    public void GpuAllocateImmediate()
    {
        if (GLHandle != -1) return;
        GLHandle = GLAllocAction();
        LoadToGpu();
    }

    public void GpuFreeDeferred()
    {
      RenderContext.Active.AddResourceLoadAction(GpuFreeImmediate);
    }

    public void GpuFreeImmediate()
    {
        if (GLHandle == -1) return;
        GLFreeAction(GLHandle);
        GLHandle = -1;
    }

    /// <summary>
    ///   Called during GPU Allocation, should be overriden by derrived type to buffer
    ///   GPU data and such things.
    /// </summary>
    protected abstract void LoadToGpu();
  }
}