using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToyGame
{
  public static class ResourceManager
  {

    private static Dictionary<string, Resource> resources = new Dictionary<string, Resource>();

    public static T GetDefault<T>(string path) where T : Resource
    {
      if (typeof(TextureResource).IsAssignableFrom(typeof(T)))
      {
        return FromPath<T>(@"C:\Users\Alec\thilenius\ToyGame\Assets\Textures\default.png");
      }
      return null;
    }

    public static T FromPath<T>(string path) where T : Resource
    {
      Resource resource = null;
      if (resources.TryGetValue(path, out resource))
      {
        return (T) resource;
      }
      // Load Texture
      if (typeof(TextureResource).IsAssignableFrom(typeof(T)))
      {
        resource = TextureResource.LoadSync(path);
      }
      // Load Mesh
      if (typeof(MeshResource).IsAssignableFrom(typeof(T)))
      {
        resource = MeshResource.LoadSync(path);
      }
      resources.Add(path, resource);
      return (T) resource;
    }

  }
}
