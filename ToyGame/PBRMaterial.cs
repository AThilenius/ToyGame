using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericGamedev.OpenTKIntro {
  sealed class PBRMaterial : Material {

    #region ShaderGLSL
    private Shader vertexShader = new Shader(ShaderType.VertexShader,
      @"#version 130
      uniform mat4 projectionMatrix;
      in vec3 position;
      in vec3 normal;
      out vec4 color;

      void main() {
        gl_Position = projectionMatrix * vec4(position, 1.0);
        color = vec4(normal.xyz, 1);
      }");

    private Shader fragmentShader = new Shader(ShaderType.FragmentShader,
      @"#version 130
        in vec4 color;
        out vec4 fragColor;

        void main() {
          fragColor = color;
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
    private bool projectionMatrixDirty;
    private Matrix4 projectionMatrix;
    private int projectionMatrixLocation;
    public Matrix4 ProjectionMatrix {
      set { projectionMatrixDirty = true; projectionMatrix = value; }
      get { return projectionMatrix; }
    }

    public PBRMaterial() {
      Program = new ShaderProgram(vertexShader, fragmentShader);
      projectionMatrixLocation = Program.GetUniformLocation("projectionMatrix");
      CacheAttributeLocations("position", "normal");
    }

    protected override void BindUniforms() {
      if (projectionMatrixDirty) {
        GL.UniformMatrix4(projectionMatrixLocation, false, ref projectionMatrix);
        projectionMatrixDirty = false;
      }
    }

  }
}
