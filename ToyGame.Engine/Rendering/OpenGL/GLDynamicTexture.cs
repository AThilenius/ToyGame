using System.Drawing;
using System.Drawing.Imaging;
using OpenTK.Graphics.OpenGL;
using ToyGame.Utilities;
using PixelFormat = System.Drawing.Imaging.PixelFormat;

namespace ToyGame.Rendering.OpenGL
{
  /// <summary>
  ///   Represents a GL loaded bitmap that will be changing over time. This is done via double-buffering.
  /// </summary>
  internal sealed class GLDynamicTexture : GLTexture
  {
    #region Fields / Properties

    public Bitmap Bitmap
    {
      get
      {
        lock (_bufferLock)
        {
          return _doubleBuffer.BackBuffer;
        }
      }
    }

    private readonly object _bufferLock = new object();
    private DoubleBuffer<Bitmap> _doubleBuffer;
    private Bitmap _bitmapLoadNeeded;

    #endregion

    public GLDynamicTexture(GLTextureParams textureParams, int width, int height) : base(textureParams)
    {
      _doubleBuffer = new DoubleBuffer<Bitmap>(new Bitmap(width, height), new Bitmap(width, height));
      RenderContext.Active.OnSyncronize += (sender, args) =>
      {
        lock (_bufferLock)
        {
          _bitmapLoadNeeded = _doubleBuffer.SwapBuffers();
        }
      };
    }

    /// <summary>
    ///   Resizes the dynamic texture
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    public void Resize(int width, int height)
    {
      lock (_bufferLock)
      {
        _doubleBuffer = new DoubleBuffer<Bitmap>(new Bitmap(width, height), new Bitmap(width, height));
      }
    }

    /// <summary>
    ///   This must be called on the GPU thread. In a dynamic texture, this will do the actual GPU loading of the front buffer,
    ///   and is expensive the first time it's invoked per frame.
    /// </summary>
    /// <param name="textureUnit"></param>
    public override void Bind(TextureUnit textureUnit)
    {
      base.Bind(textureUnit);
      if (_bitmapLoadNeeded == null) return;
      var data = _bitmapLoadNeeded.LockBits(new Rectangle(0, 0, _bitmapLoadNeeded.Width, _bitmapLoadNeeded.Height),
        ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
      GL.TexImage2D(TextureParams.Target, 0, PixelInternalFormat.Rgba, _bitmapLoadNeeded.Width,
        _bitmapLoadNeeded.Height, 0, OpenTK.Graphics.OpenGL.PixelFormat.Bgra, PixelType.UnsignedByte, data.Scan0);
      _bitmapLoadNeeded.UnlockBits(data);
      if (TextureParams.GenerateMipMaps) GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);
      DebugUtils.GLErrorCheck();
      _bitmapLoadNeeded = null;
    }
  }
}