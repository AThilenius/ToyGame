using FileFormatWavefront;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToyGame
{
  class WavefrontObjLoader
  {

    public Mesh LoadFromFile (string path)
    {
      var result = FileFormatObj.Load(path, false);
      var model = result.Model;
      // Because Wavefront is a massive peice of shit, we have to normalize the face list into the vert/norm/uv arrays.
      // What a fucking nightmare.
      Dictionary<string, int> uniqueVertice = new Dictionary<string, int>();
      List<Vector3> positions = new List<Vector3>();
      List<Vector3> normals = new List<Vector3>();
      List<Vector2> uvs = new List<Vector2>();
      List<uint> indecies = new List<uint>();
      foreach (var group in model.Groups)
      {
        foreach (var face in group.Faces)
        {
          foreach (var index in face.Indices)
          {
            Vector3 position = new Vector3(model.Vertices[index.vertex].x, model.Vertices[index.vertex].y, model.Vertices[index.vertex].z);
            Vector3 normal = new Vector3(model.Normals[index.vertex].x, model.Normals[index.vertex].y, model.Normals[index.vertex].z);
            Vector2 uv = new Vector2(model.Uvs[index.uv.Value].u, model.Uvs[index.uv.Value].v);
            string fullName = position + ":" + normal + ":" + uv;
            int existingIndex;
            if (uniqueVertice.TryGetValue(fullName, out existingIndex))
            {
              indecies.Add((uint) existingIndex);
            }
            else
            {
              positions.Add(position);
              normals.Add(normal);
              uvs.Add(uv);
              indecies.Add((uint) positions.Count - 1);
            }
          }
        }
      }
      return new Mesh { Positions = positions.ToArray(), Normals = normals.ToArray(), UV0 = uvs.ToArray(), Indices = indecies.ToArray() };
    }

  }
}
