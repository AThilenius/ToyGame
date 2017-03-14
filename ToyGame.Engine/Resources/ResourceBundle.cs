using System;
using System.Collections.Concurrent;
using System.IO;
using ProtoBuf;
using ToyGame.Rendering;

namespace ToyGame.Resources
{
  /// <summary>
  ///   An exception that can be thrown during resouce importing.
  /// </summary>
  public class ResourceImportException : Exception
  {
    public ResourceImportException(string reason) : base(reason)
    {
    }
  }

  /// <summary>
  ///   Represents a bundle of resources. This can be an active project in the editor (where files are
  ///   saves one at a time) or have been loaded from a bin file.
  /// </summary>
  [ProtoContract]
  public class ResourceBundle
  {
    #region Types

    public enum BundleSourceType
    {
      ProjectPath
    }

    #endregion

    #region Fields / Properties

    public readonly RenderContext RenderContext;
    [ProtoMember(1)] public Guid Guid = Guid.NewGuid();
    [ProtoMember(2)] public BundleSourceType SourceType;
    [ProtoMember(3)] public string ProjectPath;
    private const string AssetExtension = ".xasset";

    private readonly ConcurrentDictionary<Guid, Resource> _allReferances =
      new ConcurrentDictionary<Guid, Resource>();

    #endregion

    internal ResourceBundle(RenderContext renderContext, string projectPath,
      BundleSourceType sourceType = BundleSourceType.ProjectPath)
    {
      RenderContext = renderContext;
      ProjectPath = projectPath;
      SourceType = sourceType;
    }

    public Resource GetResource(Guid guid)
    {
      Resource resouce;
      if (_allReferances.TryGetValue(guid, out resouce))
      {
        return resouce;
      }
      throw new Exception("The Resource has not yet been added to the bundle.");
    }

    public Resource ImportResource(string fromPath, string toDirectory)
    {
      // I'm not sure that there is any point in doing importing async. And it looks like FreeImage needs
      // to look at the file anyway.
      fromPath = Path.Combine(ProjectPath, fromPath);
      var toPath = Path.Combine(ProjectPath, toDirectory, Path.GetFileNameWithoutExtension(fromPath) + AssetExtension);
      var resource = Resource.GetDerivedResouceFromFileType(this, fromPath);
      resource.ImportFromFullPath(fromPath);
      // Save it to file as well
      using (var file = File.Create(toPath))
      {
        Serializer.Serialize(file, resource.DataBlock);
        if (file.Length == 0)
        {
          throw new ResourceImportException("Failed to save to: " + toPath);
        }
      }
      resource.GLResource?.GpuAllocate();
      _allReferances.TryAdd(resource.Guid, resource);
      return resource;
    }
  }
}