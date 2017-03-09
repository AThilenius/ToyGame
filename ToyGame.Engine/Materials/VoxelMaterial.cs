using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace ToyGame
{
  public class VoxelMaterial : Material
  {

    public TextureResource DiffuseTexture = ResourceManager.FromPath<TextureResource>(@"C:\Users\Alec\thilenius\ToyGame\Assets\Models\AKM\textures\WPNT_AKM_BaseColor.tga");
    public TextureResource RoughnessMetallicTexture = ResourceManager.FromPath<TextureResource>(@"C:\Users\Alec\thilenius\ToyGame\Assets\Models\AKM\textures\WPNT_AKM_RoughnessMetallic.tga");

    private static VoxelShader standardShader;

    public VoxelMaterial()
    {
      if (standardShader == null)
      {
        standardShader = new VoxelShader();
      }
      Program = standardShader;
    }

    protected override void UpdateProgramUniforms()
    {
      DiffuseTexture.GLTexture.Bind(TextureUnit.Texture0);
      RoughnessMetallicTexture.GLTexture.Bind(TextureUnit.Texture1);
    }
  }
}
