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
    }

    // Update is called once per frame
    void Update()
    {
        ++frames;

        var timeNow = Time.realtimeSinceStartup;


        if (timeNow > lastInterval + updateInterval)
        {
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

            float fps = frames / (timeNow - lastInterval);
            float ms = 1000.0f / Mathf.Max(fps, 0.00001f);

            ++framesavtick;
            framesav += fps;
            float fpsav = framesav / framesavtick;

            leftText.Length = 0;
            middleText.Length = 0;
            rightText.Length = 0;


            leftText.AppendFormat("FPS: \t{0}\r\nAVG: \t{1}\r\n\n", string.Format("{0:0.##}", fps), fpsav)
                .AppendFormat("Time : \t{0} ms\r\n\n", ms)
           
                .AppendFormat("CHUNK CREATION\r\nTime : \t{0}ms\r\nAvg Time : \t{1}ms\r\nMin Time : \t{2}ms\r\nMax Time : \t{3}ms\r\n\n",
                AvgCounter.GetCounter(ChunkFactory.ChunkMeshCreationString)?.Time,
                AvgCounter.GetCounter(ChunkFactory.ChunkMeshCreationString)?.AVG,
                AvgCounter.GetCounter(ChunkFactory.ChunkMeshCreationString)?.MinTime,
                AvgCounter.GetCounter(ChunkFactory.ChunkMeshCreationString)?.MaxTime)

                .AppendFormat("CHUNK FACTORY\r\nTime : \t{0}ms\r\nAvg Time : \t{1}ms\r\nMin Time : \t{2}ms\r\nMax Time : \t{3}ms\r\n\n",
                AvgCounter.GetCounter(ChunkFactory.ChunkFactoryLoopString)?.Time,
                AvgCounter.GetCounter(ChunkFactory.ChunkFactoryLoopString)?.AVG,
                AvgCounter.GetCounter(ChunkFactory.ChunkFactoryLoopString)?.MinTime,
                AvgCounter.GetCounter(ChunkFactory.ChunkFactoryLoopString)?.MaxTime)

                .AppendFormat("CHUNK MANAGER\r\nTime : \t{0}ms\r\nAvg Time : \t{1}ms\r\nMin Time : \t{2}ms\r\nMax Time : \t{3}ms\r\n\n",
                AvgCounter.GetCounter(ChunksManager.ChunksManagerLoopString)?.Time,
                AvgCounter.GetCounter(ChunksManager.ChunksManagerLoopString)?.AVG,
                AvgCounter.GetCounter(ChunksManager.ChunksManagerLoopString)?.MinTime,
                AvgCounter.GetCounter(ChunksManager.ChunksManagerLoopString)?.MaxTime)

                .AppendFormat("WORLD MANAGER\r\nTime : \t{0}ms\r\nAvg Time : \t{1}ms\r\nMin Time : \t{2}ms\r\nMax Time : \t{3}ms\r\n\n",
                AvgCounter.GetCounter(WorldManager.WorldManagerLoopString)?.Time,
                AvgCounter.GetCounter(WorldManager.WorldManagerLoopString)?.AVG,
                AvgCounter.GetCounter(WorldManager.WorldManagerLoopString)?.MinTime,
                AvgCounter.GetCounter(WorldManager.WorldManagerLoopString)?.MaxTime);


            middleText.AppendFormat("{0}\n{1}", worldManager.player.position, worldManager.playerChunkPosition);
#if UNITY_EDITOR
            middleText.AppendFormat("\n\nDrawCalls : {0}\nUsed Texture Memory : {1}\nrenderedTextureCount : {2}", UnityStats.drawCalls, UnityStats.usedTextureMemorySize / 1048576, UnityStats.usedTextureCount);
#endif

            rightText.AppendFormat("GPU memory : {0}\nSys Memory : {1}\n" + "TotalAllocatedMemory : {2}mb\nTotalReservedMemory : {3}mb\nTotalUnusedReservedMemory : {4}mb",
                SystemInfo.graphicsMemorySize,
                SystemInfo.systemMemorySize,
                Profiler.GetTotalAllocatedMemoryLong() / 1048576,
                Profiler.GetTotalReservedMemoryLong() / 1048576,
                Profiler.GetTotalUnusedReservedMemoryLong() / 1048576)
                .AppendFormat("\nTotalAllocatedMemoryForGraphicsDriver: {0}mb",
                Profiler.GetAllocatedMemoryForGraphicsDriver() / 1048576);

            leftSide.text = leftText.ToString();
            middleSide.text = middleText.ToString();
            rightSide.text = rightText.ToString();
            frames = 0;
            lastInterval = timeNow;
        }

    }
}

