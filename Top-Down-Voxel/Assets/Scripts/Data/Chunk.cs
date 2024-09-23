using Unity.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class Chunk
{
    #region Fields

    public Vector3 Position { get; private set; }

    private GameObject chunkInstance;

    private NativeArray<Voxel> voxels;
    private NativeArray<HeightMap> heightMap;

    public bool Dirty { get; private set; }

    public Transform Parent 
    { 
        get
        {
            if (chunkInstance == null)
            {
                return null;
            }
            return chunkInstance.transform.parent;
        }
        set
        {
            if (chunkInstance != null)
                chunkInstance.transform.parent = value;
        }
    }

    public Renderer Renderer
    {
        get
        {
            if (chunkInstance != null && chunkInstance.TryGetComponent<Renderer>(out var Renderer))
            {
                return Renderer;
            }
            return null;
        }
    }
    public bool Render
    {
        get
        {
            if (Renderer != null)
                return Renderer.enabled;
            return false;
        }
        set
        {
            if (Renderer != null)
            {
                Renderer.enabled = value;
            }
        }
    }
    public bool Active 
    { 
        get
        {
            if (chunkInstance != null)
                return chunkInstance.activeSelf;
            return false;
        }
        set
        {
            if (chunkInstance != null)
                chunkInstance.SetActive(value);
        }
    }

    public Voxel this[Vector3 pos]
    {
        get 
        {
            return voxels[GetVoxelIndex(pos)];
        }

        private set
        {
            int index = GetVoxelIndex(pos);
            if (voxels[index].IsEmpty) 
                voxels[index] = value;
        }
    }

    public Voxel this[int x, int y, int z]
    {
        get
        {
            return voxels[GetVoxelIndex(x, y, z)];
        }

        private set
        {
            int index = GetVoxelIndex(x, y, z);
            if (voxels[index].IsEmpty)
                voxels[index] = value;
        }
    }
    #endregion


    #region Voxels

    public bool SetVoxel(Voxel voxel, Vector3 pos)
    {
        if (voxel.IsEmpty) // removing
        {
            Debug.Log($"Removing voxel at coords: {pos}");
        }
        else
        {
            if (!this[pos].IsEmpty)
            {
                Debug.Log($"Cannot swap voxels at coords: {pos}");
                return false;
            }
        }
        Debug.Log($"Placing voxel at coords: {pos}");
        Dirty = true;
        this[pos] = voxel;
        return true;
    }

    private int GetVoxelIndex(int x, int y, int z)
    {
        return z + (y * (WorldSettings.ChunkWidth + 2)) + (x * (WorldSettings.ChunkWidth + 2) * WorldSettings.ChunkHeight);
    }

    private int GetVoxelIndex(Vector3 pos)
    {
        return GetVoxelIndex((int)pos.x, (int)pos.y, (int)pos.z);
    }
    #endregion

    #region Chunk instance
    public Chunk(Vector3 Position)
    {
        this.Position = Position;
        CreateInstance();
    }
    #endregion


    private void CreateInstance()
    {
        if (chunkInstance == null)
        {
            chunkInstance = new GameObject();
            chunkInstance.AddComponent<MeshRenderer>();
            chunkInstance.AddComponent<MeshFilter>();
            chunkInstance.AddComponent<MeshCollider>();
            //chunkInstance.AddComponent<DrawRendererBounds>();
        }
        chunkInstance.name = $"Chunk Instance [{(int)Position.x}]:[{(int)Position.z}]";
        chunkInstance.transform.position = new Vector3(Position.x, 0, Position.z) * WorldSettings.ChunkWidth;
    }

    #region Mesh & Data & Position
    public void UpdateChunk(Vector3 Position)
    {
        this.Position = Position;
        ClearChunk();

        chunkInstance.name = $"Chunk Instance [{(int)Position.x}]:[{(int)Position.z}]";
        chunkInstance.transform.position = new Vector3(Position.x, 0, Position.z) * WorldSettings.ChunkWidth;
    }
    public void UploadMesh(ref Mesh mesh)
    {
        if (chunkInstance.TryGetComponent<MeshFilter>(out var filter))
        {
            if (filter.sharedMesh != null)
                GameObject.Destroy(filter.sharedMesh);
            filter.sharedMesh = mesh;
        }

        if(chunkInstance.TryGetComponent<MeshRenderer>(out var renderer))
        {
            renderer.sharedMaterial = ChunkFactory.Instance.Material;
        }

        if(chunkInstance.TryGetComponent<MeshCollider>(out var collider))
        {
            collider.sharedMesh = mesh;
        }
    }

    public void UploadData(ref NativeArray<Voxel> voxels, ref NativeArray<HeightMap> heightMap)
    {
        if (this.voxels.IsCreated) this.voxels.Dispose();
        if (this.heightMap.IsCreated) this.heightMap.Dispose();
        this.voxels = voxels;
        this.heightMap = heightMap;
    }

    #endregion

    #region Clear & Disposing

    private void ClearMesh()
    {
        if (chunkInstance != null)
        {
            if (chunkInstance.TryGetComponent<MeshFilter>(out var filter) && filter.sharedMesh != null)
            {
                GameObject.Destroy(filter.sharedMesh);
            }
            if (chunkInstance.TryGetComponent<MeshCollider>(out var collider) && collider.sharedMesh != null)
            {
                collider.sharedMesh = null;
            }
        }
    }

    public void ClearChunk()
    {
        if (Dirty)
            Debug.Log($"Chunk {Position} needs saving.");

        ClearMesh();

        if (voxels.IsCreated) voxels.Dispose();
        if (heightMap.IsCreated) heightMap.Dispose();

        if (chunkInstance != null)
            chunkInstance.name = "Chunk Instance [pool]";

        this.Active = false;
    }

    public void Dispose()
    {
        if (voxels.IsCreated) voxels.Dispose();
        if (heightMap.IsCreated) heightMap.Dispose();
        ClearMesh();

        if(chunkInstance != null)
            GameObject.Destroy(chunkInstance);

    }
    #endregion
}
