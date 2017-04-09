using OpenTK.Graphics.OpenGL;

namespace ToyGame.Rendering.Shaders
{
  internal class  GLGuiShader: GLShaderProgram
  {
    public GLGuiShader() : base(new[]
    {
      new GLShaderStage(ShaderType.VertexShader, VertexShaderCode),
      new GLShaderStage(ShaderType.FragmentShader, FragmentShaderCode)
    }, new[] {"position"}, new string[0])
    {
    }

    #region ShaderGLSL

    private const string VertexShaderCode = @"
        #version 420
        in vec3 position;

        out vec2 TexCoords;

        void main()
        {
          TexCoords = vec2((position.x + 1.0) / 2.0, 1.0 - ((position.y + 1.0) / 2.0));
          // Transform Screen Space to UV coordinates
          gl_Position =  vec4(position, 1.0);
        }";

    private const string FragmentShaderCode = @"
        #version 420
        in vec2 TexCoords;

        out vec4 FragColor;

        // Bitmap image to draw on full-size quad
        layout(binding=0) uniform sampler2D bitmap;

        void main()
        {		
          FragColor = texture(bitmap, TexCoords);
        }";

    #endregion
  }
}