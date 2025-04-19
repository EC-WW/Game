/*********************************************************************
 * file:   wireframe_shader.vert
 * author: evan.gray (evan.gray@digipen.edu)
 * date:   October 29, 2024
 * Copyright � 2024 DigiPen (USA) Corporation.
 *
 * brief:  The vertex shader for the wireframe pipeline
 *********************************************************************/
#version 460

//VERTEX INPUTS -----------------------------------------------------
layout(location = 0) in vec3 position;
layout(location = 1) in vec3 color;
layout(location = 2) in vec3 normal;
layout(location = 3) in vec2 uv;
layout(location = 4) in ivec4 boneIds;
layout(location = 5) in vec4 weights;

#include "include/camera_set.glsl"
#include "include/instance_set.glsl"

void main() 
{
  //Transform the vertex position to clip space
  gl_Position = uniforms.projectionView * instanceData.instances[gl_InstanceIndex].modelMatrix * vec4(position, 1.0);
}