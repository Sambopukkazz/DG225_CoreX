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

float4 EllipsePS(VertexShaderOutput input) : COLOR
{
    float2 delta = (input.TextureCoordinates - Center) / Scale;
    float dist = length(delta);
    
    float maskAlpha = smoothstep(1.0, 1.0 + FadeSoftness, dist);
    
    float finalAlpha = maskAlpha * Color.a;
    
    return float4(Color.rgb * finalAlpha, finalAlpha);
}

float4 RectanglePS(VertexShaderOutput input) : COLOR
{
    float2 uv = input.TextureCoordinates;
    
    // Define our 4 borders
    float leftEdge = Center.x - Scale.x;
    float rightEdge = Center.x + Scale.x;
    float topEdge = Center.y - Scale.y;
    float bottomEdge = Center.y + Scale.y; // In MonoGame, Y goes DOWN (so + is bottom)
    
    // 0.002 gives a tiny bit of anti-aliasing to the sharp edges so they don't look pixelated
    float crisp = 0.002;
    
    // Calculate masks for each side. (1.0 = color overlay, 0.0 = clear)
    float alphaLeft = 1.0 - smoothstep(leftEdge - crisp, leftEdge, uv.x);
    float alphaRight = smoothstep(rightEdge, rightEdge + crisp, uv.x);
    float alphaTop = 1.0 - smoothstep(topEdge - crisp, topEdge, uv.y);
    
    // The bottom edge specifically uses your FadeSoftness!
    float alphaBottom = smoothstep(bottomEdge, bottomEdge + FadeSoftness, uv.y);
    
    // Combine them all. If the pixel triggers ANY edge, we clamp it to 1.0 (saturate)
    float maskAlpha = saturate(alphaLeft + alphaRight + alphaTop + alphaBottom);

    float finalAlpha = maskAlpha * Color.a;
    return float4(Color.rgb * finalAlpha, finalAlpha);
}

technique EllipseMask
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL EllipsePS();
    }
}

technique RectangleMask
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL RectanglePS();
    }
}