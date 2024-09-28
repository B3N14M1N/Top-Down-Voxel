using System.Collections.Generic;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

public class JobChunksFilter
{
    private NativeArray<Vector3> oldChunks;
    private NativeArray<Vector3> newChunks;
    private NativeArray<float3> Chunks;
    private NativeArray<int> Count;

    private JobHandle jobHandle;
    public int start;
    public int end;
    public JobChunksFilter(NativeArray<Vector3> oldChunks, int start, int end, List<Vector3> newChunks, Vector3 center, int range)
    {
        this.oldChunks = oldChunks;
        this.start = start;
        this.end = end;
        this.newChunks = new NativeArray<Vector3>(newChunks.ToArray(), Allocator.Persistent);

        int size = end - start + 1;
        Chunks = new NativeArray<float3>(size + newChunks.Count, Allocator.Persistent);
        Count = new NativeArray<int>(1, Allocator.Persistent, NativeArrayOptions.UninitializedMemory);

        var job = new ChunksFilterJob()
        {
            oldChunks = this.oldChunks.Reinterpret<float3>(),
            start = start,
            end = end,
            newChunks = this.newChunks.Reinterpret<float3>(),
            Chunks = Chunks,
            Count = Count,
            Center = center,
            Range = range
        };
        jobHandle = job.Schedule();
    }
    public NativeArray<Vector3> Complete()
    {
        jobHandle.Complete();
        start = 0;
        end = Count[0];
        return Chunks.Reinterpret<Vector3>();
    }



    public void Dispose()
    {
        if(Count.IsCreated) Count.Dispose();
        if(newChunks.IsCreated) newChunks.Dispose();
        if(oldChunks.IsCreated) oldChunks.Dispose();
        //if (Chunks.IsCreated) Chunks.Dispose();
    }

    ~JobChunksFilter()
    {
        Dispose();
    }

    [BurstCompile]
    protected struct ChunksFilterJob : IJob
    {
        [ReadOnly] public Vector3 Center;
        [ReadOnly] public int Range;
        [ReadOnly] public int start, end;

        [ReadOnly] public NativeArray<float3> newChunks;
        [ReadOnly] public NativeArray<float3> oldChunks;
        public NativeArray<float3> Chunks;
        public NativeArray<int> Count;

        bool ChunkInRange(Vector3 center, Vector3 position, int range)
        {
            return position.x <= center.x + range &&
                position.x >= center.x - range &&
                position.z <= center.z + range &&
                position.z >= center.z - range;
        }

        float ChunkRangeMagnitude(Vector3 center, Vector3 position)
        {
            return (position.x - center.x) * (position.x - center.x) + (position.z - center.z) * (position.z - center.z);
        }

        public void Execute()
        {
            Count[0] = 0;
            NativeArray<(float, float3)> data = new NativeArray<(float, float3)>(Chunks.Length, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            for (int i = start; i < end; i++)
            {
                float magnitude = ChunkRangeMagnitude(Center, oldChunks[i]);
                bool inRange = ChunkInRange(Center, oldChunks[i], Range);
                if (inRange)
                {
                    data[Count[0]++] = (magnitude, oldChunks[i]);
                }
            }
            for (int i = 0; i < newChunks.Length; i++)
            {
                float magnitude = ChunkRangeMagnitude(Center, newChunks[i]);
                bool inRange = ChunkInRange(Center, newChunks[i], Range);
                if (inRange)
                {
                    data[Count[0]++] = (magnitude, newChunks[i]);
                }
            }
            mergeSort(ref data, 0, Count[0] - 1);

            // assign the data back
            for (int i = 0; i < Count[0]; i++)
            {
                Chunks[i] = data[i].Item2;
            }

            if (data.IsCreated) data.Dispose();
        }
        void mergeSort(ref NativeArray<(float, float3)> arr, int left, int right)
        {
            if (left >= right)
                return;

            int mid = left + (right - left) / 2;
            mergeSort(ref arr, left, mid);
            mergeSort(ref arr, mid + 1, right);
            merge(ref arr, left, mid, right);
        }

        void merge(ref NativeArray<(float, float3)> arr, int left, int mid, int right)
        {
            int n1 = mid - left + 1;
            int n2 = right - mid;

            // Create temp vectors
            NativeArray<(float, float3)>
                L = new NativeArray<(float, float3)>(n1, Allocator.Temp, NativeArrayOptions.UninitializedMemory),
                R = new NativeArray<(float, float3)>(n2, Allocator.Temp, NativeArrayOptions.UninitializedMemory);

            int i = 0, j = 0;
            // Copy data to temp vectors L[] and R[]
            for (i = 0; i < n1; i++)
                L[i] = arr[left + i];
            for (j = 0; j < n2; j++)
                R[j] = arr[mid + 1 + j];

            i = j = 0;
            int k = left;

            // Merge the temp vectors back 
            // into arr[left..right]
            while (i < n1 && j < n2)
            {
                if (L[i].Item1 <= R[j].Item1)
                {
                    arr[k] = L[i];
                    i++;
                }
                else
                {
                    arr[k] = R[j];
                    j++;
                }
                k++;
            }

            // Copy the remaining elements of L[], 
            // if there are any
            while (i < n1)
            {
                arr[k] = L[i];
                i++;
                k++;
            }

            // Copy the remaining elements of R[], 
            // if there are any
            while (j < n2)
            {
                arr[k] = R[j];
                j++;
                k++;
            }

            if (L.IsCreated) L.Dispose();
            if (R.IsCreated) R.Dispose();
        }

    }
}
