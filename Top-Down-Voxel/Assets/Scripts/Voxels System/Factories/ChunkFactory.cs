using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerationData
{
    public Vector3 position;
    public ChunkGenerationFlags flags;
}
public enum ChunkGenerationFlags
{
    None = 1,
    Mesh = 2,
    Data = 4,
};

public class ChunkFactory : MonoBehaviour
{

    private List<GenerationData> chunksToProccess = new List<GenerationData>();

    #region Fields

    [Header("Noise Parameters")]
    [SerializeField] private NoiseParametersScriptableObject noiseParameters;
    private Vector2Int[] octaveOffsets;

    [Header("Materials")]
    [SerializeField] private List<Material> materials = new List<Material>();
    private int materialIndex = 0;
    public Material Material { get { return materials[materialIndex]; } }
    public bool CanChangeMaterial { get; set; }

    [SerializeField] private KeyCode MaterialChangeKey = KeyCode.M;

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

    public void GenerateChunksData(List<GenerationData> toGenerate)
    {
        chunksToProccess = toGenerate;
    }


    #region Initializations

    public void Awake()
    {
        InitSeed();
        CanChangeMaterial = true;
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
}
