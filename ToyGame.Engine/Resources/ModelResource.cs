using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assimp;
using System.Diagnostics;
using System.IO;
using Assimp;

namespace ToyGame
{
  public class ModelResource : Resource
  {

    public string Name;

    internal List<GLMesh> GLMeshes = new List<GLMesh>();

    private ModelResource(string path)
      : base(path, ResourceType.Mesh)
    {
    }

    internal static Resource LoadSync(AssimpContext importer, string path)
    {
      ModelResource meshResource = new ModelResource(path);
      meshResource.Name = Path.GetFileNameWithoutExtension(path);
      Scene scene = importer.ImportFile(path, PostProcessPreset.TargetRealTimeMaximumQuality);

      foreach (Mesh mesh in scene.Meshes)
      {
        Vector3[] positions = mesh.Vertices.Select(v => new Vector3(v.X, v.Z, v.Y)).ToArray();
        uint[] indexes = mesh.Faces.SelectMany(f => f.Indices).Select(i => (uint) i).ToArray();
        Vector3[] normals = mesh.Normals.Select(n => new Vector3(n.X, n.Z, n.Y)).ToArray();
        Vector2[] uv0 = mesh.TextureCoordinateChannels[0].Select(uv => new Vector2(uv.X, uv.Y)).ToArray();
        // Not currently loaded
        Vector2[] uv1;
        // Not currently loaded
        Color4[] colors;
        meshResource.GLMeshes.Add(new GLMesh(positions, indexes, normals, uv0, null, null, OpenTK.Graphics.OpenGL.BufferUsageHint.StaticDraw));
      }
      return meshResource;
    }

  }
}
