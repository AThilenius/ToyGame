using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToyGame
{
  class StandardShader : ShaderProgram
  {

    #region ShaderGLSL
    private ShaderStage vertexShader = new ShaderStage(ShaderType.VertexShader,
      @"#version 420
      uniform mat4 projectionMatrix;
      in vec3 position;
      in vec2 uv0;
      out vec2 f_uv0;

      void main() {
        gl_Position = projectionMatrix * vec4(position, 1.0);
        f_uv0 = vec2(uv0.x, 1 - uv0.y);
      }");

    private ShaderStage fragmentShader = new ShaderStage(ShaderType.FragmentShader,
      @"#version 420
        in vec2 f_uv0;
        out vec4 fragColor;
        layout(binding=0) uniform sampler2D diffuseTexture;
        layout(binding=1) uniform sampler2D normalTexture;
        layout(binding=2) uniform sampler2D roughnessMetallicTexture;

        void main() {
          fragColor = texture(roughnessMetallicTexture, f_uv0);
        }");
    #endregion

    // Uniforms
    public Matrix4 ProjectionMatrix = Matrix4.Identity;

    public StandardShader()
    {
      Compile(new ShaderStage[] { vertexShader, fragmentShader },
          new string[] { "position", "uv0" },
          new string[] { "projectionMatrix" });
    }

    public override void BindUniforms()
    {
      GL.UniformMatrix4(GetUniformLocation("projectionMatrix"), false, ref ProjectionMatrix);
    }

  }
}
