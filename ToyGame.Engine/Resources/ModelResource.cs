using System.IO;
using System.Linq;
using Assimp;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using ToyGame.OpenGL;
using ToyGame.Resources.DataBlocks;

namespace ToyGame.Resources
{
  public class ModelResource : Resource
  {
    #region Fields / Properties

    internal GLMesh[] GLMeshes;

    #endregion

    internal ModelResource(ResourceBundle bundle) : base(bundle)
    {
      DataBlock = new ModelDataBlock();
    }

    public override void ImportFromFullPath(string fullPath)
    {
      base.ImportFromFullPath(fullPath);
      var assimpContext = new AssimpContext();
      var scene = assimpContext.ImportFile(fullPath, PostProcessPreset.TargetRealTimeMaximumQuality);
      ((ModelDataBlock) DataBlock).ModelParts = scene.Meshes
        .Where(aMesh => aMesh.Vertices.Count > 0)
        .Select(aMesh => new ModelPart
        {
          Name = aMesh.Name,
          Positions = aMesh.Vertices.Select(v => new Vector3(v.X, v.Z, v.Y)).ToArray(),
          Indexes = aMesh.Faces.SelectMany(f => f.Indices).Select(i => (uint) i).ToArray(),
          Normals = aMesh.Normals.Select(n => new Vector3(n.X, n.Z, n.Y)).ToArray(),
          Uv0 = aMesh.TextureCoordinateChannels[0].Select(uv => new Vector2(uv.X, uv.Y)).ToArray()
          // UV1 and Colors not currently loaded
        }).ToArray();
      ((ModelDataBlock) DataBlock).Name = Path.GetFileNameWithoutExtension(fullPath);
    }

    internal override void LoadToGpu()
    {
      GLMeshes = ((ModelDataBlock) DataBlock).ModelParts.Select(part => new GLMesh(
        part.Positions, part.Indexes, part.Normals, part.Uv0, part.Uv1, part.Colors,
        BufferUsageHint.StaticDraw)).ToArray();
    }
  }
}