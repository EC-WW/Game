/*********************************************************************
 * file:   main_shader.frag
 * author: aditya.prakash (aditya.prakash@digpen.edu) and evan.gray (evan.gray@digipen.edu)
 * date:   October 15, 2024
 * Copyright 2024 DigiPen (USA) Corporation.
 *
 * brief:  The fragment shader!
 *********************************************************************/

#version 460

// Extensions
#extension GL_EXT_nonuniform_qualifier : require

// FRAG INPUTS -----------------------------------------------------
layout(location = 0) in vec3 fragColor;
layout(location = 1) in vec2 fragTexCoord;
layout(location = 2) in vec3 fragNormal;
layout(location = 3) flat in uint instanceIndex;

// -------------------------------------------------------------------
// FRAG OUTPUTS ----------------------------------------------------
layout(location = 0) out vec4 outColor;

#include "instance_set.glsl"

// -------------------------------------------------------------------
// Textures ----------------------------------------------------------
layout(set = 2, binding = 1) uniform sampler2D uTextures[NL_MAX_TEXTURES];

// -------------------------------------------------------------------
// Calculates the screen-space pixel range of the glyph
float screenPxRange() 
{
    uint fragTextureIndex = particleInstanceData.instances[instanceIndex].textureIndex;

    const float pxRange = 2.0;

    vec2 unitRange = vec2(pxRange)/vec2(textureSize(uTextures[fragTextureIndex], 0));

    vec2 screenTexSize = vec2(1.0)/fwidth(fragTexCoord);

    return max(0.5*dot(unitRange, screenTexSize), 1.0);
}

// Calculates the median of three values
float median(float r, float g, float b) 
{
    return max(min(r, g), min(max(r, g), b));
}

// Constants
const float ALPHA_THRESHOLD = 0.5; // Sharp cutoff for alpha testing
const vec4 OUTLINE_COLOR = vec4(0.0, 0.0, 1.0, 1.0); // Blue outline
const float OUTLINE_WIDTH = 0.1; // Width of the outline

void main()
{
    uint fragTextureIndex = particleInstanceData.instances[instanceIndex].textureIndex;

    if (fragTextureIndex == NL_NO_TEXTURES) 
    {
        // No textures, use vertex colors instead!
        outColor = vec4(fragColor, 1.0);
    }
    else if (particleInstanceData.instances[instanceIndex].charIndex == NL_INVALID_FONT_INDEX)
    {
        // Directly use the texture color
		outColor = texture(uTextures[fragTextureIndex], fragTexCoord);
    }
    else 
    {
        outColor = vec4(0, 0, 0, 1);
    }
}