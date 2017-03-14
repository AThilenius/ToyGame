using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using ToyGame.Rendering;
using ToyGame.Rendering.OpenGL;
using ToyGame.Rendering.Shaders;
using ToyGame.Resources;

namespace ToyGame.Materials
{
  public class VoxelMaterial : Material
  {
    #region Fields / Properties

    public readonly Vector3 CameraPosition = Vector3.Zero;
    public readonly float Exposure = 1.0f;

    public readonly Vector3[] LightColors = new Vector3[4]
    {
      new Vector3(500.0f, 100.0f, 100.0f),
      new Vector3(100.0f, 500.0f, 100.0f),
      new Vector3(100.0f, 100.0f, 500.0f),
      new Vector3(100.0f, 100.0f, 100.0f)
    };

    public readonly Vector3[] LightPositions = new Vector3[4]
    {
      new Vector3(-10.0f, 0.0f, 10.0f),
      new Vector3(10.0f, 0.0f, 10.0f),
      new Vector3(-10.0f, -10.0f, 10.0f),
      new Vector3(10.0f, -10.0f, 10.0f)
    };

    public TextureResource DiffuseTexture;
    public TextureResource MetallicRoughnessTexture;
    private static GLVoxelShader _glVoxelShader;

    #endregion

    public VoxelMaterial(RenderContext renderContext)
    {
      if (_glVoxelShader == null)
      {
        _glVoxelShader = new GLVoxelShader(renderContext);
        _glVoxelShader.GpuAllocate();
      }
      Program = _glVoxelShader;
    }

    internal override GLDrawCall.UniformBind[] GenerateUniformBinds(Matrix4 modelMatrix)
    {
      var baseUniforms = base.GenerateUniformBinds(modelMatrix);
      var thisUniforms = new[]
      {
        new GLDrawCall.UniformBind("exposure-" + Exposure,
          () => GL.Uniform1(Program.GetUniformLocation("exposure"), Exposure)),
        new GLDrawCall.UniformBind("camPos-" + CameraPosition,
          () => GL.Uniform3(Program.GetUniformLocation("camPos"), CameraPosition)),
        new GLDrawCall.UniformBind("lightPositions-" + LightPositions.Aggregate("", (left, right) => left + right),
          () =>
          {
            for (var i = 0; i < 4; i++)
            {
              GL.Uniform3(Program.GetUniformLocation("lightPositions[" + i + "]"), LightPositions[i]);
            }
          }),
        new GLDrawCall.UniformBind("lightColors-" + LightColors.Aggregate("", (left, right) => left + right), () =>
        {
          for (var i = 0; i < 4; i++)
          {
            GL.Uniform3(Program.GetUniformLocation("lightColors[" + i + "]"), LightColors[i]);
          }
        })
      };
      return baseUniforms.Concat(thisUniforms).ToArray();
    }

    internal override GLDrawCall.GLTextureBind[] GenerateTexturebinds()
    {
      return new[]
      {
        new GLDrawCall.GLTextureBind(DiffuseTexture.GLTexture, TextureTarget.Texture2D, TextureUnit.Texture0),
        new GLDrawCall.GLTextureBind(MetallicRoughnessTexture.GLTexture, TextureTarget.Texture2D, TextureUnit.Texture1)
      };
    }
  }
}