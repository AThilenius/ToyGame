using OpenTK.Graphics.OpenGL;
using System;

namespace ToyGame
{
  sealed class VertexArrayObject : IDisposable
  {

    private static VertexArrayObject activeVertexArray;
    private readonly int handle = GL.GenVertexArray();

    public VertexArrayObject(VertexBufferObject[] vertexBuffers, VertexBufferObject indexBuffer, Material material)
    {
      // Bind the VAO
      Bind();
      // Bind the IBO
      indexBuffer.Bind();
      // In tern, bind each VBO, and bind all of it's vertex attributes
      foreach (VertexBufferObject vbo in vertexBuffers)
      {
        vbo.Bind();
        foreach (VertexAttribute attribute in vbo.VertexAttributes)
        {
          attribute.SetIfPresent(material);
        }
      }
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
