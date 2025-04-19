/*********************************************************************
 * file:   instance_set.glsl
 * author: aditya.prakash (aditya.prakash@digpen.edu)
 * date:   November 21, 2024
 * Copyright  2024 DigiPen (USA) Corporation.
 *
 * brief:  Instance set!
 *********************************************************************/

#extension GL_EXT_shader_explicit_arithmetic_types : require
#extension GL_EXT_shader_16bit_storage : require

// -------------------------------------------------------------------
// Instances ---------------------------------------------------------
struct Instance {
  mat4 modelMatrix;    // 64 bytes
  vec4 boundingSphere; // 16 bytes (center, radius)
  vec4 tintColor;      // 16 bytes
  uint textureIndex;   // 4  bytes
  uint animationIndex; // 4  bytes
  uint meshIndex;      // 4  bytes
  uint16_t charIndex;  // 2 bytes (if it's NL_INVALID_FONT_INDEX it's not a char)
  uint16_t flags;      // 2 bytes 1 = diagetic, 2 = billboard, 4 = shading
};

layout(std430, set = 1, binding = 0) buffer readonly InstanceDataBuffer {
  Instance instances[NL_MAX_INSTANCES];
} instanceData;

// Meshes
struct MeshData
{
  uint indexCount;   // 4 bytes
  uint firstIndex;   // 4 bytes
  int  vertexOffset; // 4 bytes
};

// NL_MAX_INSTANCES for the size here isn't accurate but it's surely large enough so we don't care
layout(std430, set = 1, binding = 1) buffer readonly MeshDataBuffer {
  MeshData meshes[NL_MAX_INSTANCES];
} meshData;

struct ModelData
{
  uint meshIndices[NL_MAX_MESHES];
  uint textureIndices[NL_MAX_MESHES];
  uint numMeshes;
};

layout(std430, set = 1, binding = 3) buffer readonly ModelDataBuffer {
  ModelData models[NL_MAX_MODELS];
} modelData;
