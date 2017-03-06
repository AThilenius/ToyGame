using System;
using OpenTK.Graphics.OpenGL;

namespace ToyGame
{
  sealed class GLVertexAttribute
  {

    private readonly string name;
    private readonly int size;
    private readonly VertexAttribPointerType type;
    private readonly bool normalize;
    private readonly int stride;
    private readonly int offset;

    public GLVertexAttribute(string name, int size, VertexAttribPointerType type, bool normalize, int stride, int offset)
    {
      this.name = name;
      this.size = size;
      this.type = type;
      this.normalize = normalize;
      this.stride = stride;
      this.offset = offset;
    }

    public void SetIfPresent(GLShaderProgram shaderProgram)
    {
      // Get location of attribute from shader program
      int index = shaderProgram.GetAttributeLocation(name);
      if (index >= 0)
      {
        // Enable and set attribute
        GL.EnableVertexAttribArray(index);
        GL.VertexAttribPointer(index, size, type, normalize, stride, offset);
      }
    }

  }
}
