﻿using OpenTK.Graphics.OpenGL;

namespace ToyGame.Rendering.Shaders
{
  internal class GLVoxelShader : GLShaderProgram
  {
    public GLVoxelShader(RenderContext renderContext) : base(renderContext, new[]
    {
      new GLShaderStage(renderContext, ShaderType.VertexShader, VertexShaderCode),
      new GLShaderStage(renderContext, ShaderType.FragmentShader, FragmentShaderCode)
    }, new[] {"position", "uv0", "normal"},
      new[]
      {
        "camPos", "lightPositions[0]", "lightPositions[1]", "lightPositions[2]", "lightPositions[3]",
        "lightColors[0]", "lightColors[1]", "lightColors[2]", "lightColors[3]"
      })
    {
    }

    #region ShaderGLSL

    private const string VertexShaderCode = @"
        #version 420
        in vec3 position;
        in vec2 uv0;
        in vec3 normal;

        out vec2 TexCoords;
        out vec3 WorldPos;
        out vec3 Normal;

        layout (std140, binding = 0) uniform GlobalUniforms
        {
          uniform mat4 projectionMatrix;
          uniform mat4 viewMatrix;
        };

        uniform mat4 modelMatrix;

        void main()
        {
            TexCoords = uv0;
            WorldPos = vec3(modelMatrix * vec4(position, 1.0f));
            //Normal = mat3(modelMatrix) * normal;
            Normal = mat3(transpose(inverse(modelMatrix))) * normal;
            gl_Position =  projectionMatrix * viewMatrix * vec4(WorldPos, 1.0);
        }";

    private const string FragmentShaderCode = @"
        #version 420 core
        out vec4 FragColor;
        in vec2 TexCoords;
        in vec3 WorldPos;
        in vec3 Normal;

        // Material parameters
        layout(binding=0) uniform sampler2D albedoMap;
        layout(binding=1) uniform sampler2D metallicRoughnessMap;

        // Lights
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
            // For now, don't gamma correct the albedo. Looks more cartoony then.
            // vec3 albedo = pow(texture(albedoMap, TexCoords).rgb, vec3(2.2));
            vec3 albedo = texture(albedoMap, TexCoords).rgb;
            vec2 metallicRoughness = texture(metallicRoughnessMap, TexCoords).rg;
            float roughness = metallicRoughness.r;
            float metallic  = metallicRoughness.g;

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
            vec3 ambient = vec3(0.03) * albedo;

            vec3 color = ambient + Lo;

            // HDR tonemapping
            color = color / (color + vec3(1.0));
            // gamma correct
            color = pow(color, vec3(1.0/2.2)); 

            FragColor = vec4(color, 1.0);
        }";

    #endregion
  }
}