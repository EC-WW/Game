/*********************************************************************
 * file:   camera_set.glsl
 * author: aditya.prakash (aditya.prakash@digpen.edu) and evan.gray (evan.gray@digipen.edu)
 * date:   November 21, 2024
 * Copyright  2024 DigiPen (USA) Corporation.
 *
 * brief:  Matrix uniform set!
 *********************************************************************/

// -------------------------------------------------------------------
// Camera -----------------------------------------------------------
layout(set = 0, binding = 0) uniform MatrixUniforms {
    mat4 projectionView;
    mat4 projection;
    mat4 view;
    vec3 directionToLight;
} uniforms;

layout(set = 0, binding = 1) uniform ParticleMatrixUniforms {
  mat4 projectionView;
  mat4 projection;
  mat4 view;
  vec3 directionToLight;
} particleUniforms;