/*********************************************************************
 * file:   heightmap_shader.tese
 * author: evan.gray (evan.gray@digipen.edu)
 * date:   November 21, 2024
 * Copyright � 2024 DigiPen (USA) Corporation.
 *
 * brief:  The tessilation evaluation shader for the heightmap pipeline
 *********************************************************************/
#version 460
//#extension GL_EXT_debug_printf : enable

//Define patch type, subdivision mode, and winding order of the new tesselated data
layout (quads, fractional_odd_spacing, ccw) in;

//TESSILATION EVALUATION INPUTS ----------------------------------------

//Received from Tessellation Control Shader - all texture coordinates for the patch vertices
layout(location = 0) in vec2 TextureCoord[];

//----------------------------------------------------------------------
//TESSILATION EVALUATION OUTPUTS ---------------------------------------

//Send to Fragment Shader for coloring
layout(location = 0) out vec2 TexCoord;
layout(location = 1) out float normalizedYPosition;

//----------------------------------------------------------------------
//TESSILATION EVALUATION UNIFORMS --------------------------------------
layout(set = 0, binding = 0) uniform MatrixUniforms 
{
    mat4 uProjectionView;
    vec3 uDirectionToLight;
};

layout(set = 1, binding = 0) uniform HeightmapUniforms 
{
  mat4 uModelMatrix[NL_MAX_HEIGHTMAP_COUNT];
  vec4 uBaseColor[NL_MAX_HEIGHTMAP_COUNT];
  vec4 uPeakColor[NL_MAX_HEIGHTMAP_COUNT];
  bool uVisibleBase[NL_MAX_HEIGHTMAP_COUNT];
  mat4 uViewMatrix;
  vec4 uCameraPos;
};

layout(set = 1, binding = 1)  uniform sampler2D uHeightmapTextures[NL_MAX_HEIGHTMAP_COUNT];

layout(push_constant) uniform PushConstants 
{
	uint uHeightmapIndex;
};

void main()
{
    //Get patch coordinate
    float u = gl_TessCoord.x;
    float v = gl_TessCoord.y;
    
    //Print current patch
    //debugPrintfEXT("Patch: %f, %f", u, v);

    //----------------------------------------------------------------------
    //Retrieve control point texture coordinates
    vec2 t00 = TextureCoord[0];
    vec2 t01 = TextureCoord[1];
    vec2 t10 = TextureCoord[2];
    vec2 t11 = TextureCoord[3];

    //Bilinearly interpolate texture coordinate across patch
    vec2 t0 = (t01 - t00) * u + t00;
    vec2 t1 = (t11 - t10) * u + t10;
    vec2 texCoord = (t1 - t0) * v + t0;

    //Lookup texel at patch coordinate for height and scale + shift as desired
    normalizedYPosition = texture(uHeightmapTextures[uHeightmapIndex], texCoord).y;
    float yPosition = normalizedYPosition * 0.1;


    //----------------------------------------------------------------------
    //Retrieve control point position coordinates
    vec4 p00 = gl_in[0].gl_Position;
    vec4 p01 = gl_in[1].gl_Position;
    vec4 p10 = gl_in[2].gl_Position;
    vec4 p11 = gl_in[3].gl_Position;

    //Compute patch surface normal
    vec4 uVec = p01 - p00;
    vec4 vVec = p10 - p00;
    vec4 normal = normalize( vec4(cross(vVec.xyz, uVec.xyz), 0));

    //Bilinearly interpolate position coordinate across patch
    vec4 p0 = (p01 - p00) * u + p00;
    vec4 p1 = (p11 - p10) * u + p10;
    vec4 p = (p1 - p0) * v + p0;


    //Displace point along normal
    p += normal * yPosition;

    //----------------------------------------------------------------------
    //Output patch point position in clip space
    gl_Position = uProjectionView * uModelMatrix[uHeightmapIndex] * p;

    //Pass texture coordinate though
    TexCoord = texCoord;
}