using System;
using ProtoBuf;

namespace ToyGame.Resources.DataBlocks
{
  [ProtoContract]
  [ProtoInclude(100, typeof (TextureDataBlock))]
  [ProtoInclude(200, typeof (ModelDataBlock))]
  internal class ResourceDataBlock
  {
    #region Fields / Properties

    [ProtoMember(1)] public Guid Guid = Guid.NewGuid();
    [ProtoMember(2)] public string Name = "Unnamed";
    [ProtoMember(3)] public ResourceType ResourceType;

    #endregion

    public ResourceDataBlock(ResourceType resourceType)
    {
      ResourceType = resourceType;
    }
  }
}