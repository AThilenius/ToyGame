using System;
using System.IO;
using Assimp;
using FreeImageAPI;
using ProtoBuf;
using ToyGame.Resources.DataBlocks;

namespace ToyGame.Resources
{
  public enum ResourceType
  {
    Unknown,
    Texture,
    Model
  }

  public abstract class Resource
  {

    #region Fields / Properties

    public Guid Guid => DataBlock.Guid;
    public ResourceType ResourceType => DataBlock.ResourceType;

    public string Name
    {
      get { return DataBlock.Name; }
      set { DataBlock.Name = value; }
    }

    public string FilePath { get; internal set; }
    public ResourceBundle ResourceBundle { get; internal set; }
    internal ResourceDataBlock DataBlock;

    #endregion

    internal Resource(ResourceBundle bundle)
    {
      ResourceBundle = bundle;
    }

    /// <summary>
    /// Override in a Resouce class to serialize the object to a corresponding *DataBlock object.
    /// </summary>
    /// <param name="stream">The stream to save to</param>
    public virtual void SaveToStream(Stream stream)
    {
      
    }

    /// <summary>
    /// Override in a Resouce to handle importing an asset (normally from file) and converting
    /// it's data to the resouce format.
    /// </summary>
    /// <param name="fullPath"></param>
    public virtual void ImportFromFullPath(string fullPath)
    {
      Name = Path.GetFileNameWithoutExtension(fullPath);
      FilePath = fullPath;
    }

    /// <summary>
    /// Implemented by Resouce classes to handle loading their data into GPU memory.
    /// </summary>
    internal abstract void LoadToGpu();

    /// <summary>
    /// Imports the resouce from a serialized (protobuf) stream. Uses RTTI to cast to the derrived type.
    /// </summary>
    /// <param name="stream">The stream to load from</param>
    internal static Resource LoadFromStream(Stream stream)
    {
      throw new NotImplementedException();
    }

    public static ResourceType GetResourceTypeFromFile(string fullPath)
    {
      // Check if it's an image
      if (FreeImage.GetFileType(fullPath, 0) != FREE_IMAGE_FORMAT.FIF_UNKNOWN ||
          FreeImage.GetFIFFromFilename(fullPath) != FREE_IMAGE_FORMAT.FIF_UNKNOWN)
      {
        return ResourceType.Texture;
      }
      // Check if it's a model
      var assimp = new AssimpContext();
      return assimp.IsImportFormatSupported(Path.GetExtension(fullPath)) ? ResourceType.Model : ResourceType.Unknown;
      // Unknown resource type, out of luck
    }

    internal static Resource GetDerivedResouceFromFileType(ResourceBundle resourceBundle, string fullPath)
    {
      switch (GetResourceTypeFromFile(fullPath))
      {
        case ResourceType.Model:
          return new ModelResource(resourceBundle);
        case ResourceType.Texture:
          return new TextureResource(resourceBundle);
        default:
          throw new ResourceImportException("Unhandled file type");
      }
    }

  }
}