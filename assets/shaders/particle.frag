/**
 * @file particle_shader.frag
 * @brief The fragment shader for particles!
 */

#version 460

// Extensions
#extension GL_EXT_nonuniform_qualifier : require

// FRAG INPUTS -----------------------------------------------------
layout(location = 0) in vec2 fragTexCoord;
layout(location = 1) flat in vec4 fragTextureIndexTint;

// -------------------------------------------------------------------
// FRAG OUTPUTS ----------------------------------------------------
layout(location = 0) out vec4 outColor;

// -------------------------------------------------------------------
// Textures ----------------------------------------------------------
layout(set = 2, binding = 1) uniform sampler2D uTextures[NL_MAX_TEXTURES];

void main()
{
    vec4 texColor = texture(uTextures[uint(fragTextureIndexTint.x)], fragTexCoord);
    if (texColor.a < 0.1)
        discard;
    outColor = vec4(texColor.xyz * fragTextureIndexTint.yzw, 1.0);
}

//if (fragTextureIndex == NL_NO_TEXTURES) {
//    outColor = vec4(fragColor, 1.0);
//} else {
//    vec4 texColor = texture(uTextures[fragTextureIndex], fragTexCoord);
//    if (texColor.a < 0.1)
//        discard;
//    outColor = texColor;
//}