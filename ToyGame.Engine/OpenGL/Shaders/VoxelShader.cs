using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToyGame
{
  class VoxelShader : GLShaderProgram
  {

    #region ShaderGLSL
    private GLShaderStage vertexShader = new GLShaderStage(ShaderType.VertexShader, @"
        #version 420
        in vec3 position;
        in vec2 uv0;
        in vec3 normal;

        out vec2 TexCoords;
        out vec3 WorldPos;
        out vec3 Normal;

        uniform mat4 projectionMatrix;
        uniform mat4 viewMatrix;
        uniform mat4 modelMatrix;

        void main()
        {
            TexCoords = uv0;
            WorldPos = vec3(modelMatrix * vec4(position, 1.0f));
            //Normal = mat3(modelMatrix) * normal;
            Normal = mat3(transpose(inverse(modelMatrix))) * normal;
            gl_Position =  projectionMatrix * viewMatrix * vec4(WorldPos, 1.0);
        }");

    private GLShaderStage fragmentShader = new GLShaderStage(ShaderType.FragmentShader, @"
        #version 330 core
        out vec4 FragColor;
        in vec2 TexCoords;
        in vec3 WorldPos;
        in vec3 Normal;

        // material parameters
        uniform vec3 albedo;
        uniform float metallic;
        uniform float roughness;
        uniform float ao;

        // lights
        uniform vec3 lightPositions[4];
        uniform vec3 lightColors[4];

        uniform vec3 camPos;
        uniform float exposure;

        const float PI = 3.14159265359;
        // ----------------------------------------------------------------------------
        float DistributionGGX(vec3 N, vec3 H, float roughness)
        {
            float a = roughness*roughness;
            float a2 = a*a;
            float NdotH = max(dot(N, H), 0.0);
            float NdotH2 = NdotH*NdotH;

            float nom   = a2;
            float denom = (NdotH2 * (a2 - 1.0) + 1.0);
            denom = PI * denom * denom;

            return nom / denom;
        }
        // ----------------------------------------------------------------------------
        float GeometrySchlickGGX(float NdotV, float roughness)
        {
            float r = (roughness + 1.0);
            float k = (r*r) / 8.0;

            float nom   = NdotV;
            float denom = NdotV * (1.0 - k) + k;

            return nom / denom;
        }
        // ----------------------------------------------------------------------------
        float GeometrySmith(vec3 N, vec3 V, vec3 L, float roughness)
        {
            float NdotV = max(dot(N, V), 0.0);
            float NdotL = max(dot(N, L), 0.0);
            float ggx2 = GeometrySchlickGGX(NdotV, roughness);
            float ggx1 = GeometrySchlickGGX(NdotL, roughness);

            return ggx1 * ggx2;
        }
        // ----------------------------------------------------------------------------
        vec3 fresnelSchlick(float cosTheta, vec3 F0)
        {
            return F0 + (1.0 - F0) * pow(1.0 - cosTheta, 5.0);
        }
        // ----------------------------------------------------------------------------
        void main()
        {		
            vec3 N = normalize(Normal);
            vec3 V = normalize(camPos - WorldPos);

            // calculate reflectance at normal incidence; if dia-electric (like plastic) use F0 
            // of 0.04 and if it's a metal, use their albedo color as F0 (metallic workflow)    
            vec3 F0 = vec3(0.04); 
            F0 = mix(F0, albedo, metallic);

            // reflectance equation
            vec3 Lo = vec3(0.0);
            for(int i = 0; i < 4; ++i) 
            {
                // calculate per-light radiance
                vec3 L = normalize(lightPositions[i] - WorldPos);
                vec3 H = normalize(V + L);
                float distance = length(lightPositions[i] - WorldPos);
                float attenuation = 1.0 / (distance * distance);
                vec3 radiance = lightColors[i] * attenuation;

                // Cook-Torrance BRDF
                float NDF = DistributionGGX(N, H, roughness);   
                float G   = GeometrySmith(N, V, L, roughness);      
                vec3 F    = fresnelSchlick(max(dot(H, V), 0.0), F0);
                   
                vec3 nominator    = NDF * G * F; 
                float denominator = 4 * max(dot(N, V), 0.0) * max(dot(N, L), 0.0) + 0.001; // 0.001 to prevent divide by zero.
                vec3 brdf = nominator / denominator;
                
                // kS is equal to Fresnel
                vec3 kS = F;
                // for energy conservation, the diffuse and specular light can't
                // be above 1.0 (unless the surface emits light); to preserve this
                // relationship the diffuse component (kD) should equal 1.0 - kS.
                vec3 kD = vec3(1.0) - kS;
                // multiply kD by the inverse metalness such that only non-metals 
                // have diffuse lighting, or a linear blend if partly metal (pure metals
                // have no diffuse light).
                kD *= 1.0 - metallic;	  

                // scale light by NdotL
                float NdotL = max(dot(N, L), 0.0);        

                // add to outgoing radiance Lo
                Lo += (kD * albedo / PI + brdf) * radiance * NdotL;  // note that we already multiplied the BRDF by the Fresnel (kS) so we won't multiply by kS again
            }   
            
            // ambient lighting (note that the next IBL tutorial will replace 
            // this ambient lighting with environment lighting).
            vec3 ambient = vec3(0.03) * albedo * ao;

            vec3 color = ambient + Lo;

            // HDR tonemapping
            color = color / (color + vec3(1.0));
            // gamma correct
            color = pow(color, vec3(1.0/2.2)); 

            FragColor = vec4(color, 1.0);
        }");
    #endregion

    Vector3 Albedo = new Vector3(0.5f, 0.0f, 0.0f);
    float Metalic = 0.9f;
    float Roughness = 0.2f;
    float AO = 1.0f;

    Vector3[] LightPositions = new Vector3[4]
    {
        new Vector3(-10.0f,  10.0f, 10.0f),
        new Vector3( 10.0f,  10.0f, 10.0f),
        new Vector3(-10.0f, -10.0f, 10.0f),
        new Vector3( 10.0f, -10.0f, 10.0f)
    };

    Vector3[] LightColors = new Vector3[4]
    {
        new Vector3(10000.0f, 10000.0f, 10000.0f),
        new Vector3(300.0f, 300.0f, 300.0f),
        new Vector3(300.0f, 300.0f, 300.0f),
        new Vector3(300.0f, 300.0f, 300.0f)
    };

    Vector3 CameraPosition = Vector3.Zero;
    float Exposure = 1.0f;

    public VoxelShader()
    {
      Compile(new GLShaderStage[] { vertexShader, fragmentShader },
          new string[] { "position", "uv0", "normal" },
          new string[] { "albedo", "metallic", "roughness", "ao", "camPos", "exposure",
          "lightPositions[0]",
          "lightPositions[1]",
          "lightPositions[2]",
          "lightPositions[3]",
          "lightColors[0]",
          "lightColors[1]",
          "lightColors[2]",
          "lightColors[3]" });
    }

    public override void BindUniforms()
    {
      base.BindUniforms();

      GL.Uniform1(GetUniformLocation("metallic"), Metalic);
      GL.Uniform1(GetUniformLocation("roughness"), Roughness);
      GL.Uniform1(GetUniformLocation("ao"), AO);
      GL.Uniform1(GetUniformLocation("exposure"), Exposure);

      GL.Uniform3(GetUniformLocation("albedo"), Albedo);
      GL.Uniform3(GetUniformLocation("exposure"), CameraPosition);

      for (int i = 0; i < 4; i++)
      {
        GL.Uniform3(GetUniformLocation("lightPositions[" + i + "]"), LightPositions[i]);
        GL.Uniform3(GetUniformLocation("lightColors[" + i + "]"), LightColors[i]);
      }
    }

  }
}
