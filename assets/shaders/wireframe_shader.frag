/*********************************************************************
 * file:   wireframe_shader.frag
 * author: evan.gray (evan.gray@digipen.edu)
 * date:   October 29, 2024
 * Copyright � 2024 DigiPen (USA) Corporation.
 *
 * brief:  The fragment shader for the wireframe pipeline
 *********************************************************************/
#version 460

//FRAG OUTPUTS ----------------------------------------------------
layout(location = 0) out vec4 outColor;

void main()
{
  outColor = vec4(0, 1, 0, 1);
}