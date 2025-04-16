/*********************************************************************
 * file:   main_shader.vert
 * author: aditya.prakash (aditya.prakash@digpen.edu) and evan.gray (evan.gray@digipen.edu)
 * date:   October 15, 2024
 * Copyright © 2024 DigiPen (USA) Corporation.
 *
 * brief:  The vertex shader! Specialized version of main.vert for particles
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
// INSTANCE INPUTS ---------------------------------------------------
layout(location = 6) in mat4 transform;
layout(location = 10) in vec4 textureIndexTint;

// -------------------------------------------------------------------
// VERTEX OUTPUTS ----------------------------------------------------
layout(location = 0) out vec2 texCoord;
layout(location = 1) flat out vec4 fragTextureIndexTint;

void main()
{
    gl_Position = transform * vec4(position, 1.0);

    texCoord = uv;
    fragTextureIndexTint = textureIndexTint;
}
