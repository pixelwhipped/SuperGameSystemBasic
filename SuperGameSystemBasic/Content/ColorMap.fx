// Pixel shader extracts the brighter areas of an image.
// This is the first step in applying a bloom postprocess.

sampler TextureSampler : register(s0);

float4 NewForeground;
float4 NewBackground;
float4 OldForeground;
float4 OldBackground;

struct VSOutput
{
	float4 position		: SV_Position;
	float4 color		: COLOR0;
	float2 texCoord		: TEXCOORD0;
};


float4 PixelShaderFunction(VSOutput input) : COLOR0
{
	float4 c = tex2D(TextureSampler, input.texCoord);  // Look up the original image color.    
	if (c.a == OldForeground.a) c = NewForeground;
	if (c.a == OldBackground.a) c = NewBackground;
	return c;
}


technique ColorMap
{
	pass Pass1
	{
		PixelShader = compile ps_4_0_level_9_1 PixelShaderFunction();
	}
}
