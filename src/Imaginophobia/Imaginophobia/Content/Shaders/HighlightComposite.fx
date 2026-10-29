#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_3
#define PS_SHADERMODEL ps_4_0_level_9_3
#endif

// Full screen composite for ONE highlight group. The mask carries the object's state in its channels, so
// each effect reads its own channel: R = focused, G = ambient, B = pulse.
// A colour's ALPHA is that effect's switch - Color.Transparent means off.
Texture2D MaskTexture;

sampler2D MaskSampler = sampler_state
{
    Texture = <MaskTexture>;
    Filter = Point;
    AddressU = Clamp;
    AddressV = Clamp;
};

// ---- settings ----
float2 MaskTexel; // 1 / mask width, 1 / mask height
float Time; // seconds since startup
float OutlineRadius; // glow thickness in screen pixels
float ScanSpeed; // sweeps per second
float ScanWidth; // band thickness as a fraction of screen height
float PulseSpeed; // pulses per second
float PulseMin; // 0..1 brightness floor of the pulse
float4 OutlineColor;
float4 ScanColor;
float4 PulseColor;

struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0;
};

float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 uv = input.TextureCoordinates;
    float4 mask = tex2D(MaskSampler, uv);
    float focused = mask.r;
    float ambient = mask.g;
    float pulsing = mask.b;

    // 1. Outline. Dilate the focused channel so the ring traces the object's outer silhouette and not the
    // tile edges
    float outline = 0.0;
    if (OutlineColor.a > 0.0)
    {
        float dilated = focused;
        [unroll]
        for (int i = 0; i < 16; i++)
        {
            float angle = (i / 16.0) * 6.2831853;
            dilated = max(dilated, tex2D(MaskSampler, uv + float2(cos(angle), sin(angle)) * MaskTexel * OutlineRadius).r);
        }
        [unroll]
        for (int i = 0; i < 12; i++)
        {
            float angle = (i / 12.0) * 6.2831853;
            dilated = max(dilated, tex2D(MaskSampler, uv + float2(cos(angle), sin(angle)) * MaskTexel * OutlineRadius * 0.5).r);
        }
        outline = saturate(dilated - focused);
    }

    // 2. Scanline. One band sweeping down the screen, visible only where it crosses an ambient object
    float band = 1.0 - saturate(abs(frac(Time * ScanSpeed) - uv.y) / max(ScanWidth, 0.001));
    float scan = ambient * band * ScanColor.a;

    // 3. Pulse
    float pulse = 0.5 + 0.5 * sin(Time * 6.2831853 * PulseSpeed);
    float tint = pulsing * PulseColor.a * (PulseMin + (1.0 - PulseMin) * pulse);

    float3 rgb = OutlineColor.rgb * outline
               + ScanColor.rgb * scan
               + PulseColor.rgb * tint;

    float alpha = saturate(outline * OutlineColor.a + scan + tint * 0.45);

    return float4(rgb, alpha);
}

technique Highlight
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}