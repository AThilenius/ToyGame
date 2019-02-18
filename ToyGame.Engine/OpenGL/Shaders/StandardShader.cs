﻿using OpenTK.Graphics.OpenGL;

namespace ToyGame.OpenGL.Shaders
{
  internal class StandardShader : GLShaderProgram
  {
    // Uniforms

    public StandardShader()
    {
      Compile(new[] {_vertexShader, _fragmentShader},
        new[] {"position", "uv0", "normal"},
        new string[0]);
    }

    #region ShaderGLSL

    private readonly GLShaderStage _vertexShader = new GLShaderStage(ShaderType.VertexShader,
      @"#version 420
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
            Normal = mat3(modelMatrix) * normal;   
            gl_Position =  projectionMatrix * viewMatrix * vec4(WorldPos, 1.0);
        }");

    private readonly GLShaderStage _fragmentShader = new GLShaderStage(ShaderType.FragmentShader,
      @"#version 420
        out vec4 FragColor;
        in vec2 TexCoords;
        in vec3 WorldPos;
        in vec3 Normal;
        in mat3 TBN;

        // material parameters
        layout(binding=0) uniform sampler2D albedoMap;
        layout(binding=1) uniform sampler2D normalMap;
        layout(binding=2) uniform sampler2D roughnessMetallicMap;
        layout(binding=3) uniform sampler2D aoMap;

        // lights
// DEBUG
        //uniform vec3 lightPositions[4];
        //uniform vec3 lightColors[4];
        //uniform vec3 camPos;

        const vec3 lightPositions[4] = vec3[4](vec3(0, 10, 10), vec3(0, 0, 0), vec3(0, 0, 0), vec3(0, 0, 0));
        const vec3 lightColors[4] = vec3[4](vec3(150, 150, 150), vec3(0, 0, 0), vec3(0, 0, 0), vec3(0, 0, 0));
        const vec3 camPos = vec3(0, 0, 0);
// DEBUG

        const float PI = 3.14159265359;
        // ----------------------------------------------------------------------------
        // Easy trick to get tangent-normals to world-space to keep PBR code simplified.
        // Don't worry if you don't get what's going on; you generally want to do normal 
        // mapping the usual way for performance anways; I do plan make a note of this 
        // technique somewhere later in the normal mapping tutorial.
        vec3 getNormalFromMap()
        {
            vec3 tangentNormal = texture(normalMap, TexCoords).xyz * 2.0 - 1.0;

            vec3 Q1  = dFdx(WorldPos);
            vec3 Q2  = dFdy(WorldPos);
            vec2 st1 = dFdx(TexCoords);
            vec2 st2 = dFdy(TexCoords);

            vec3 N   = normalize(Normal);
            vec3 T  = normalize(Q1*st2.t - Q2*st1.t);
            vec3 B  = -normalize(cross(N, T));
            mat3 TBN = mat3(T, B, N);

            return normalize(TBN * tangentNormal);
        }
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
            vec3 albedo     = pow(texture(albedoMap, TexCoords).rgb, vec3(2.2));
            float metallic  = texture(roughnessMetallicMap, TexCoords).r;
            float roughness = texture(roughnessMetallicMap, TexCoords).g;
            float ao        = texture(aoMap, TexCoords).r;

            vec3 N = getNormalFromMap();
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
  }
}