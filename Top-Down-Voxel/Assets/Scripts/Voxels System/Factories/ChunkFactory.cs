using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChunkFactory : MonoBehaviour
{
    #region Fields
    public const string ChunkMeshCreationString = "ChunkMeshCreationString";
    private float time = 0f;

    [Header("Noise Parameters")]
    public NoiseParametersScriptableObject noiseParameters;
    private Vector2Int[] octaveOffsets;

    [Header("Buffers")]
    public List<Chunk> chunksToProccess = new List<Chunk>();

    public List<JobChunkGenerator> jobs = new List<JobChunkGenerator>();
    public List<JobChunkColliderGenerator> priorityJobs = new List<JobChunkColliderGenerator>();
    [Header("Materials")]
    public List<Material> materials = new List<Material>();
    public int materialIndex = 0;
    public Material Material { get { return materials[materialIndex]; } }
    public bool canChangeMaterial = true;

    public KeyCode MaterialChangeKey = KeyCode.M;

    private static ChunkFactory instance;
    public static ChunkFactory Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<ChunkFactory>();
            if (instance == null)
                instance = new ChunkFactory();
            return instance;
        }
        private set
        {
            instance = value;
        }
    }

    #endregion

    #region Initializations

    public void Awake()
    {
        InitSeed();
    }

    public void InitSeed()
    {
        uint octavesMax = 0;
        for (int i = 0; i < noiseParameters.noise.Count; i++)
        {
            if (noiseParameters.noise[i].octaves > octavesMax)
                octavesMax = noiseParameters.noise[i].octaves;
        }
        octaveOffsets = new Vector2Int[octavesMax];
        System.Random rnd = new System.Random((int)WorldSettings.Seed);
        for (int i = 0; i < octavesMax; i++)
        {
            octaveOffsets[i] = new Vector2Int(rnd.Next(-10000, 10000), rnd.Next(-10000, 10000));
        }
    }
    #endregion

    void Update()
    {
        if (Input.GetKeyUp(MaterialChangeKey) && canChangeMaterial)
        {
            materialIndex = materialIndex == materials.Count - 1 ? 0 : ++materialIndex;
        }

        int loaded = 0;
        for (int i = 0; i < jobs.Count; i++)
        {
            if (jobs[i] != null)
            {
                var timeNow = Time.realtimeSinceStartup;
                if (jobs[i].CompleteDataGeneration())
                {
                    priorityJobs.Add(new JobChunkColliderGenerator(jobs[i].heightMaps.AsReadOnly(), jobs[i].chunkPos));
                    jobs[i].ScheduleMeshGeneration();
                }
                else
                if (jobs[i].CompleteMeshGeneration())
                {
                    Chunk chunk = ChunksManager.Instance.GetChunk(jobs[i].chunkPos);
                    if (chunk != null)
                    {
                        chunk.UploadData(ref jobs[i].voxels, ref jobs[i].heightMaps);
                        Mesh mesh = new Mesh() { indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
                        jobs[i].SetMesh(ref mesh);
                        if (mesh != null)
                        {
                            chunk.UploadMesh(ref mesh);
                        }
                    }
                    jobs[i].Dispose();
                    jobs.RemoveAt(i);
                    i--;
                    loaded++;
                    AvgCounter.UpdateTimer(ChunkMeshCreationString, (Time.realtimeSinceStartup - timeNow) * 1000f);

                    if (loaded >= PlayerSettings.ChunksToLoad)
                        break;
                }
            }
        }
        for (int i = 0; i < priorityJobs.Count; i++)
        {
            if(priorityJobs[i] != null && priorityJobs[i].CompletedGeneration())
            {
                Chunk chunk = ChunksManager.Instance.GetChunk(priorityJobs[i].chunkPos);
                if (chunk != null)
                {
                    Mesh mesh = new Mesh() { indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
                    priorityJobs[i].SetCollider(ref mesh);
                    if (mesh != null)
                    {
                        chunk.UpdateCollider(ref mesh);
                    }
                    priorityJobs[i].Dispose();
                    priorityJobs.RemoveAt(i);
                    i--;
                }
            }
        }
        time += Time.deltaTime;
        if (time >= PlayerSettings.TimeToLoadNextChunks)
        {
            //sort chunks to generate the closest first
            chunksToProccess = (from chunk
                                in chunksToProccess
                                where WorldSettings.ChunksInRange(ChunksManager.Instance.Center, chunk.Position, PlayerSettings.LoadDistance)
                                orderby WorldSettings.ChunkRangeMagnitude(ChunksManager.Instance.Center, chunk.Position) ascending
                                select chunk).ToList();

            for (int i = 0; i < chunksToProccess.Count && JobChunkGenerator.Processed < PlayerSettings.ChunksProcessed; i++)
            {
                if (chunksToProccess[i] != null)
                {
                    jobs.Add(new JobChunkGenerator(chunksToProccess[i].Position, noiseParameters.noise.ToArray(), octaveOffsets.ToArray(), noiseParameters.globalScale));
                    chunksToProccess.RemoveAt(i);
                    i--;
                }
            }
            time = 0f;
        }
    }

    public void GenerateChunkData(Chunk chunk)
    {
        chunksToProccess.Add(chunk);
    }

    public void Dispose()
    {
        for (int i = 0; i < priorityJobs.Count; i++)
        {
            priorityJobs[i].Dispose();
            priorityJobs[i] = null;
            priorityJobs.RemoveAt(i);
            i--;
        }
        for (int i = 0; i < jobs.Count; i++)
        {
            jobs[i].Dispose(disposeData: true);
            jobs[i] = null;
            jobs.RemoveAt(i);
            i--;
        }
    }

    public void OnApplicationQuit()
    {
        Dispose();
    }

    public void OnDestroy()
    {
        Dispose();
    }
}
