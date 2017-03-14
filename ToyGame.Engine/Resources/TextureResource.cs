﻿using System.IO;
using FreeImageAPI;
using ToyGame.Rendering;
using ToyGame.Rendering.OpenGL;
using ToyGame.Resources.DataBlocks;

namespace ToyGame.Resources
{
  public class TextureResource : Resource
  {
    #region Fields / Properties

    public uint Width => ((TextureDataBlock) DataBlock).Width;
    public uint Height => ((TextureDataBlock) DataBlock).Height;
    public byte[] RawData => ((TextureDataBlock) DataBlock).RawData;
    internal GLTexture GLTexture;

    #endregion

    internal TextureResource(ResourceBundle bundle) : base(bundle)
    {
      DataBlock = new TextureDataBlock();
    }

    public override void ImportFromFullPath(string fullPath)
    {
      base.ImportFromFullPath(fullPath);
      var fileFormat = FreeImage.GetFileType(fullPath, 0);
      if (fileFormat == FREE_IMAGE_FORMAT.FIF_UNKNOWN)
      {
        fileFormat = FreeImage.GetFIFFromFilename(fullPath);
        if (fileFormat == FREE_IMAGE_FORMAT.FIF_UNKNOWN)
        {
          throw new ResourceImportException("Failed to import texture from: " + fullPath);
        }
        if (!FreeImage.FIFSupportsReading(fileFormat))
        {
          throw new ResourceImportException("Unsupported file format: " + fullPath);
        }
      }
      var image = FreeImage.Load(fileFormat, fullPath, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
      var bitsPerPixel = FreeImage.GetBPP(image);
      // Convert all images (for now) to full 32bit.
      var bitmap32 = bitsPerPixel == 32 ? image : FreeImage.ConvertTo32Bits(image);
      ((TextureDataBlock) DataBlock).Width = FreeImage.GetWidth(bitmap32);
      ((TextureDataBlock) DataBlock).Height = FreeImage.GetHeight(bitmap32);
      var stream = new MemoryStream();
      FreeImage.SaveToStream(bitmap32, stream, FREE_IMAGE_FORMAT.FIF_TARGA);
      ((TextureDataBlock) DataBlock).RawData = stream.ToArray();
    }

    internal override void LoadToGpu(RenderCore renderCore)
    {
      var stream = new MemoryStream(RawData);
      var image = FreeImage.LoadFromStream(stream, FREE_IMAGE_LOAD_FLAGS.DEFAULT);
      var data = FreeImage.GetBits(image);
      GLTexture = GLTexture.LoadGLTexture(renderCore, Width, Height, GLTextureParams.Default, data);
      FreeImage.FreeHbitmap(data);
    }
  }
}