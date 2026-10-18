#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_3
#define PS_SHADERMODEL ps_4_0_level_9_3
#endif

// Mandatory dummy texture for SpriteBatch tracking blocks
Texture2D SpriteTexture;
sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
};

// ---- settings ----
float Saturation; // 0 = grayscale, 1 = unchanged, >1 = boosted

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 c = tex2D(SpriteTextureSampler, input.TextureCoordinates) * input.Color;

    // 1. luma, then blend back toward full colour by Saturation
    float luma = dot(c.rgb, float3(0.299, 0.587, 0.114));
    float3 gray = float3(luma, luma, luma);
    float3 graded = lerp(gray, c.rgb, Saturation);

    return float4(graded, c.a);
}

technique ApplySaturation
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}
