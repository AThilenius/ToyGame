using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToyGame
{
  public enum ResourceType
  {
    Texture,
    Mesh,
    MagicaVoxel
  }

  public abstract class Resource
  {

    public string PathOnDisk;
    public ResourceType ResourceType;

    public Resource(string path, ResourceType type)
    {
      PathOnDisk = path;
      ResourceType = type;
    }

  }
}
