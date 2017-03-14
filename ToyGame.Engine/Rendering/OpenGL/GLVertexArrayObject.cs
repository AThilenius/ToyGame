using System;
using System.Diagnostics;
using System.Threading;
using OpenTK.Graphics.OpenGL;
using ToyGame.Rendering.Shaders;

namespace ToyGame.Rendering.OpenGL
{
  internal sealed class GLVertexArrayObject : IDisposable, IComparable<GLVertexArrayObject>
  {
    #region Fields / Properties

    //private static readonly Dictionary<Tuple<GLMesh, GLShaderProgram>, GLVertexArrayObject> ExistingBuffers =
    //  new Dictionary<Tuple<GLMesh, GLShaderProgram>, GLVertexArrayObject>();

    private readonly GLHandle _glHandle;

    #endregion

    public GLVertexArrayObject(RenderCore renderCore, GLMesh mesh, GLShaderProgram shaderProgram)
    {
      _glHandle = new GLHandle {RenderCore = renderCore};
      renderCore.AddResourceLoadAction(() =>
      {
        _glHandle.Handle = GL.GenVertexArray();
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
      });
    }

    public int CompareTo(GLVertexArrayObject other)
    {
      return _glHandle.CompareTo(other._glHandle);
    }

    public void Dispose()
    {
      _glHandle.RenderCore.AddResourceLoadAction(() => GL.DeleteVertexArray(_glHandle.Handle));
    }

    /// <summary>
    ///   Must be called on the GPU thread
    /// </summary>
    public void Bind()
    {
      Debug.Assert(Thread.CurrentThread.Name == RenderCore.GpuThreadName);
      Debug.Assert(_glHandle.Handle != -1);
      GL.BindVertexArray(_glHandle.Handle);
    }
  }
}