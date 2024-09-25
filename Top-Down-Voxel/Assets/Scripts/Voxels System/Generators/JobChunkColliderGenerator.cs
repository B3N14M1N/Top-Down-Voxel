using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class JobChunkColliderGenerator
{
    public NativeArray<HeightMap>.ReadOnly HeightMap;
    public MeshColliderDataStruct ColliderData;

    public JobHandle dataHandle;

    public bool GenerationStarted { get; private set; }
    public bool Generated { get; private set; }
    public JobChunkColliderGenerator(NativeArray<HeightMap>.ReadOnly heightMap)
    {
        HeightMap = heightMap;
        GenerationStarted = false;
        Generated = false;
    }

    public void ScheduleGeneration()
    {
        if(!GenerationStarted && !Generated)
        {
            var dataJob = new ChunkMeshColliderJob()
            {
                chunkWidth = WorldSettings.ChunkWidth,
                chunkHeight = WorldSettings.ChunkHeight,
                heightMaps = this.HeightMap,
            };
            GenerationStarted = true;
            dataHandle = dataJob.Schedule();
        }
    }
    public bool CompletedGeneration()
    {
        if (GenerationStarted && !Generated && dataHandle.IsCompleted)
        {
            dataHandle.Complete();
            Generated = true;
            return true;
        }
        return false;
    }
    public void SetCollider(ref Mesh mesh)
    {
        if(GenerationStarted && ColliderData.Initialized)
        {
            if (mesh == null)
                mesh = new Mesh() { indexFormat = IndexFormat.UInt32 };
            ColliderData.UploadToMesh(ref mesh);
        }
    }

    public void Dispose()
    {
        dataHandle.Complete();
        ColliderData.Dispose();
    }
}

public struct MeshColliderDataStruct
{
    [NativeDisableParallelForRestriction]
    [NativeDisableContainerSafetyRestriction]
    public NativeArray<float3> vertices;
    [NativeDisableParallelForRestriction]
    [NativeDisableContainerSafetyRestriction]
    public NativeArray<int> indices;

    [NativeDisableParallelForRestriction]
    [NativeDisableContainerSafetyRestriction]
    public NativeArray<int> count;

    public bool Initialized;
    public void Initialize()
    {
        if (Initialized)
            Dispose();

        count = new NativeArray<int>(2, Allocator.Persistent);
        vertices = new NativeArray<float3>(WorldSettings.ChunkWidth * 4 * (WorldSettings.ChunkWidth + 4), Allocator.Persistent, NativeArrayOptions.UninitializedMemory);
        int size = WorldSettings.ChunkWidth * (WorldSettings.ChunkWidth * 3 + 2);
        indices = new NativeArray<int>(size * 6, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

        Initialized = true;
    }
    public void Dispose()
    {
        Initialized = false;
        if (vertices.IsCreated) vertices.Dispose();
        if (indices.IsCreated) indices.Dispose();
        if (count.IsCreated) count.Dispose();
    }
    public void UploadToMesh(ref Mesh mesh)
    {
        if (Initialized && mesh != null)
        {
            mesh.SetVertices(vertices.Reinterpret<Vector3>(), 0, count[0], MeshUpdateFlags.DontRecalculateBounds & MeshUpdateFlags.DontValidateIndices & MeshUpdateFlags.DontNotifyMeshUsers);
            mesh.SetIndices(indices, 0, count[1], MeshTopology.Triangles, 0, false);
            mesh.bounds = WorldSettings.ChunkBounds;
        }
    }
}

[BurstCompile]
public struct ChunkMeshColliderJob : IJob
{
    #region Input

    [ReadOnly]
    public int chunkWidth;
    [ReadOnly]
    public int chunkHeight;
    [ReadOnly]
    [NativeDisableContainerSafetyRestriction]
    public NativeArray<HeightMap>.ReadOnly heightMaps;
    #endregion

    #region Output
    [NativeDisableContainerSafetyRestriction]
    public MeshColliderDataStruct colliderData;
    #endregion

    #region Methods

    private readonly int GetMapIndex(int x, int z)
    {
        return z + (x * (chunkWidth + 2));
    }
    #endregion

    #region Execution

    public void Execute()
    {
        #region Allocations
        NativeArray<float3> Vertices = new NativeArray<float3>(8, Allocator.Temp)
        {
            [0] = new float3(0, 1, 0), //0
            [1] = new float3(1, 1, 0), //1
            [2] = new float3(1, 1, 1), //2
            [3] = new float3(0, 1, 1), //3

            [4] = new float3(0, 0, 0), //4
            [5] = new float3(1, 0, 0), //5
            [6] = new float3(1, 0, 1), //6
            [7] = new float3(0, 0, 1) //7
        };


        NativeArray<int> FaceVerticeIndex = new NativeArray<int>(24, Allocator.Temp)
        {
            [0] = 4,
            [1] = 5,
            [2] = 1,
            [3] = 0,
            [4] = 5,
            [5] = 6,
            [6] = 2,
            [7] = 1,
            [8] = 6,
            [9] = 7,
            [10] = 3,
            [11] = 2,
            [12] = 7,
            [13] = 4,
            [14] = 0,
            [15] = 3,
            [16] = 0,
            [17] = 1,
            [18] = 2,
            [19] = 3,
            [20] = 7,
            [21] = 6,
            [22] = 5,
            [23] = 4,
        };


        NativeArray<int> FaceIndices = new NativeArray<int>(6, Allocator.Temp)
        {
            [0] = 0,
            [1] = 3,
            [2] = 2,
            [3] = 0,
            [4] = 2,
            [5] = 1
        };
        #endregion

        #region Execution
        int sidesVerticesStartIndex = 4 * chunkWidth * chunkWidth;
        for (int x = 1; x <= chunkWidth; x++)
        {
            for (int z = 1; z <= chunkWidth; z++)
            {
                float3 voxelPos = new float3(x - 1, (int)(RWStructs.GetSolid(heightMaps[GetMapIndex(x, z)])) - 1, z - 1);

                int i = 4; //Top face

                for (int j = 0; j < 4; j++)
                {
                    colliderData.vertices[colliderData.count[0] + j] = Vertices[FaceVerticeIndex[i * 4 + j]] + voxelPos;
                }
                for (int k = 0; k < 6; k++)
                {
                    colliderData.indices[colliderData.count[1] + k] = colliderData.count[0] + FaceIndices[k];
                }
                colliderData.count[0] += 4;
                colliderData.count[1] += 6;
            }
        }
        #endregion

        #region Deallocations

        if (Vertices.IsCreated) Vertices.Dispose();
        if (FaceVerticeIndex.IsCreated) FaceVerticeIndex.Dispose();
        if (FaceIndices.IsCreated) FaceIndices.Dispose();
        #endregion
    }
    #endregion
}
