/*********************************************************************
 * file:   commonStructs.glsl
 * author: aditya.prakash (aditya.prakash@digpen.edu)
 * date:   March 14, 2025
 * Copyright  2025 DigiPen (USA) Corporation.
 *
 * brief:  Holds a bunch of common structs that are used in a lot of shaders
 *********************************************************************/

struct VkDispatchIndirectCommand {
  uint x;
  uint y;
  uint z;
};

struct MeshCountData {
  uint meshIndex;
  uint count;
  uint firstInstance;
  uint instanceCounter;
};