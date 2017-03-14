using OpenTK.Graphics.OpenGL;
using ToyGame.Rendering;
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
    private static GLStandardShader _glStandardShader;

    #endregion

    public StandardMaterial(RenderCore renderCore)
    {
      if (_glStandardShader == null)
      {
        _glStandardShader = new GLStandardShader(renderCore);
      }
      Program = _glStandardShader;
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