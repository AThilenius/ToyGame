using System;
using System.Collections.Concurrent;
using System.Linq;
using ProtoBuf;
using ToyGame.Rendering;

namespace ToyGame.Resources
{
  /// <summary>
  ///   Singleton class for loading and unloading resource bundles, as well as fetching bundles by Guid.
  /// </summary>
  [ProtoContract]
  public class ResourceBundleManager
  {
    #region Fields / Properties

    public static ResourceBundleManager Instance => _instance ?? (_instance = new ResourceBundleManager());
    private static ResourceBundleManager _instance;
    private ConcurrentDictionary<Guid, ResourceBundle> _bundlesByGuid = new ConcurrentDictionary<Guid, ResourceBundle>();

    [ProtoMember(1)]
    private ResourceBundle[] AllBundles
    {
      get { return _bundlesByGuid.Values.ToArray(); }
      set
      {
        _bundlesByGuid =
          new ConcurrentDictionary<Guid, ResourceBundle>(value.Select(bundle => new {bundle.Guid, bundle})
            .ToDictionary(pair => pair.Guid, pair => pair.bundle));
      }
    }

    #endregion

    public ResourceBundle GetBundle(Guid guid)
    {
      ResourceBundle bundle;
      _bundlesByGuid.TryGetValue(guid, out bundle);
      return bundle;
    }

    public ResourceBundle AddBundleFromProjectPath(string projectPath)
    {
      var bundle = new ResourceBundle(projectPath);
      Console.WriteLine(@"Need to load all .xassets in the folder here...");
      _bundlesByGuid.TryAdd(bundle.Guid, bundle);
      return bundle;
    }
  }
}