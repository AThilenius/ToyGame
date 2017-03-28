using OpenTK.Graphics.OpenGL;

namespace ToyGame.Rendering.Shaders
{
  internal class  GLGuiShader: GLShaderProgram
  {
    public GLGuiShader() : base(new[]
    {
      new GLShaderStage(ShaderType.VertexShader, VertexShaderCode),
      new GLShaderStage(ShaderType.FragmentShader, FragmentShaderCode)
    }, new[] {"position", "uv0"}, new string[0])
    {
    }

    #region ShaderGLSL

    private const string VertexShaderCode = @"
        #version 420
        in vec3 position;
        in vec2 uv0;

        out vec2 TexCoords;

        void main()
        {
          TexCoords = uv0;
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