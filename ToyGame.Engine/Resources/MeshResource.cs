using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToyGame
{
  public class MeshResource : Resource
  {

    internal GLGeometry GLGeometry;

    private static WavefrontObjLoader objLoader = new WavefrontObjLoader();

    private MeshResource(string path)
      : base(path, ResourceType.Mesh)
    {
    }

    public static MeshResource LoadSync(string path)
    {
      MeshResource mesh = new MeshResource(path);
      Vector3[] positions;
      uint[] indexes;
      Vector3[] normals;
      Vector2[] uv0;
      Vector2[] uv1;
      Color4[] colors;
      if (path.EndsWith(".obj", StringComparison.CurrentCultureIgnoreCase))
      {
        objLoader.LoadFromFile(path, out positions, out indexes, out normals, out uv0, out uv1, out colors);
      }
      else
      {
        throw new Exception("The mesh type at [" + path + "] is not supported.");
      }
      mesh.GLGeometry = new GLGeometry(positions, indexes, normals, uv0, uv1, colors, OpenTK.Graphics.OpenGL.BufferUsageHint.StaticDraw);
      return mesh;
    }

  }
}
