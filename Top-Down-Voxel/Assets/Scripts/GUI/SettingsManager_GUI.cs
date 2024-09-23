using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsManager_GUI : MonoBehaviour
{
    [Header("FPS settings")]
    public int maxFps = 30;
    public Slider slider;
    public TMP_Text sliderText;

    public void Awake()
    {
        Application.targetFrameRate = maxFps;
        sliderText.text = "FPS CAP: " + maxFps;
        slider.value = maxFps;

        slider.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
    }

    public void ValueChangeCheck()
    {
        if (slider.value != maxFps)
        {
            maxFps = (int)slider.value;
            Application.targetFrameRate = maxFps;
            sliderText.text = "FPS CAP: " + maxFps;
        }
    }

}
