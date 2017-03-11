using OpenTK.Graphics.OpenGL;
using ToyGame.OpenGL.Shaders;
using ToyGame.Resources;

namespace ToyGame.Materials
{
  public class StandardMaterial : Material
  {
    private static StandardShader _standardShader;
    public TextureResource AmbientOcclusionTexture;
    public TextureResource DiffuseTexture;
    public TextureResource NormalTexture;
    public TextureResource RoughnessMetallicTexture;

    public StandardMaterial()
    {
      if (_standardShader == null)
      {
        _standardShader = new StandardShader();
      }
      Program = _standardShader;
    }

    protected override void UpdateProgramUniforms()
    {
      DiffuseTexture?.GLTexture.Bind(TextureUnit.Texture0);
      NormalTexture?.GLTexture.Bind(TextureUnit.Texture1);
      RoughnessMetallicTexture?.GLTexture.Bind(TextureUnit.Texture2);
      AmbientOcclusionTexture?.GLTexture.Bind(TextureUnit.Texture3);
    }
  }
}