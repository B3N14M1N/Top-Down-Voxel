using System;
using UnityEditor;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    [Header("World Settings")]
    public int seed;

    [Header("Player")]
    public Transform player;



    public void Start()
    {
        ChunksManager.Instance.UpdateChunks(WorldSettings.ChunkPositionFromPosition(player.transform.position));
    }

    // Update is called once per frame
    void Update()
    {
        var currentPosition = WorldSettings.ChunkPositionFromPosition(player.transform.position);
        if (ChunksManager.Instance.Center != currentPosition)
        {
            ChunksManager.Instance.UpdateChunks(currentPosition);
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
