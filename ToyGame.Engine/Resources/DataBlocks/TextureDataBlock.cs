using ProtoBuf;

namespace ToyGame.Resources.DataBlocks
{
  [ProtoContract]
  internal class TextureDataBlock : ResourceDataBlock
  {
    #region Fields / Properties

    [ProtoMember(1)]
    public uint Width { get; internal set; }

    [ProtoMember(2)]
    public uint Height { get; internal set; }

    [ProtoMember(3)]
    public byte[] RawData { get; internal set; }

    #endregion

    public TextureDataBlock() : base(ResourceType.Texture)
    {
    }
  }
}