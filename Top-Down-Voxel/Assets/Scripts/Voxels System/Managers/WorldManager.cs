using System;
using UnityEditor;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [Header("World Settings")]
    public int seed;

    [Header("Player")]
    public Transform player;
    public Vector3 playerChunkPosition;


    public void Start()
    {
        playerChunkPosition = WorldSettings.ChunkPositionFromPosition(player.transform.position);
        ChunksManager.Instance.UpdateChunks(WorldSettings.ChunkPositionFromPosition(player.transform.position));
    }

    // Update is called once per frame
    void Update()
    {
        playerChunkPosition = WorldSettings.ChunkPositionFromPosition(player.transform.position);
        if (ChunksManager.Instance.Center != playerChunkPosition)
        {
            ChunksManager.Instance.UpdateChunks(playerChunkPosition);
        }
    }

    private void OnApplicationQuit()
    {
#if UNITY_EDITOR
        EditorUtility.UnloadUnusedAssetsImmediate();
        GC.Collect();
#endif
    }
}
