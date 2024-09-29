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

void UnpackVertexData_float(in vector packedData, out float3 position, out float3 normals, out float2 uvs, out vector color32)
{
	int data = asint(packedData.x);

	float x = data & 0xff;
	data >>= 8;
	float y = data & 0xff;
	data >>= 8;
	float z = data & 0xff;
	data >>= 8;
	position = float3(x,y,z);

	//float normalIndex = asint(packedData.z) & 0xff;
	//normals = float3(MyNormals[floor(normalIndex)]);
	normals = float3(MyNormals[floor(asint(packedData.z) & 0xff)]);
	uvs = float2(MyUVs[(asint(packedData.z) >> 8) & 0xff]);
	color32 = vector(float(packedData.y), 0, 0.0, 1.0);
};

void UnpackColorData_float(in vector packedData, out float2 uvs, out vector color32)
{
	uvs = float2(MyUVs[floor(packedData.x)]);

	float h = packedData.y;
	float m = packedData.z;
	color32 = vector(h / m, 0.0, 0.0, 0.0);
}

#endif