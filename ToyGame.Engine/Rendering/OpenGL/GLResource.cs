using System;
using OpenTK.Graphics.OpenGL;

namespace ToyGame.Rendering.OpenGL
{
  internal interface IGLResource
  {
    #region Fields / Properties

    int GLHandle { get; }

    #endregion

    void GpuAllocate();
    void GpuFree();
  }

  internal abstract class GLResource<T> : IGLResource, IDisposable, IComparable<T> where T : IGLResource
  {
    #region Fields / Properties

    public readonly RenderContext RenderContext;
    protected Func<int> GLAllocAction;
    protected Action<int> GLFreeAction;

    #endregion

    protected GLResource(RenderContext renderContext) : this(renderContext, GL.GenBuffer, GL.DeleteBuffer)
    {
    }

    protected GLResource(RenderContext renderContext, Func<int> glAllocAction, Action<int> glFreeAction)
    {
      RenderContext = renderContext;
      GLAllocAction = glAllocAction;
      GLFreeAction = glFreeAction;
    }

    /// <summary>
    /// IComparable Implementation
    /// </summary>
    public int CompareTo(T other)
    {
      return GLHandle.CompareTo(other.GLHandle);
    }

    /// <summary>
    /// IDisposable Implementation
    /// </summary>
    public void Dispose()
    {
      GpuFree();
    }

    /// <summary>
    /// IGLResource Implementation
    /// </summary>
    public int GLHandle { get; private set; } = -1;

    public void GpuAllocate()
    {
      if (GLHandle != -1) return;
      RenderContext.AddResourceLoadAction(() =>
      {
        GLHandle = GLAllocAction();
        LoadToGpu();
      });
    }

    public void GpuFree()
    {
      if (GLHandle == -1) return;
      var handleId = GLHandle;
      GLHandle = -1;
      RenderContext.AddResourceLoadAction(() => GLFreeAction(handleId));
    }

    /// <summary>
    /// Called during GPU Allocation, should be overriden by derrived type to buffer
    /// GPU data and such things.
    /// </summary>
    protected abstract void LoadToGpu();
  }
}