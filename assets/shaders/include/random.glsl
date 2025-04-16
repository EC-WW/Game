/*********************************************************************
 * file:   instance_set.glsl
 * author: aditya.prakash (aditya.prakash@digpen.edu)
 * date:   November 21, 2024
 * Copyright  2024 DigiPen (USA) Corporation.
 *
 * brief:  Random Number Generation (LCG - Linear Congruential Generator)
 *********************************************************************/

uint pcg_hash(uint input_) {
  uint state = input_ * 747796405u + 2892285090u;
  uint word = ((state >> ((state >> 28u) + 4u)) ^ state) * 277803737u;
  return (word >> 22u) ^ word;
}

float randFloat(in uint seed) {
  return float(pcg_hash(seed)) / float(0xFFFFFFFFu);
}

vec3 randVec3(in uint seedBase, in vec3 variation) {
  uint seedX = seedBase + 1u;
  uint seedY = seedBase + 2u;
  uint seedZ = seedBase + 3u;
  return vec3(
    (randFloat(seedX) * 2.0 - 1.0) * variation.x,
    (randFloat(seedY) * 2.0 - 1.0) * variation.y,
    (randFloat(seedZ) * 2.0 - 1.0) * variation.z
  );
}