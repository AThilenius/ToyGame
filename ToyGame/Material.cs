using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericGamedev.OpenTKIntro {
  // A material has a shader program, and exposes all the unifrms as nice properties.
  // Calling Bind makes sure all uniforms are set and the shader is in use.
  abstract class Material {

    public ShaderProgram Program;

    protected static Material currentMaterial;

    private Dictionary<string, int> attributeNamesToLocations = new Dictionary<string, int>();

    public void Bind() {
      if (currentMaterial != this) {
        Program.Use();
        BindUniforms();
        currentMaterial = this;
      }
    }

    public int GetShaderAttributeLocation(string name)
    {
      return attributeNamesToLocations[name];
    }

    protected abstract void BindUniforms();
    
    protected void CacheAttributeLocations(params string[] names)
    {
      Program.Use();
      foreach(string name in names)
      {
        attributeNamesToLocations.Add(name, Program.GetAttributeLocation(name));
      }
    }

  }
}
