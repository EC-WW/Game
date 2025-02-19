/*********************************************************************
 * file:   heightmap_shader.tesc
 * author: evan.gray (evan.gray@digipen.edu)
 * date:   November 21, 2024
 * Copyright � 2024 DigiPen (USA) Corporation.
 *
 * brief:  The tessilation control shader for the heightmap pipeline
 *********************************************************************/
#version 460

layout (vertices = NL_PATCH_SIZE) out;

//TESSILATION CONTROL INPUTS ----------------------------------------

//Varying input from vertex shader
layout(location = 0) in vec2 TexCoord[];

//-------------------------------------------------------------------
//TESSILATION CONTROL OUTPUTS ---------------------------------------

//Varying output to evaluation shader
layout(location = 0) out vec2 TextureCoord[];

//-------------------------------------------------------------------
//TESSILATION CONTROL UNIFORMS --------------------------------------
layout(set = 1, binding = 0) uniform HeightmapUniforms 
{
  mat4 uModelMatrix;
  mat4 uViewMatrix;
  vec4 uCameraPos;
  uint uTessellationFactor;
  uint uTextureIndex;
};

void main()
{
  // ----------------------------------------------------------------------
  //Pass attributes through to evaluation
  gl_out[gl_InvocationID].gl_Position = gl_in[gl_InvocationID].gl_Position;
  TextureCoord[gl_InvocationID] = TexCoord[gl_InvocationID];

  // ----------------------------------------------------------------------
  //Invocation zero controls tessellation levels for the entire patch
  if (gl_InvocationID == 0)
  {
    gl_TessLevelOuter[0] = uTessellationFactor;
    gl_TessLevelOuter[1] = uTessellationFactor;
    gl_TessLevelOuter[2] = uTessellationFactor;
    gl_TessLevelOuter[3] = uTessellationFactor;

    gl_TessLevelInner[0] = uTessellationFactor;
    gl_TessLevelInner[1] = uTessellationFactor;
  }
}