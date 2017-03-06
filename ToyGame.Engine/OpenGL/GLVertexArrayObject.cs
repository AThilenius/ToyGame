using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;

namespace ToyGame
{
  sealed class GLVertexArrayObject : IDisposable
  {

    private static Dictionary<Tuple<MeshResource, GLShaderProgram>, GLVertexArrayObject> existingBuffers = new Dictionary<Tuple<MeshResource, GLShaderProgram>, GLVertexArrayObject>();
    private static GLVertexArrayObject activeVertexArray;
    private readonly int handle = GL.GenVertexArray();

    private GLVertexArrayObject(MeshResource meshResource, GLShaderProgram shaderProgram)
    {
      // Bind the VAO
      Bind();
      // Bind the IBO
      meshResource.GLGeometry.IndexBuffer.Bind();
      // In tern, bind each VBO, and bind all of it's vertex attributes
      foreach (GLVertexBufferObject vbo in meshResource.GLGeometry.VertexBuffers)
      {
        vbo.Bind();
        foreach (GLVertexAttribute attribute in vbo.VertexAttributes)
        {
          attribute.SetIfPresent(shaderProgram);
        }
      }
    }

    public static GLVertexArrayObject FromPair(MeshResource meshResource, GLShaderProgram glShaderProgram)
    {
      GLVertexArrayObject glVAO;
      var tupple = new Tuple<MeshResource, GLShaderProgram>(meshResource, glShaderProgram);
      if (!existingBuffers.TryGetValue(tupple, out glVAO))
      {
        glVAO = new GLVertexArrayObject(meshResource, glShaderProgram);
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
