using System;
using System.Drawing;
using FreeImageAPI;
using OpenTK.Graphics.OpenGL;
using ToyGame.Utilities;

namespace ToyGame.Rendering.OpenGL
{
  internal sealed class GLStaticTexture : GLTexture
  {
    #region Fields / Properties

    private readonly IntPtr _data;
    private readonly Size _size;

    #endregion

    public GLStaticTexture(GLTextureParams textureParams, FIBITMAP bitmap) : base (textureParams)
    {
      _size = new Size((int) FreeImage.GetWidth(bitmap), (int) FreeImage.GetHeight(bitmap));
      _data = FreeImage.GetBits(bitmap);
    }

    protected override void LoadToGpu()
    {
      base.LoadToGpu();
      GL.TexImage2D(TextureParams.Target, 0, PixelInternalFormat.Rgba, _size.Width, _size.Height, 0, PixelFormat.Bgra,
        PixelType.UnsignedByte, _data);
      FreeImage.FreeHbitmap(_data);
      if (TextureParams.GenerateMipMaps) GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
      GL.BindTexture(TextureParams.Target, 0);
      DebugUtils.GLErrorCheck();
    }
  }
}