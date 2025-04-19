/*********************************************************************
 * file:   particleStructs.glsl
 * author: aditya.prakash (aditya.prakash@digpen.edu)
 * date:   March 14, 2025
 * Copyright  2025 DigiPen (USA) Corporation.
 *
 * brief:  Holds a bunch of structs that are relavant for particles
 *********************************************************************/

struct ParticleEmitter
{
  // --- Initial Particle Properties ---
  vec3 position;			       // initial position
  vec3 velocity; 			       // initial velocity
  vec3 velocityVariation;        // variation in velocity
  vec3 acceleration; 		       // acceleration
  vec3 accelerationVariation;    // variation in acceleration
  vec3 rotation; 			       // initial rotation
  vec3 angularVelocity; 	       // initial angular velocity
  vec3 angularVelocityVariation; // variation in angular velocity
  vec4 tint; 		  // base color
  float sizeBegin;			   // initial size
  float sizeEnd;				   // final size
  float sizeVariation;		   // variation in size
  float lifetime; 			   // particle lifetime

  // --- Texture & Model ---
  uint isBillboard; 	  // is the particle a billboard?
  uint isDiagetic; 	  // is the particle diagetic?
  uint isTextured; 	  // is the particle textured?
  uint textureIndex;    // (internal) index of the texture 
  uint modelIndex; 	  // (internal) index of the model

  // --- Emission ---
  uint maxParticles;           // maximum number of particles
  uint spawnCount;             // number of particles to spawn
};

struct Particle
{
  vec4 posSize;        // vec3 (pos), float (startSize)
  vec4 velSize;        // vec3 (vel), float (endSize)
  vec4 accelSize;      // vec3 (accel), float (currentSize)
  vec4 rotLife;        // vec3 (rotation), float (lifetime)
  vec4 angularVelAge;  // vec3 (angular vel), float (age)
  vec4 tint;           // current color
  uint textureIndex;    // (internal) index of the texture
  uint modelIndex;      // (internal) index of the model
  uint isBillboard;	 // is the particle a billboard?
  uint isDiagetic;     // is the particle diagetic?
};