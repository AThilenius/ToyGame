using OpenTK;
using OpenTK.Graphics.OpenGL;
using ToyGame.Rendering.OpenGL;
using ToyGame.Rendering.Shaders;

namespace ToyGame.Materials
{
  // A material has a shader program, and exposes all the unifrms as nice properties.
  // Calling Bind makes sure all uniforms are set and the shader is in use.
  public abstract class Material
  {
    #region Fields / Properties

    internal GLShaderProgram Program;

    #endregion

    internal virtual GLDrawCall.UniformBind[] GenerateUniformBinds(Matrix4 modelMatrix)
    {
      // Copy the values to local (closures box structs, so make a copy here)
      return new[]
      {
        new GLDrawCall.UniformBind("modelMatrix-" + modelMatrix, () =>
          GL.UniformMatrix4(Program.GetUniformLocation("modelMatrix"), false, ref modelMatrix))
      };
    }

    internal abstract GLDrawCall.GLTextureBind[] GenerateTexturebinds();
  }
}