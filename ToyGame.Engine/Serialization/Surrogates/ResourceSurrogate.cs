using System;
using ProtoBuf;
using ToyGame.Resources;

namespace ToyGame.Serialization.Surrogates
{
  /// <summary>
  ///   This allows a resource to be serialized by-value, while storing the data by-ref
  /// </summary>
  [ProtoContract]
  internal class ResourceSurrogate
  {
    #region Fields / Properties

    [ProtoMember(2)] public Guid ResourceGuid;
    [ProtoMember(2)] public Guid ResourceBundleGuid;

    #endregion

    public static implicit operator ResourceSurrogate(Resource resource)
    {
      return new ResourceSurrogate {ResourceGuid = resource.Guid, ResourceBundleGuid = resource.ResourceBundle.Guid};
    }

    public static implicit operator Resource(ResourceSurrogate surrogate)
    {
      return ResourceBundleManager.Instance.GetBundle(surrogate.ResourceBundleGuid).GetResource(surrogate.ResourceGuid);
    }
  }
}