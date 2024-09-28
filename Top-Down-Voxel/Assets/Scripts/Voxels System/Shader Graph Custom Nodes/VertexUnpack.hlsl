#ifndef VERTEXUNPACK_INCLUDED
#define VERTEXUNPACK_INCLUDED

static const float3 MyNormals[6]  = 
{
	float3(0.0, 0.0, -1.0),
	float3(1.0, 0.0, 0.0),
	float3(0.0, 0.0, 1.0),
	float3(-1.0, 0.0, 0.0),
	float3(0.0, 1.0, 0.0),
	float3(0.0, -1.0, 0.0)
};

static const float2 MyUVs[4] = 
{
	float2(0.0, 0.0),
	float2(1.0, 0.0),
	float2(1.0, 1.0),
	float2(0.0, 1.0)
};

void UnpackVertexData_float(float3 packedData, out float3 position, out float3 normals, out float2 uvs, out float4 color32)
{
	int data = asint(packedData.x);

	float x = data & 0xff;
	data >>= 8;
	float y = data & 0xff;
	data >>= 8;
	float z = data & 0xff;
	data >>= 8;
	position = float3(x,y,z);

	int normalIndex = data & 0xf;
	normals = float3(MyNormals[normalIndex]);
	data >>= 4;
	int uvIndex = floor(packedData.z);
	uvs = float2(MyUVs[uvIndex]);
	
	color32 = float4(packedData.y, 0, 0, 0);
};

#endif