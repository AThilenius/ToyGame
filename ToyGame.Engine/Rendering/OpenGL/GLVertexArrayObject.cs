using System.Diagnostics;
using System.Threading;
using OpenTK.Graphics.OpenGL;
using ToyGame.Rendering.Shaders;

namespace ToyGame.Rendering.OpenGL
{
  internal sealed class GLVertexArrayObject : GLResource<GLVertexArrayObject>
  {
    #region Fields / Properties

    public readonly GLMesh Mesh;
    public readonly GLShaderProgram ShaderProgram;

    #endregion

    public GLVertexArrayObject(GLMesh mesh, GLShaderProgram shaderProgram)
      : base(GL.GenVertexArray, GL.DeleteVertexArray)
    {
      Mesh = mesh;
      ShaderProgram = shaderProgram;
    }

    /// <summary>
    ///   Must be called on the GPU thread
    /// </summary>
    public void Bind()
    {
      Debug.Assert(Thread.CurrentThread.Name == RenderContext.GpuThreadName);
      Debug.Assert(GLHandle != -1);
      GL.BindVertexArray(GLHandle);
    }

    protected override void LoadToGpu()
    {
      Bind();
      Mesh.IndexBuffer.Bind();
      // In tern, bind each VBO, and bind all of it's vertex attributes
      foreach (var vbo in Mesh.VertexBuffers)
      {
        vbo.Bind();
        foreach (var attribute in vbo.VertexAttributes)
        {
          attribute.SetIfPresent(ShaderProgram);
        }
      }
    }
  }
}