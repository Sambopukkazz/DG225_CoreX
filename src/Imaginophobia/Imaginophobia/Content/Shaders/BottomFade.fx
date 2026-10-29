#if OPENGL
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_3
#define PS_SHADERMODEL ps_4_0_level_9_3
#endif

// How much of the bottom should fade? (e.g., 0.2 = bottom 20%)
float FadePercentage;

sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
};

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    // 1. Get the base color from SpriteBatch.Draw (your texture + your color tint)
    float4 color = tex2D(SpriteTextureSampler, input.TextureCoordinates) * input.Color;
    
    // 2. Calculate the fade based on the Y coordinate
    // smoothstep(1.0, 1.0 - FadePercentage) means:
    // When Y reaches 1.0 (bottom edge), alpha is 0.0
    // When Y is above the fade zone, alpha is 1.0
    float alphaMask = smoothstep(1.0, 1.0 - FadePercentage, input.TextureCoordinates.y);
    
    // 3. Multiply the entire color by alphaMask to maintain MonoGame's premultiplied alpha
    return color * alphaMask;
}

technique BottomFade
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}