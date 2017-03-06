using FileFormatWavefront;
using OpenTK;
using OpenTK.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToyGame
{
  class WavefrontObjLoader
  {

    public void LoadFromFile (string path, out Vector3[] positions, out uint[] indexes, out Vector3[] normals, out Vector2[] uv0, out Vector2[] uv1, out Color4[] colors)
    {
      var result = FileFormatObj.Load(path, false);
      var model = result.Model;
      // Because Wavefront is a massive peice of shit, we have to normalize the face list into the vert/norm/uv arrays.
      // What a fucking nightmare.
      Dictionary<string, int> uniqueVertice = new Dictionary<string, int>();
      List<Vector3> positionsList = new List<Vector3>();
      List<Vector3> normalsList = new List<Vector3>();
      List<Vector2> uvsList = new List<Vector2>();
      List<uint> indeciesList = new List<uint>();
      foreach (var group in model.Groups)
      {
        foreach (var face in group.Faces)
        {
          foreach (var index in face.Indices)
          {
            Vector3 position = new Vector3(model.Vertices[index.vertex].x, model.Vertices[index.vertex].y, model.Vertices[index.vertex].z);
            Vector3 normal = new Vector3(model.Normals[index.vertex].x, model.Normals[index.vertex].y, model.Normals[index.vertex].z);
            Vector2 uv = new Vector2(model.Uvs[index.uv.Value].u, 1.0f - model.Uvs[index.uv.Value].v);
            string fullName = position + ":" + normal + ":" + uv;
            int existingIndex;
            if (uniqueVertice.TryGetValue(fullName, out existingIndex))
            {
              indeciesList.Add((uint) existingIndex);
            }
            else
            {
              positionsList.Add(position);
              normalsList.Add(normal);
              uvsList.Add(uv);
              indeciesList.Add((uint) positionsList.Count - 1);
            }
          }
        }
      }
      positions = positionsList.ToArray();
      normals = normalsList.ToArray();
      uv0 = uvsList.ToArray();
      uv1 = null;
      colors = null;
      indexes = indeciesList.ToArray();
    }

  }
}
