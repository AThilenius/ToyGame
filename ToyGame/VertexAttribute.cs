using System;
using OpenTK.Graphics.OpenGL;

namespace GenericGamedev.OpenTKIntro {
  sealed class VertexAttribute {

    private readonly string name;
    private readonly int size;
    private readonly VertexAttribPointerType type;
    private readonly bool normalize;
    private readonly int stride;
    private readonly int offset;

    public VertexAttribute(string name, int size, VertexAttribPointerType type, bool normalize, int stride, int offset)
    {
      this.name = name;
      this.size = size;
      this.type = type;
      this.normalize = normalize;
      this.stride = stride;
      this.offset = offset;
    }

    public void Set(Material material) {
      // Get location of attribute from shader program
      int index = material.GetShaderAttributeLocation(name);
      // Enable and set attribute
      GL.EnableVertexAttribArray(index);
      GL.VertexAttribPointer(index, size, type, normalize, stride, offset);
    }

  }
}
