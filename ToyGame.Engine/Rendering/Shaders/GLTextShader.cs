using OpenTK.Graphics.OpenGL;

namespace ToyGame.Rendering.Shaders
{
  internal class GLTextShader : GLShaderProgram
  {
    public GLTextShader() : base(new[]
    {
      new GLShaderStage(ShaderType.VertexShader, VertexShaderCode),
      new GLShaderStage(ShaderType.FragmentShader, FragmentShaderCode)
    }, new[] {"position", "uv0", "color"}, new string[0])
    {
    }

    #region ShaderGLSL

    private const string VertexShaderCode = @"
        #version 420
        in vec3 position;
        in vec2 uv0;
        in vec4 color;

        out vec2 TexCoords;
        out vec4 Color;

        layout (std140, binding = 0) uniform GlobalUniforms
        {
          uniform mat4 projectionMatrix;
          uniform mat4 viewMatrix;
        };

        uniform mat4 modelMatrix;

        void main()
        {
          TexCoords = uv0;
          Color = color;
          gl_Position =  projectionMatrix * viewMatrix * modelMatrix * vec4(position, 1.0);
        }";

    private const string FragmentShaderCode = @"
        #version 420
        out vec4 FragColor;
        in vec2 TexCoords;
        in vec4 Color;

        // Material parameters
        layout(binding=0) uniform sampler2D glyphAtlas;

        void main()
        {		
          FragColor = vec4(Color.rgb, Color.a * texture(glyphAtlas, TexCoords).r);
        }";

    #endregion
  }
}