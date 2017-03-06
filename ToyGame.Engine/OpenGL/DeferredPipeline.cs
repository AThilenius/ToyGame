using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace ToyGame
{
  class DeferredPipline
  {

    private int gFrameBuffer = GL.GenFramebuffer();
    private int gPositionBuffer = GL.GenTexture();
    private int gNormalbuffer = GL.GenTexture();
    private int gDiffuseSpecBuffer = GL.GenTexture();

    public DeferredPipline(int screenWidth, int screenHeight)
    {
      GL.BindFramebuffer(FramebufferTarget.Framebuffer, gFrameBuffer);
      // Position GBuffer
      GL.BindTexture(TextureTarget.Texture2D, gPositionBuffer);
      GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, screenWidth, screenHeight, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) All.Nearest);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) All.Nearest);
      GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, gPositionBuffer, 0);
      // Normal GBuffer
      GL.BindTexture(TextureTarget.Texture2D, gNormalbuffer);
      GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, screenWidth, screenHeight, 0, PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) All.Nearest);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) All.Nearest);
      GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0, TextureTarget.Texture2D, gNormalbuffer, 0);
      // Color + Specular GBuffer
      GL.BindTexture(TextureTarget.Texture2D, gDiffuseSpecBuffer);
      GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, screenWidth, screenHeight, 0, PixelFormat.Rgb, PixelType.UnsignedByte, IntPtr.Zero);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) All.Nearest);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) All.Nearest);
      GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment2, TextureTarget.Texture2D, gDiffuseSpecBuffer, 0);
    }

    //public void FullyRenderScene (Scene scene)
    //{

    //}

    //public void RenderGBuffer (Scene scene)
    //{
    //  GL.DrawBuffers(3, new DrawBuffersEnum[] { DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1, DrawBuffersEnum.ColorAttachment2 });
    //}

  }
}
