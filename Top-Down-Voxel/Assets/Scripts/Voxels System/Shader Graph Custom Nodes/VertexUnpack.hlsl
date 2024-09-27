#ifndef VERTEXUNPACK_INCLUDED
#define VERTEXUNPACK_INCLUDED

static const float3 Normals[6]  = 
{
	float3(0,0,-1),
	float3(1,0,0),
	float3(0,0,1),
	float3(-1,0,0),
	float3(0,1,0),
	float3(0,-1,0)
};

static const float2 UVs[4] = 
{
	float2(0,0),
	float2(1,0),
	float2(1,1),
	float2(0,1)
};

void UnpackVertexData_float(float3 input, out float3 position, out float3 normals, out float2 uvs, out float4 color32)
{
	uint data = asuint(input.x);
	float x = data & 0xff;
	data >>= 8;
	float y = data & 0xff;
	data >>= 8;
	float z = data & 0xff;
	data >>= 8;
	int normalIndex = data & 0x7;
	data >>= 3;
	position = float3(x,y,z);
	normals = float3(Normals[normalIndex]);
	uvs = float2(UVs[int(data)]);
	color32 = float4(input.y/256.0,0,0,0);
}

#endif