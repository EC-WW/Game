/*********************************************************************
 * file:   main_shader.vert
 * author: aditya.prakash (aditya.prakash@digpen.edu) and evan.gray (evan.gray@digipen.edu)
 * date:   October 15, 2024
 * Copyright © 2024 DigiPen (USA) Corporation.
 *
 * brief:  The vertex shader!
 *********************************************************************/

#version 460

// Extensions
#extension GL_EXT_nonuniform_qualifier : require

// VERTEX INPUTS -----------------------------------------------------
layout(location = 0) in vec3 position;
layout(location = 1) in vec3 color;
layout(location = 2) in vec3 normal;
layout(location = 3) in vec2 uv;
layout(location = 4) in ivec4 boneIds;
layout(location = 5) in vec4 weights;

// -------------------------------------------------------------------
// VERTEX OUTPUTS ----------------------------------------------------
layout(location = 0) out vec3 fragColor;
layout(location = 1) out vec2 texCoord;
layout(location = 2) out vec3 fragNormal;
layout(location = 3) flat out uint outInstanceIndex;

#include "camera_set.glsl"
#include "instance_set.glsl"

// -------------------------------------------------------------------
// Glyphs --------------------------------------------------------
layout(set = 2, binding = 0) buffer readonly GlyphDataBuffer {
    vec4 glyphs[NL_MAX_GLYPHS];
} glyphData;

// -------------------------------------------------------------------
// Bones! ------------------------------------------------------------
struct AnimationData {
    mat4 finalBonesMatrices[NL_MAX_BONES];
};

layout(set = 2, binding = 2) buffer readonly BoneBuffer {
	AnimationData bones[NL_MAX_MODELS];
} animationData;

// Global light data
const float cAmbient = 0.02;

void main() 
{
    // Fetch the instance-specific data
    int instanceIndex = gl_InstanceIndex;

    mat4 model = instanceData.instances[instanceIndex].modelMatrix;
    mat3 normalMat = transpose(inverse(mat3(model)));

    uint animationIndex = instanceData.instances[instanceIndex].animationIndex;

    vec4 totalPosition = vec4(0.0f);
    vec3 totalNormal = vec3(0.0f);

    if (animationIndex == NL_INVALID_ANIMATION_INDEX) 
    {
        // No animations
		totalPosition = vec4(position, 1.0);
        totalNormal = normal;
    }
    else 
	{
        // Yes animations
        bool validBoneFound = false;

        for(int i = 0 ; i < NL_MAX_BONE_INFLUENCE ; i++)
        {
            if(boneIds[i] == -1) continue;

            if(boneIds[i] >= NL_MAX_BONES) 
            {
                continue;
            }

            vec4 localPosition = animationData.bones[animationIndex].finalBonesMatrices[boneIds[i]] * vec4(position,1.0f);
            totalPosition += localPosition * weights[i];

            vec3 localNormal = mat3(animationData.bones[animationIndex].finalBonesMatrices[boneIds[i]]) * normal;
            totalNormal += localNormal * weights[i];

            validBoneFound = true;
        }

        if (!validBoneFound || totalPosition == vec4(0.0)) {
		    totalPosition = vec4(position, 1.0);
	    }
	}

    // Transform the vertex position to clip space
    uint16_t flags = instanceData.instances[instanceIndex].flags;
    bool diagetic = (flags & 1) != 0;
    bool billboard = (flags & 2) != 0;

    // Extract camera's right and up vectors
    if (billboard) 
    {
        vec3 right = normalize(vec3(uniforms.view[0].x, uniforms.view[1].x, uniforms.view[2].x));
        vec3 up = normalize(vec3(uniforms.view[0].y, uniforms.view[1].y, uniforms.view[2].y));
    
        // Compute billboard position in world space
        vec3 billboardPosition = instanceData.instances[instanceIndex].modelMatrix[3].xyz;
        totalPosition = vec4(billboardPosition + position.x * right + position.y * up, 1.0);

        if (diagetic)
        {
            gl_Position = uniforms.projection * uniforms.view * totalPosition;
        }
        else
        {    
            gl_Position = uniforms.projection * totalPosition;
        }
    }
    else if (diagetic) 
    {
        gl_Position = uniforms.projection * uniforms.view * model * totalPosition;
    }
    else
    {    
        gl_Position = uniforms.projection * model * totalPosition;
    }

    // Calculate the normal in world space
    vec3 normalWorldSpace = normalize(normalMat * totalNormal);

    // Compute light intensity
    float lightIntensity = cAmbient + max(dot(normalWorldSpace, uniforms.directionToLight), 0.0);

    // Pass the color and texture coordinates to the fragment shader
    fragColor = lightIntensity * color;

    if (instanceData.instances[instanceIndex].charIndex != NL_INVALID_FONT_INDEX) 
    {
        uint charIndex = instanceData.instances[instanceIndex].charIndex - 32;
    
        vec4 texInfo = glyphData.glyphs[charIndex];
        
        float epsilon = 0.0005;
        float l = texInfo.x + epsilon; 
        float b = texInfo.w + epsilon; // flipped w/ t
        float r = texInfo.z - epsilon; 
        float t = texInfo.y - epsilon; // flipped w/ b
    
        if (uv.x == 0 && uv.y == 0)      texCoord = vec2(l, t);
        else if (uv.x == 1 && uv.y == 0) texCoord = vec2(r, t);
        else if (uv.x == 1 && uv.y == 1) texCoord = vec2(r, b);
        else if (uv.x == 0 && uv.y == 1) texCoord = vec2(l, b);
    }
    else {
        texCoord = uv;
    }

    fragNormal = normalWorldSpace;
    outInstanceIndex = instanceIndex;
}
