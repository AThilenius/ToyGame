﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;

namespace ToyGame
{
  sealed class GLTexture : IDisposable
  {

    private readonly int handle = GL.GenTexture();

    public GLTexture(BitmapData data, bool repeat = true)
    {
      GL.BindTexture(TextureTarget.Texture2D, handle);
      float maxAniso;
      GL.GetFloat((GetPName) ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt, out maxAniso);
      GL.TexParameter(TextureTarget.Texture2D, (TextureParameterName) ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, maxAniso);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int) All.Nearest);
      GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int) All.Nearest);
      if (repeat)
      {
        //This will repeat the texture past its bounds set by TexImage2D
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) All.Repeat);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) All.Repeat);
      }
      else
      {
        //This will clamp the texture to the edge, so manipulation will result in skewing
        //It can also be useful for getting rid of repeating texture bits at the borders
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int) All.ClampToEdge);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int) All.ClampToEdge);
      }
      GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, data.Width, data.Height, 0,
          OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
      // Generate Mip Maps
      GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
      GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    public void Bind(TextureUnit textureUnit)
    {
      GL.ActiveTexture(textureUnit);
      GL.BindTexture(TextureTarget.Texture2D, handle);
    }

    public void Dispose()
    {
      GL.DeleteTexture(handle);
    }

  }
}