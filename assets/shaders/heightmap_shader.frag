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
layout(location = 0) in vec2 TexCoord;
layout(location = 1) in float normalizedYPosition;

//FRAG OUTPUTS ----------------------------------------------------
layout(location = 0) out vec4 outColor;

//-------------------------------------------------------------------
//TESSILATION CONTROL UNIFORMS --------------------------------------
layout(set = 1, binding = 0) uniform HeightmapUniforms 
{
  mat4 uModelMatrix[NL_MAX_HEIGHTMAP_COUNT];
  vec4 uBaseColor[NL_MAX_HEIGHTMAP_COUNT];
  vec4 uPeakColor[NL_MAX_HEIGHTMAP_COUNT];
  vec4 uVisibleBase[NL_MAX_HEIGHTMAP_COUNT];
  mat4 uViewMatrix;
  vec4 uCameraPos;
};

layout(push_constant) uniform PushConstants 
{
	uint uHeightmapIndex;
};

void main()
{
  //Check if fragment is near the edge (within threshold)
  float edgeThreshold = 0.01;
  if (TexCoord.x < edgeThreshold || TexCoord.x > 1.0 - edgeThreshold ||
      TexCoord.y < edgeThreshold || TexCoord.y > 1.0 - edgeThreshold) 
  {
    discard;
  }

  //Discard height 0 if the base is not visible
  if(normalizedYPosition == 0 && uVisibleBase[uHeightmapIndex].x == 0)
  {
    discard;
  }

  //Lerp between base and peak color based off of height
  outColor = vec4(uBaseColor[uHeightmapIndex].x + (uPeakColor[uHeightmapIndex].x - uBaseColor[uHeightmapIndex].x) * normalizedYPosition,
                  uBaseColor[uHeightmapIndex].y + (uPeakColor[uHeightmapIndex].y - uBaseColor[uHeightmapIndex].y) * normalizedYPosition,
                  uBaseColor[uHeightmapIndex].z + (uPeakColor[uHeightmapIndex].z - uBaseColor[uHeightmapIndex].z) * normalizedYPosition, 
                  1);
}