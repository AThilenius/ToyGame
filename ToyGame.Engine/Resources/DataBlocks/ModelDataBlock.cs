using OpenTK;
using OpenTK.Graphics;
using ProtoBuf;

namespace ToyGame.Resources.DataBlocks
{
  [ProtoContract]
  internal class ModelPart
  {
    #region Fields / Properties

    [ProtoMember(1)]
    public string Name { get; internal set; }

    [ProtoMember(2)]
    public Vector3[] Positions { get; internal set; }

    [ProtoMember(3)]
    public Vector3[] Normals { get; internal set; }

    [ProtoMember(4)]
    public Vector2[] Uv0 { get; internal set; }

    [ProtoMember(5)]
    public Vector2[] Uv1 { get; internal set; }

    [ProtoMember(6)]
    public Color4[] Colors { get; internal set; }

    [ProtoMember(7)]
    public uint[] Indexes { get; internal set; }

    #endregion
  }

  [ProtoContract]
  internal class ModelDataBlock : ResourceDataBlock
  {
    #region Fields / Properties

    [ProtoMember(1)]
    public ModelPart[] ModelParts { get; internal set; }

    #endregion

    public ModelDataBlock() : base(ResourceType.Model)
    {
    }
  }
}