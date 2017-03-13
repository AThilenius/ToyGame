using System;
using System.Collections.Generic;
using OpenTK;
using ProtoBuf;
using ToyGame.Gameplay;
using ToyGame.Rendering;
using ToyGame.Rendering.Shaders;
using ToyGame.Rendering.OpenGL;
using OpenTK.Graphics.OpenGL;

namespace ToyGame.Materials
{
  // A material has a shader program, and exposes all the unifrms as nice properties.
  // Calling Bind makes sure all uniforms are set and the shader is in use.
  public abstract class Material
  {
    internal GLShaderProgram Program;

    internal virtual GLDrawCall.UniformBind[] GenerateUniformBinds (ACamera camera, Matrix4 modelMatrix)
    {
      // Copy the values to local (closures box structs, so make a copy here)
      var projectionMatrix = camera.ProjectionMatrix;
      var viewMatrix = camera.ViewMatrix;
      return new[]
      {
        new GLDrawCall.UniformBind("projectionMatrix-" + projectionMatrix.ToString(), () =>
          GL.UniformMatrix4(Program.GetUniformLocation("projectionMatrix"), false, ref projectionMatrix)),
        new GLDrawCall.UniformBind("viewMatrix-" + viewMatrix.ToString(), () =>
          GL.UniformMatrix4(Program.GetUniformLocation("viewMatrix"), false, ref viewMatrix)),
        new GLDrawCall.UniformBind("modelMatrix-" + modelMatrix.ToString(), () =>
          GL.UniformMatrix4(Program.GetUniformLocation("modelMatrix"), false, ref modelMatrix))
      };
    }

    internal abstract GLDrawCall.GLTextureBind[] GenerateTexturebinds();

    protected abstract void UpdateProgramUniforms();
  }
}