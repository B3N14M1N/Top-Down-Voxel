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
	int posData = asint(packedData.x);

	float x = posData & 0xff;
	posData >>= 8;
	float y = posData & 0xff;
	posData >>= 8;
	float z = posData & 0xff;
	posData >>= 8;
	position = float3(x,y,z);

	int data = asint(packedData.z);

	float normalIndex = data & 0xff;
	normals = float3(MyNormals[floor(normalIndex)]);
	data >>= 8;
	float uvIndex = data & 0xff;
	uvs = float2(MyUVs[int(floor(uvIndex))]);
	
	color32 = float4(packedData.y, 0, 0, 0);
};

#endif