﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToyGame
{
  public class TextureResource : Resource
  {

    internal GLTexture GLTexture;

    private TextureResource(string path)
      : base(path, ResourceType.Texture)
    {
    }

    public static TextureResource LoadSync(string path)
    {
      TextureResource texture = new TextureResource(path);
      Bitmap bitmap = new Bitmap(path);
      BitmapData data = bitmap.LockBits(new System.Drawing.Rectangle(0, 0, bitmap.Width, bitmap.Height),
          ImageLockMode.ReadOnly, System.Drawing.Imaging.PixelFormat.Format32bppArgb);
      texture.GLTexture = new GLTexture(data);
      bitmap.UnlockBits(data);
      return texture;
    }

  }
}