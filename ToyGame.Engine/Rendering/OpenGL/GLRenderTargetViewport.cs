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
      //RenderContext.GLGraphicsContext.MakeCurrent(_windowInfo);
      throw new NotImplementedException();
    }

    public void FinalizeRender()
    {
      //RenderContext.GLGraphicsContext.SwapBuffers();
      throw new NotImplementedException();
    }
  }
}