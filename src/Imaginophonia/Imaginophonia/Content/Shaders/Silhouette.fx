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

// R = focused, G = ambient, B = pulse. Set before each batch
float4 MaskChannel;

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 color = tex2D(SpriteTextureSampler, input.TextureCoordinates) * input.Color;

    // Transparent atlas texels must not be stamped into the mask
    clip(color.a - 0.5);

    return float4(MaskChannel.rgb, 1.0);
}

technique HighlightMask
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}
