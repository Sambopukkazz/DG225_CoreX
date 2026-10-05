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

// Customizable properties mimicking the tutorial controls
float2 Center;
float2 Scale;
float FadeSoftness;
float4 Color;

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 delta = (input.TextureCoordinates - Center) / Scale;
    float dist = length(delta);
    
    float maskAlpha = smoothstep(1.0, 1.0 + FadeSoftness, dist);
    
    float finalAlpha = maskAlpha * Color.a;
    
    return float4(Color.rgb * finalAlpha, finalAlpha);
}

technique PostProcess
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}