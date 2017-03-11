using System;
using System.Collections.Generic;
using OpenTK.Graphics.OpenGL;
using ToyGame.OpenGL.Shaders;

namespace ToyGame.OpenGL
{
  internal sealed class GLVertexArrayObject : IDisposable
  {
    private static readonly Dictionary<Tuple<GLMesh, GLShaderProgram>, GLVertexArrayObject> ExistingBuffers =
      new Dictionary<Tuple<GLMesh, GLShaderProgram>, GLVertexArrayObject>();

    private static GLVertexArrayObject _activeVertexArray;
    private readonly int _handle = GL.GenVertexArray();

    private GLVertexArrayObject(GLMesh mesh, GLShaderProgram shaderProgram)
    {
      // Bind the VAO
      Bind();
      // Bind the IBO
      mesh.IndexBuffer.Bind();
      // In tern, bind each VBO, and bind all of it's vertex attributes
      foreach (var vbo in mesh.VertexBuffers)
      {
        vbo.Bind();
        foreach (var attribute in vbo.VertexAttributes)
        {
          attribute.SetIfPresent(shaderProgram);
        }
      }
    }

    public void Dispose()
    {
      GL.DeleteVertexArray(_handle);
    }

    public static GLVertexArrayObject FromPair(GLMesh mesh, GLShaderProgram glShaderProgram)
    {
      GLVertexArrayObject glVao;
      var tupple = new Tuple<GLMesh, GLShaderProgram>(mesh, glShaderProgram);
      if (!ExistingBuffers.TryGetValue(tupple, out glVao))
      {
        glVao = new GLVertexArrayObject(mesh, glShaderProgram);
        ExistingBuffers.Add(tupple, glVao);
      }
      return glVao;
    }

    public void Bind()
    {
      if (_activeVertexArray != this)
      {
        _activeVertexArray = this;
        GL.BindVertexArray(_handle);
      }
    }
  }
}