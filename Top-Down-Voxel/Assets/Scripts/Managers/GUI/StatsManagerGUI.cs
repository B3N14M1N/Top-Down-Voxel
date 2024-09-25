using UnityEngine;
using System.Text;
using UnityEngine.Profiling;
using TMPro;
using System;



#if UNITY_EDITOR
using UnityEditor;
#endif
#if UNITY_5
using UnityEngine.Profiling;
#endif

[Serializable]
public class StatsManagerGUI : MonoBehaviour
{
    [Header("Stats Display")]
    private StringBuilder leftText;
    private StringBuilder middleText;
    private StringBuilder rightText;
    public TMP_Text leftSide;
    public TMP_Text middleSide;
    public TMP_Text rightSide;

    public WorldManager worldManager;

    private float updateInterval = 1.0f;
    private float lastInterval; // Last interval end time
    private float frames = 0; // Frames over current interval

    private float framesavtick = 0;
    private float framesav = 0.0f;
    private float FPS = 0;


    // Use this for initialization
    void Awake()
    {
        lastInterval = Time.realtimeSinceStartup;
        frames = 0;
        framesav = 0;
        leftText = new StringBuilder();
        leftText.Capacity = 200;
        rightText = new StringBuilder();
        rightText.Capacity = 200;
        middleText = new StringBuilder();
        middleText.Capacity = 200;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;
        if (!leftSide)
        {
            leftSide.gameObject.hideFlags = HideFlags.HideAndDontSave;
            leftSide.gameObject.transform.position = new Vector3(0, 0, 0);
        }
        if (!middleSide)
        {
            middleSide.gameObject.hideFlags = HideFlags.HideAndDontSave;
            middleSide.gameObject.transform.position = new Vector3(0, 0, 0);
        }

        if (!rightSide)
        {
            rightSide.gameObject.hideFlags = HideFlags.HideAndDontSave;
            rightSide.gameObject.transform.position = new Vector3(0, 0, 0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        ++frames;

        var timeNow = Time.realtimeSinceStartup;

        FPS = frames / (timeNow - lastInterval);

        if (timeNow > lastInterval + updateInterval)
        {
            float fps = frames / (timeNow - lastInterval);
            float ms = 1000.0f / Mathf.Max(fps, 0.00001f);

            ++framesavtick;
            framesav += fps;
            float fpsav = framesav / framesavtick;

            middleText.Length = 0;
            rightText.Length = 0;


            middleText.AppendFormat("Time : {0} ms\nCurrent FPS: {1}\nAvgFPS: {2}\nAvg Chunk Mesh Creation: {3}ms", ms, fps, fpsav, AvgCounter.GetTimer(ChunkFactory.ChunkMeshCreationString));
#if UNITY_EDITOR
            middleText.AppendFormat("\n\nDrawCalls : {0}\nUsed Texture Memory : {1}\nrenderedTextureCount : {2}", UnityStats.drawCalls, UnityStats.usedTextureMemorySize / 1048576, UnityStats.usedTextureCount);
#endif
            rightText.AppendFormat("GPU memory : {0}\nSys Memory : {1}\n" + "TotalAllocatedMemory : {2}mb\nTotalReservedMemory : {3}mb\nTotalUnusedReservedMemory : {4}mb",
                SystemInfo.graphicsMemorySize,
                SystemInfo.systemMemorySize,
                Profiler.GetTotalAllocatedMemoryLong() / 1048576,
                Profiler.GetTotalReservedMemoryLong() / 1048576,
                Profiler.GetTotalUnusedReservedMemoryLong() / 1048576)
                .AppendFormat("\nTotalAllocatedMemoryForGraphicsDriver: {0}mb", Profiler.GetAllocatedMemoryForGraphicsDriver() / 1048576);

            middleSide.text = middleText.ToString();
            rightSide.text = rightText.ToString();
            frames = 0;
            lastInterval = timeNow;
        }

        leftText.Length = 0;
        leftText.AppendFormat("FPS: {0}\n\nCoords: {1}\nChunk: {2}", string.Format("{0:0.##}",FPS), worldManager.player.position, worldManager.playerChunkPosition);
        leftSide.text = leftText.ToString();
    }
}

