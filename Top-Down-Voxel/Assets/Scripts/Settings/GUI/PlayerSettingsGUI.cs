using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerSettingsGUI : MonoBehaviour, ISettings
{
    [Header("Player settings")]
    public TMP_InputField InputRenderDistance;
    public TMP_InputField InputLoadDistance;
    public TMP_InputField InputChunksProcessed;
    public TMP_InputField InputChunksToLoad;
    public TMP_InputField InputTimeToLoadNextChunks;

    public void OnEnable()
    {
        Load();
    }

    public void Load()
    {
        InputRenderDistance.text = PlayerSettings.RenderDistance.ToString();
        InputLoadDistance.text = PlayerSettings.LoadDistance.ToString();
        InputChunksProcessed.text = PlayerSettings.ChunksProcessed.ToString();
        InputChunksToLoad.text = PlayerSettings.ChunksToLoad.ToString();
        InputTimeToLoadNextChunks.text = PlayerSettings.TimeToLoadNextChunks.ToString();
    }
    public void Save()
    {
        var valueInt = Convert.ToInt32(InputRenderDistance.text);
        PlayerSettings.RenderDistance = valueInt > 0 ? valueInt : PlayerSettings.RenderDistance;
        valueInt = Convert.ToInt32(InputLoadDistance.text);
        PlayerSettings.LoadDistance = valueInt > 0 ? valueInt : PlayerSettings.LoadDistance;
        valueInt = Convert.ToInt32(InputChunksProcessed.text);
        PlayerSettings.ChunksProcessed = valueInt > 0 ? valueInt : PlayerSettings.ChunksProcessed;
        valueInt = Convert.ToInt32(InputChunksToLoad.text);
        PlayerSettings.ChunksToLoad = valueInt > 0 ? valueInt : PlayerSettings.ChunksToLoad;
        var valueFloat = (float)Convert.ToDouble(InputTimeToLoadNextChunks.text);
        PlayerSettings.TimeToLoadNextChunks = valueFloat >= 0 ? valueFloat : PlayerSettings.TimeToLoadNextChunks;
    }
}
