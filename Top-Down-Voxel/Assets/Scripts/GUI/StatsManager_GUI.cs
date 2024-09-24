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
public class StatsManager_GUI : MonoBehaviour
{
    [Header("Stats Display")]
    private StringBuilder leftText;
    private StringBuilder rightText;
    public TMP_Text leftSide;
    public TMP_Text rightSide;

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
            rightText.Length = 0;

            leftText.AppendFormat("Time : {0} ms\nCurrent FPS: {1}\nAvgFPS: {2}", ms, fps, fpsav);

#if UNITY_EDITOR
            leftText.AppendFormat("\n\nDrawCalls : {0}\nUsed Texture Memory : {1}\nrenderedTextureCount : {2}", UnityStats.drawCalls, UnityStats.usedTextureMemorySize / 1048576, UnityStats.usedTextureCount);
#endif
            rightText.AppendFormat("GPU memory : {3}\nSys Memory : {4}\n" + "TotalAllocatedMemory : {0}mb\nTotalReservedMemory : {1}mb\nTotalUnusedReservedMemory : {2}mb",
                SystemInfo.graphicsMemorySize, SystemInfo.systemMemorySize,
                Profiler.GetTotalAllocatedMemoryLong() / 1048576,
                Profiler.GetTotalReservedMemoryLong() / 1048576,
                Profiler.GetTotalUnusedReservedMemoryLong() / 1048576)
                .AppendFormat("\nTotalAllocatedMemoryForGraphicsDriver: {0}mb", Profiler.GetAllocatedMemoryForGraphicsDriver() / 1048576);

            leftSide.text = leftText.ToString();
            rightSide.text = rightText.ToString();
            frames = 0;
            lastInterval = timeNow;
        }
    }
}

