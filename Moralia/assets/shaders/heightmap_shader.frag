/*********************************************************************
 * file:   heightmap_shader.frag
 * author: evan.gray (evan.gray@digipen.edu)
 * date:   October 29, 2024
 * Copyright � 2024 DigiPen (USA) Corporation.
 *
 * brief:  The fragment shader for the heightmap pipeline
 *********************************************************************/
#version 460

//FRAG INPUTS -----------------------------------------------------
layout(location = 0) in float normalizedYPosition;

//FRAG OUTPUTS ----------------------------------------------------
layout(location = 0) out vec4 outColor;

void main()
{
  //Color based off of height
  outColor = vec4(normalizedYPosition, normalizedYPosition, normalizedYPosition, 1);
}