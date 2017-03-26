using System;
using OpenTK.Graphics;
using OpenTK.Platform;

namespace ToyGame.Rendering.OpenGL
{
  internal class GLRenderTargetViewport : IRenderTarget
  {
    #region Fields / Properties

    private readonly IWindowInfo _windowInfo;

    #endregion

    public GLRenderTargetViewport(IWindowInfo windowInfo)
    {
      _windowInfo = windowInfo;
    }

    public void Bind()
    {
      // Not much to do here, just make the current window current
      try
      {
        RenderContext.Active.GLGraphicsContext.MakeCurrent(_windowInfo);
      }
      catch (GraphicsContextException ex)
      {
        // Ignore it for now, the window is probably closing.
      }
    }

    public void FinalizeRender()
    {
      try
      {
        RenderContext.Active.GLGraphicsContext.SwapBuffers();
      }
      catch (GraphicsContextException ex)
      {
        // Ignore it for now, the window is probably closing.
      }
    }
  }
}