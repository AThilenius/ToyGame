using System;
using OpenTK.Graphics.OpenGL;

namespace ToyGame.OpenGL
{
  internal class DeferredPipline
  {
    private readonly int _gDiffuseSpecBuffer = GL.GenTexture();
    private readonly int _gFrameBuffer = GL.GenFramebuffer();
    private readonly int _gNormalbuffer = GL.GenTexture();
    private readonly int _gPositionBuffer = GL.GenTexture();

    public DeferredPipline(int screenWidth, int screenHeight)
    {
      GL.BindFramebuffer(FramebufferTarget.Framebuffer, _gFrameBuffer);
      // Position GBuffer
      GL.BindTexture(TextureTarget.Texture2D, _gPositionBuffer);
      GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, screenWidth, screenHeight, 0,
        PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) All.Nearest);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) All.Nearest);
      GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
        TextureTarget.Texture2D, _gPositionBuffer, 0);
      // Normal GBuffer
      GL.BindTexture(TextureTarget.Texture2D, _gNormalbuffer);
      GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgb16f, screenWidth, screenHeight, 0,
        PixelFormat.Rgb, PixelType.Float, IntPtr.Zero);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) All.Nearest);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) All.Nearest);
      GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment0,
        TextureTarget.Texture2D, _gNormalbuffer, 0);
      // Color + Specular GBuffer
      GL.BindTexture(TextureTarget.Texture2D, _gDiffuseSpecBuffer);
      GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, screenWidth, screenHeight, 0, PixelFormat.Rgb,
        PixelType.UnsignedByte, IntPtr.Zero);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) All.Nearest);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) All.Nearest);
      GL.FramebufferTexture2D(FramebufferTarget.Framebuffer, FramebufferAttachment.ColorAttachment2,
        TextureTarget.Texture2D, _gDiffuseSpecBuffer, 0);
    }
  }
}