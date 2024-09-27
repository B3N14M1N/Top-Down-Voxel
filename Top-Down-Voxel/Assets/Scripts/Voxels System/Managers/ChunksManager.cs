using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ChunksManager : MonoBehaviour
{
    public const string ChunksManagerLoopString = "ChunksManagerLoop";
    public Vector3 Center { get; private set; }

    private Queue<Chunk> pool = new Queue<Chunk>();
    private Dictionary<Vector3, Chunk> active = new Dictionary<Vector3, Chunk>();
    private Dictionary<Vector3, Chunk> cached = new Dictionary<Vector3, Chunk>();
    private Chunk NewChunk
    {
        get
        {
            Chunk chunk = new Chunk(Vector3.zero);
            chunk.Parent = transform;
            chunk.Active = false;
            return chunk;
        }
    }

    private static ChunksManager instance;
    public static ChunksManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<ChunksManager>();
            }
            return instance;
        }
    }

    private void Awake()
    {
        for (int i = 0; i < (PlayerSettings.RenderDistance + PlayerSettings.CacheDistance) * (PlayerSettings.RenderDistance + PlayerSettings.CacheDistance); i++)
        {
            pool.Enqueue(NewChunk);
        }
    }

    public void UpdateChunks(Vector3 center)
    {
        var UpdateTime = Time.realtimeSinceStartup;
        Center = center;
        cached.AddRange(active);
        active.Clear();

        var chunksToGenerate = new List<Vector3>();

        for (int x = (int)center.x - PlayerSettings.RenderDistance; x <= (int)center.x + PlayerSettings.RenderDistance; x++)
        {
            for (int z = (int)center.z - PlayerSettings.RenderDistance; z <= (int)center.z + PlayerSettings.RenderDistance; z++)
            {
                Vector3 key = new Vector3(x, 0, z);
                if (cached.TryGetValue(key, out Chunk chunk))
                {
                    chunk.Active = true;
                    chunk.Render = true;
                    active.Add(key, chunk);
                    cached.Remove(key);
                    continue;
                }

                chunk = GetPooledChunk();
                chunk.UpdateChunk(key); 
                chunksToGenerate.Add(key);
                chunk.Active = true;
                chunk.Render = true;
                active.Add(key, chunk);
            }
        }

        ChunkFactory.Instance.GenerateChunksData(chunksToGenerate);

        // ~0.02 ms 20Chunks, ~0.06ms 50Chunks
        List<Vector3> removals = (from key in cached.Keys
                                  where !WorldSettings.ChunksInRange(center, key, PlayerSettings.RenderDistance + PlayerSettings.CacheDistance)
                                  select key).ToList();
        
        foreach (var key in removals)
        {
            ClearChunkAndEnqueue(key, ref cached);
        }

        foreach (var key in cached.Keys)
        {
            cached[key].Active = false;
        }

        AvgCounter.UpdateCounter(ChunksManagerLoopString, (Time.realtimeSinceStartup - UpdateTime) * 1000f);
    }

    public void ClearChunkAndEnqueue(Vector3 pos, ref Dictionary<Vector3, Chunk> source)
    {
        if (source.TryGetValue(pos, out Chunk chunk))
        {
            chunk.ClearChunk();
            var size = PlayerSettings.CacheDistance + PlayerSettings.RenderDistance;
            size *= size;
            size -= PlayerSettings.RenderDistance * PlayerSettings.RenderDistance; 
            if (pool.Count < size)
                pool.Enqueue(chunk);
            else
                chunk.Dispose();
            source.Remove(pos);
        }
    }
    private Chunk GetPooledChunk()
    {
        if (pool.Count > 0)
        {
            return pool.Dequeue();
        }
        else
        {
            return NewChunk;
        }
    }

    private Chunk GetChunkFromSource(Vector3 pos, ref Dictionary<Vector3, Chunk> source)
    {
        if (source == null)
            return null;
        source.TryGetValue(pos, out Chunk chunk);
        return chunk;
    }

    public Chunk GetChunk(Vector3 pos)
    {
        Chunk chunk =  GetChunkFromSource(pos, ref active);
        if (chunk == null)
            chunk = GetChunkFromSource(pos, ref cached);
        return chunk;
    }

    private int meshVertices = 0;
    private int meshIndices = 0;
    private int colliderVertices = 0;
    private int colliderIndices = 0;
    public void UpdateChunkMeshSize(int mV, int mI)
    {
        meshVertices += mV;
        meshIndices += mI;
    }
    public void UpdateChunkColliderSize(int cV, int cI)
    {
        colliderVertices += cV;
        colliderIndices += cI;
    }
    public (int, int, int, int, int) ChunksMeshAndColliderSize()
    {
        return (active.Count ,meshVertices, meshIndices, colliderVertices, colliderIndices);
    }


    public void Dispose()
    {
        while (pool.Count > 0)
        {
            pool.Dequeue().Dispose();
        }
        pool.Clear();

        foreach (var key in active.Keys)
        {
            active[key].Dispose();
        }
        active.Clear();

        foreach (var key in cached.Keys)
        {
            cached[key].Dispose();
        }
        cached.Clear();

#if UNITY_EDITOR
        EditorUtility.UnloadUnusedAssetsImmediate();
        GC.Collect();
#endif

    }

    private void OnApplicationQuit()
    {
        Dispose();
    }

    public void OnDestroy()
    {
        Dispose();
    }
}
