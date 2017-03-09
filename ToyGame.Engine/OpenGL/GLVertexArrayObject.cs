using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace ToyGame
{
  sealed class GLVertexArrayObject : IDisposable
  {

    private static Dictionary<Tuple<GLMesh, GLShaderProgram>, GLVertexArrayObject> existingBuffers = new Dictionary<Tuple<GLMesh, GLShaderProgram>, GLVertexArrayObject>();
    private static GLVertexArrayObject activeVertexArray;
    private readonly int handle = GL.GenVertexArray();

    private GLVertexArrayObject(GLMesh mesh, GLShaderProgram shaderProgram)
    {
      // Bind the VAO
      Bind();
      // Bind the IBO
      mesh.IndexBuffer.Bind();
      // In tern, bind each VBO, and bind all of it's vertex attributes
      foreach (GLVertexBufferObject vbo in mesh.VertexBuffers)
      {
        vbo.Bind();
        foreach (GLVertexAttribute attribute in vbo.VertexAttributes)
        {
          attribute.SetIfPresent(shaderProgram);
        }
      }
    }

    public static GLVertexArrayObject FromPair(GLMesh mesh, GLShaderProgram glShaderProgram)
    {
      GLVertexArrayObject glVAO;
      var tupple = new Tuple<GLMesh, GLShaderProgram>(mesh, glShaderProgram);
      if (!existingBuffers.TryGetValue(tupple, out glVAO))
      {
        glVAO = new GLVertexArrayObject(mesh, glShaderProgram);
        existingBuffers.Add(tupple, glVAO);
      }
      return glVAO;
    }

    public void Bind()
    {
      if (activeVertexArray != this)
      {
        activeVertexArray = this;
        GL.BindVertexArray(handle);
      }
    }

    public void Dispose()
    {
      GL.DeleteVertexArray(handle);
    }
  }
}
