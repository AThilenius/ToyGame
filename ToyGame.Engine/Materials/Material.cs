using System;
using OpenTK;
using ProtoBuf;
using ToyGame.Gameplay;
using ToyGame.OpenGL.Shaders;

namespace ToyGame.Materials
{
  // A material has a shader program, and exposes all the unifrms as nice properties.
  // Calling Bind makes sure all uniforms are set and the shader is in use.
  public abstract class Material
  {
    internal GLShaderProgram Program;

    public void Bind(ACamera camera, Matrix4 modelMatrix)
    {
      Program.Use();
      Program.ProjectionMatrix = camera.ProjectionMatrix;
      Program.ViewMatrix = camera.ViewMatrix;
      Program.ModelMatrix = modelMatrix;
      UpdateProgramUniforms();
      Program.BindUniforms();
    }

    protected abstract void UpdateProgramUniforms();
  }
}