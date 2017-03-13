using OpenTK;
using OpenTK.Graphics.OpenGL;
using ToyGame.Gameplay;
using ToyGame.Rendering.OpenGL;
using ToyGame.Rendering.Shaders;
using ToyGame.Resources;

namespace ToyGame.Materials
{
  public class StandardMaterial : Material
  {
    #region Fields / Properties

    public TextureResource AmbientOcclusionTexture;
    public TextureResource DiffuseTexture;
    public TextureResource NormalTexture;
    public TextureResource RoughnessMetallicTexture;
    private static StandardShader _standardShader;

    #endregion

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

    internal override GLDrawCall.GLTextureBind[] GenerateTexturebinds()
    {
      return new[]
      {
        new GLDrawCall.GLTextureBind(DiffuseTexture.GLTexture, TextureTarget.Texture2D, TextureUnit.Texture0),
        new GLDrawCall.GLTextureBind(NormalTexture.GLTexture, TextureTarget.Texture2D, TextureUnit.Texture1),
        new GLDrawCall.GLTextureBind(RoughnessMetallicTexture.GLTexture, TextureTarget.Texture2D, TextureUnit.Texture2),
        new GLDrawCall.GLTextureBind(AmbientOcclusionTexture.GLTexture, TextureTarget.Texture2D, TextureUnit.Texture3)
      };
    }
  }
}