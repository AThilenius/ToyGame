using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToyGame
{
  // A material has a shader program, and exposes all the unifrms as nice properties.
  // Calling Bind makes sure all uniforms are set and the shader is in use.
  abstract class Material
  {

    public ShaderProgram Program { get; protected set; }

    protected static Material currentMaterial;

    public void Bind()
    {
      if (currentMaterial != this)
      {
        Program.Use();
        UpdateProgramUniforms();
        Program.BindUniforms();
        currentMaterial = this;
      }
    }

    protected abstract void UpdateProgramUniforms();

  }
}
