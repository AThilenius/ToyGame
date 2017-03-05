using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK.Graphics.OpenGL;
using OpenTK;

namespace ToyGame
{
  class StandardMaterial : Material
  {

    public Matrix4 ProjectionMatrix = Matrix4.Identity;
    public Matrix4 ViewMatrix = Matrix4.Identity;
    public Matrix4 ModelMatrix = Matrix4.Identity;
    public Texture DiffuseTexture = Texture.FromPath(@"C:\Users\Alec\thilenius\ToyGame\Assets\Models\AKM\textures\WPNT_AKM_BaseColor.bmp");
    public Texture NormalTexture = Texture.FromPath(@"C:\Users\Alec\thilenius\ToyGame\Assets\Models\AKM\textures\WPNT_AKM_Normal.bmp");
    public Texture RoughnessMetallicTexture = Texture.FromPath(@"C:\Users\Alec\thilenius\ToyGame\Assets\Models\AKM\textures\WPNT_AKM_RoughnessMetallic.bmp");
    public Texture AmbientOcclusionTexture = Texture.FromPath(@"C:\Users\Alec\thilenius\ToyGame\Assets\Models\AKM\textures\WPNT_AKM_Ambient_occlusion.bmp");

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
      standardShader.ProjectionMatrix = ProjectionMatrix;
      standardShader.ViewMatrix = ViewMatrix;
      standardShader.ModelMatrix = ModelMatrix;
      DiffuseTexture.Bind(TextureUnit.Texture0);
      NormalTexture.Bind(TextureUnit.Texture1);
      RoughnessMetallicTexture.Bind(TextureUnit.Texture2);
      AmbientOcclusionTexture.Bind(TextureUnit.Texture3);
    }
  }
}
