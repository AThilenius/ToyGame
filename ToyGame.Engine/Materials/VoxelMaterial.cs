using OpenTK.Graphics.OpenGL;
using ToyGame.OpenGL.Shaders;
using ToyGame.Resources;

namespace ToyGame.Materials
{
  public class VoxelMaterial : Material
  {
    private static VoxelShader _voxelShader;
    public TextureResource DiffuseTexture;
    public TextureResource MetallicRoughnessTexture;

    public VoxelMaterial()
    {
      if (_voxelShader == null)
      {
        _voxelShader = new VoxelShader();
      }
      Program = _voxelShader;
    }

    protected override void UpdateProgramUniforms()
    {
      DiffuseTexture?.GLTexture.Bind(TextureUnit.Texture0);
      MetallicRoughnessTexture?.GLTexture.Bind(TextureUnit.Texture1);
    }
  }
}