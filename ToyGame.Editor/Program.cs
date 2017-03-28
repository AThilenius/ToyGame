using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using ToyGame.Gameplay;
using ToyGame.Materials;
using ToyGame.Resources;

namespace ToyGame.Editor
{
  class Program
  {
    static void Main(string[] args)
    {
      var toyEngine = new ToyEngine();
      var window = toyEngine.CreateWindow("Editor");
      // Resources
      //var resourceBundle = ResourceBundleManager.Instance.AddBundleFromProjectPath(@"C:\Users\Alec\thilenius\ToyGame");
      //var material = new VoxelMaterial
      //{
      //  DiffuseTexture =
      //    resourceBundle.ImportResource(@"Assets\Textures\build_crane_01_a.tif", "ImportCache") as TextureResource,
      //  MetallicRoughnessTexture =
      //    resourceBundle.ImportResource(@"Assets\Textures\build_crane_01_sg.tif", "ImportCache") as TextureResource
      //};
      //var model = resourceBundle.ImportResource(@"Assets\Models\build_crane_01.FBX", @"ImportCache") as ModelResource;
      //var staticMesh = new AStaticMesh(model, material)
      //{
      //  Transform =
      //  {
      //    Scale = new Vector3(0.1f),
      //    Position = new Vector3(0, 0, -200)
      //  }
      //};
      // World stuff
      var level = new Level();
      window.World.AddLevel(level);
      //level.AddActor(staticMesh);
      // Run it
      toyEngine.Run();
    }
  }
}
