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
    uint textureIndex;   // 4  bytes
    uint animationIndex; // 4  bytes
    uint meshIndex;      // 4  bytes
    uint16_t charIndex;  // 2 bytes (if it's NL_INVALID_FONT_INDEX it's not a char)
    uint16_t flags;      // 2 bytes 1 = diagetic, 2 = billboard
};

layout(std430, set = 1, binding = 0) buffer readonly InstanceDataBuffer {
    Instance instances[NL_MAX_INSTANCES];
} instanceData;

layout(std430, set = 1, binding = 2) buffer readonly ParticleInstanceDataBuffer {
  Instance instances[NL_MAX_INSTANCES];
} particleInstanceData;

struct MeshData
{
    uint indexCount;   // 4 bytes
    uint firstIndex;   // 4 bytes
    int  vertexOffset; // 4 bytes
    // no additional padding needed
};

// NL_MAX_INSTANCES for the size here isn't fully accurate
// come back to change this later perhaps
layout(std430, set = 1, binding = 1) buffer readonly MeshDataBuffer {
    MeshData meshes[NL_MAX_INSTANCES];
} meshData;