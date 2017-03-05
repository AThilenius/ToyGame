using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;

namespace ToyGame
{
  class RenderPipeline
  {

    //GLuint gBuffer;
    //glGenFramebuffers(1, &gBuffer);
    //glBindFramebuffer(GL_FRAMEBUFFER, gBuffer);
    //GLuint gPosition, gNormal, gColorSpec;

    //// - Position color buffer
    //glGenTextures(1, &gPosition);
    //glBindTexture(GL_TEXTURE_2D, gPosition);
    //glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB16F, SCR_WIDTH, SCR_HEIGHT, 0, GL_RGB, GL_FLOAT, NULL);
    //glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
    //glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
    //glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT0, GL_TEXTURE_2D, gPosition, 0);

    //// - Normal color buffer
    //glGenTextures(1, &gNormal);
    //glBindTexture(GL_TEXTURE_2D, gNormal);
    //glTexImage2D(GL_TEXTURE_2D, 0, GL_RGB16F, SCR_WIDTH, SCR_HEIGHT, 0, GL_RGB, GL_FLOAT, NULL);
    //glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
    //glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
    //glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT1, GL_TEXTURE_2D, gNormal, 0);

    //// - Color + Specular color buffer
    //glGenTextures(1, &gAlbedoSpec);
    //glBindTexture(GL_TEXTURE_2D, gAlbedoSpec);
    //glTexImage2D(GL_TEXTURE_2D, 0, GL_RGBA, SCR_WIDTH, SCR_HEIGHT, 0, GL_RGBA, GL_UNSIGNED_BYTE, NULL);
    //glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MIN_FILTER, GL_NEAREST);
    //glTexParameteri(GL_TEXTURE_2D, GL_TEXTURE_MAG_FILTER, GL_NEAREST);
    //glFramebufferTexture2D(GL_FRAMEBUFFER, GL_COLOR_ATTACHMENT2, GL_TEXTURE_2D, gAlbedoSpec, 0);

    //// - Tell OpenGL which color attachments we'll use (of this framebuffer) for rendering 
    //GLuint attachments[3] = { GL_COLOR_ATTACHMENT0, GL_COLOR_ATTACHMENT1, GL_COLOR_ATTACHMENT2 };
    //glDrawBuffers(3, attachments);

    //// Then also add render buffer object as depth buffer and check for completeness.
    //[...]

    private int gFrameBuffer = GL.GenFramebuffer();
    private int gPositionBuffer = GL.GenTexture();
    private int gNormalbuffer = GL.GenTexture();
    private int gDiffuseSpecBuffer = GL.GenTexture();

    public RenderPipeline(int screenWidth, int screenHeight)
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

    public void FullyRenderScene (Scene scene)
    {

    }

    public void RenderGBuffer (Scene scene)
    {
      GL.DrawBuffers(3, new DrawBuffersEnum[] { DrawBuffersEnum.ColorAttachment0, DrawBuffersEnum.ColorAttachment1, DrawBuffersEnum.ColorAttachment2 });
    }

  }
}
