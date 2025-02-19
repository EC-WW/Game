/*********************************************************************
 * file:   heightmap_shader.vert
 * author: evan.gray (evan.gray@digipen.edu)
 * date:   October 15, 2024
 * Copyright � 2024 DigiPen (USA) Corporation.
 *
 * brief:  The vertex shader for the heightmap pipeline
 *********************************************************************/
#version 460

//VERTEX INPUTS -----------------------------------------------------
layout(location = 0) in vec3 position;
layout(location = 1) in vec3 color;
layout(location = 2) in vec3 normal;
layout(location = 3) in vec2 uv;
layout(location = 4) in ivec4 boneIds;
layout(location = 5) in vec4 weights;

//-------------------------------------------------------------------
//VERTEX OUTPUTS ----------------------------------------------------
layout(location = 0) out vec2 TexCoord;

void main() 
{
  //Convert XYZ vertex to XYZW homogeneous coordinate
  gl_Position = vec4(position, 1.0);
  
  //Pass texture coordinate though
  TexCoord = uv;
}
