using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToyGame
{
  sealed class PBRMaterial : Material
  {

    #region ShaderGLSL
    private ShaderStage vertexShader = new ShaderStage(ShaderType.VertexShader,
      @"#version 130
      uniform mat4 projectionMatrix;
      in vec3 position;
      in vec2 uv0;
      out vec2 f_uv0;

      void main() {
        gl_Position = projectionMatrix * vec4(position, 1.0);
        f_uv0 = vec2(uv0.x, 1 - uv0.y);
      }");

    private ShaderStage fragmentShader = new ShaderStage(ShaderType.FragmentShader,
      @"#version 130
        in vec2 f_uv0;
        out vec4 fragColor;
        uniform sampler2D diffuseTexture;

        void main() {
          fragColor = texture(diffuseTexture, f_uv0);
        }");
    #endregion

    //// Model Matrix
    //private bool modelMatrixDirty = true;
    //private Matrix4 modelMatrix;
    //public Matrix4 ModelMatrix {
    //  set { modelMatrixDirty = true; modelMatrix = value; }
    //  get { return modelMatrix; }
    //}

    //// View Matrix
    //private bool viewMatrixDirty = true;
    //private Matrix4 viewMatrix;
    //public Matrix4 ViewMatrix {
    //  set { viewMatrixDirty = true; viewMatrix = value; }
    //  get { return viewMatrix; }
    //}

    // Projection Matrix
    private bool projectionMatrixDirty = true;
    private Matrix4 projectionMatrix = Matrix4.Identity;
    private int projectionMatrixLocation;
    public Matrix4 ProjectionMatrix
    {
      set { projectionMatrixDirty = true; projectionMatrix = value; }
      get { return projectionMatrix; }
    }

    // Diffuse
    private bool diffuseTextureDirty = true;
    private TextureUnit diffuseTexture = TextureUnit.Texture0;
    private int diffuseTextureLocation;
    public TextureUnit DiffuseTexture
    {
      set { diffuseTextureDirty = true; diffuseTexture = value; }
      get { return diffuseTexture; }
    }

    // DEBUG
    Texture texture = new Texture(@"C:\Users\Alec\thilenius\ToyGame\Assets\Models\AKM\textures\WPNT_AKM_BaseColor.bmp");
    // DEBUG


    public PBRMaterial()
    {
      Program = new ShaderProgram(vertexShader, fragmentShader);
      projectionMatrixLocation = Program.GetUniformLocation("projectionMatrix");
      diffuseTextureLocation = Program.GetUniformLocation("diffuseTexture");
      CacheAttributeLocations("position", "normal", "uv0");
    }

    protected override void BindUniforms()
    {
      // DEBUG
      texture.Bind(TextureUnit.Texture0);
      // DEBUG
      if (projectionMatrixDirty)
      {
        GL.UniformMatrix4(projectionMatrixLocation, false, ref projectionMatrix);
        projectionMatrixDirty = false;
      }
      if (diffuseTextureDirty)
      {
        GL.Uniform1(diffuseTextureLocation, (int) diffuseTexture);
        diffuseTextureDirty = false;
      }
    }

  }
}
