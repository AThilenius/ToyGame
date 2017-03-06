using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace ToyGame
{
  public class StandardMaterial : Material
  {

    public TextureResource DiffuseTexture = ResourceManager.FromPath<TextureResource>(@"C:\Users\Alec\thilenius\ToyGame\Assets\Models\AKM\textures\WPNT_AKM_BaseColor.bmp");
    public TextureResource NormalTexture = ResourceManager.FromPath<TextureResource>(@"C:\Users\Alec\thilenius\ToyGame\Assets\Models\AKM\textures\WPNT_AKM_Normal.bmp");
    public TextureResource RoughnessMetallicTexture = ResourceManager.FromPath<TextureResource>(@"C:\Users\Alec\thilenius\ToyGame\Assets\Models\AKM\textures\WPNT_AKM_RoughnessMetallic.bmp");
    public TextureResource AmbientOcclusionTexture = ResourceManager.FromPath<TextureResource>(@"C:\Users\Alec\thilenius\ToyGame\Assets\Models\AKM\textures\WPNT_AKM_Ambient_occlusion.bmp");

    private static StandardShader standardShader;

    public StandardMaterial()
    {
      if (standardShader == null)
      {
        standardShader = new StandardShader();
      }
      Program = standardShader;
    }

    protected override void UpdateProgramUniforms()
    {
      DiffuseTexture.GLTexture.Bind(TextureUnit.Texture0);
      NormalTexture.GLTexture.Bind(TextureUnit.Texture1);
      RoughnessMetallicTexture.GLTexture.Bind(TextureUnit.Texture2);
      AmbientOcclusionTexture.GLTexture.Bind(TextureUnit.Texture3);
    }
  }
}
