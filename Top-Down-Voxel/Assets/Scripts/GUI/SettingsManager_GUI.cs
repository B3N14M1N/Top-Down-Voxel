using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class SettingsManager_GUI : MonoBehaviour
{
    [Header("FPS settings")]
    public int maxFps = 30;
    public Slider s_slider;
    public TMP_InputField if_MaxFps;

    [Header("Player settings")]
    public TMP_InputField if_RenderDistance;
    public TMP_InputField if_LoadDistance;
    public TMP_InputField if_ChunksProcessed;
    public TMP_InputField if_ChunksToLoad;
    public TMP_InputField if_TimeToLoadNextChunks;

    [Header("World settings")]
    public TMP_InputField if_Seed;
    public TMP_InputField if_ChunkWidth;
    public TMP_InputField if_ChunkHeight;

    public void Awake()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = maxFps;
        if_MaxFps.text = maxFps.ToString();
        s_slider.value = maxFps;

        s_slider.onValueChanged.AddListener(delegate { SliderValueChangeCheck(); });
        if_MaxFps.onSubmit.AddListener(delegate { InputFPSValueChangeCheck(); });
    }

    public void OnEnable()
    {
        Load();
    }
    public void Start()
    {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = maxFps;
        Load();
    }
    public void Load()
    {
        s_slider.value = maxFps;
        if_RenderDistance.text = PlayerSettings.RenderDistance.ToString();
        if_LoadDistance.text = PlayerSettings.LoadDistance.ToString();
        if_ChunksProcessed.text = PlayerSettings.ChunksProcessed.ToString();
        if_ChunksToLoad.text = PlayerSettings.ChunksToLoad.ToString();
        if_TimeToLoadNextChunks.text = PlayerSettings.TimeToLoadNextChunks.ToString();

        if_Seed.text = WorldSettings.Seed.ToString();
        if_ChunkWidth.text = WorldSettings.ChunkWidth.ToString();
        if_ChunkHeight.text = WorldSettings.ChunkHeight.ToString();
    }
    public void Save()
    {
        s_slider.value = maxFps;

        var valueInt = Convert.ToInt32(if_RenderDistance.text);
        PlayerSettings.RenderDistance = valueInt > 0 ? valueInt : PlayerSettings.RenderDistance;
        valueInt = Convert.ToInt32(if_LoadDistance.text);
        PlayerSettings.LoadDistance = valueInt > 0 ? valueInt : PlayerSettings.LoadDistance;
        valueInt = Convert.ToInt32(if_ChunksProcessed.text);
        PlayerSettings.ChunksProcessed = valueInt > 0 ? valueInt : PlayerSettings.ChunksProcessed;
        valueInt = Convert.ToInt32(if_ChunksToLoad.text);
        PlayerSettings.ChunksToLoad = valueInt > 0 ? valueInt : PlayerSettings.ChunksToLoad;
        var valueFloat = (float)Convert.ToDouble(if_TimeToLoadNextChunks.text);
        PlayerSettings.TimeToLoadNextChunks = valueFloat >= 0 ? valueFloat : PlayerSettings.TimeToLoadNextChunks;

        valueInt = Convert.ToInt32(if_Seed.text);
        WorldSettings.Seed = valueInt;
        valueInt = Convert.ToInt32(if_ChunkWidth.text);
        WorldSettings.ChunkWidth = valueInt > 0 ? valueInt : WorldSettings.ChunkWidth;
        valueInt = Convert.ToInt32(if_ChunkHeight.text);
        WorldSettings.ChunkHeight = valueInt > 0 ? valueInt : WorldSettings.ChunkHeight;
    }
    public void SliderValueChangeCheck()
    {
        if (s_slider.value != maxFps)
        {
            maxFps = (int)s_slider.value;
            Application.targetFrameRate = maxFps;
            if_MaxFps.text = maxFps.ToString();
        }
    }

    public void InputFPSValueChangeCheck()
    {
        if (!string.IsNullOrEmpty(if_MaxFps.text))
        {
            var newMax = int.Parse(if_MaxFps.text);
            maxFps = newMax >= s_slider.minValue ? newMax : maxFps;
            s_slider.value = maxFps;
            Application.targetFrameRate = maxFps;
            
        }
        if_MaxFps.text = maxFps.ToString();
    }

}
