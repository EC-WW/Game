#include "particleStructs.glsl"
#include "commonStructs.glsl"

layout(std430, set = 1, binding = 4) buffer ParticleEmitterBuffer {
  ParticleEmitter emitters[NL_MAX_EMITTERS];
} particleEmitterBuffer;

layout(std430, set = 1, binding = 5) buffer ParticleCountersBuffer {
  int numEmitters;          // Number of emitters
  int aliveParticleCount1;   // Size of alive list
  int aliveParticleCount2;   // Size of alive list
  int deadParticleCount;    // Size of dead list
  int countAfterSimulation; // Really just for debug purposes
  uint numMeshes;
  MeshCountData meshCounts[NL_MAX_MODELS * NL_MAX_MESHES];
} particleCounterBuffer;

// Used by emitter
layout(std430, set = 2, binding = 7) buffer AliveList1Buffer {
  uint indices[NL_MAX_PARTICLES]; // Particle Indices
} aliveList1;

// List after particle update (remove dead particles)
layout(std430, set = 2, binding = 8) buffer AliveList2Buffer {
  uint indices[NL_MAX_PARTICLES]; // Particle Indices
} aliveList2;

// Pull new particle indices from here
layout(std430, set = 2, binding = 9) buffer DeadListBuffer {
  uint deadList[NL_MAX_PARTICLES]; // Indices of dead particles
} deadListBuffer;

layout(std430, set = 2, binding = 10) buffer ParticleDispatchBuffer {
  VkDispatchIndirectCommand updateDispatchCommand;
  VkDispatchIndirectCommand sortDispatchCommand;
} particleDispatchBuffer;