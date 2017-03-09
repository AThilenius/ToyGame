using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using System.Drawing.Imaging;
using FreeImageAPI;

namespace ToyGame
{
  sealed class GLTexture : IDisposable
  {

    private readonly int handle = GL.GenTexture();

    public GLTexture(string filePath, GLTextureParams textureParams)
    {
      FREE_IMAGE_FORMAT fileFormat = FreeImage.GetFileType(filePath, 0);
      if (fileFormat == FREE_IMAGE_FORMAT.FIF_UNKNOWN)
      {
        fileFormat = FreeImage.GetFIFFromFilename(filePath);
        if (fileFormat == FREE_IMAGE_FORMAT.FIF_UNKNOWN)
        {
          throw new Exception("Failed to load image at: " + filePath);
        }
        if (!FreeImage.FIFSupportsReading(fileFormat))
        {
          throw new Exception("The file type is not supported: " + filePath);
        }
      }
      FIBITMAP image = FreeImage.Load(fileFormat, filePath, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
      uint bitsPerPixel = FreeImage.GetBPP(image);
      FIBITMAP bitmap32 = bitsPerPixel == 32 ? image : FreeImage.ConvertTo32Bits(image);
      uint width = FreeImage.GetWidth(bitmap32);
      uint height = FreeImage.GetHeight(bitmap32);
      IntPtr data = FreeImage.GetBits(bitmap32);
      LoadGLTexture(width, height, textureParams, data);
      FreeImage.FreeHbitmap(data);
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

    private void LoadGLTexture(uint width, uint height, GLTextureParams textureParams, IntPtr data)
    {
      GL.BindTexture(textureParams.Target, handle);
      if (textureParams.UseAnisotropicFiltering)
      {
        float maxAniso;
        GL.GetFloat((GetPName) ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt, out maxAniso);
        GL.TexParameter(textureParams.Target, (TextureParameterName) ExtTextureFilterAnisotropic.TextureMaxAnisotropyExt, maxAniso);
      }
      GL.TexParameter(textureParams.Target, TextureParameterName.TextureMagFilter, (int) textureParams.MagFilter);
      GL.TexParameter(textureParams.Target, TextureParameterName.TextureMinFilter, (int) textureParams.MinFilter);
      GL.TexParameter(textureParams.Target, TextureParameterName.TextureWrapS, (int) textureParams.WrapS);
      GL.TexParameter(textureParams.Target, TextureParameterName.TextureWrapT, (int) textureParams.WrapT);
      GL.TexImage2D(textureParams.Target, 0, PixelInternalFormat.Rgba, (int) width, (int) height, 0,
          OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data);
      if (textureParams.GenerateMipMaps)
      {
        GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
      }
      GL.BindTexture(textureParams.Target, 0);
    }

  }
}
