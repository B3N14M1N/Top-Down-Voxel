using System;
using UnityEditor;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    [Header("Screen")]
    public KeyCode screenWindowKeyCode = KeyCode.F11;
    [Header("Stats")]
    public KeyCode statsKeyCode = KeyCode.F3;
    public GameObject Pannel;

    public void Start()
    {
        Cursor.visible = false;
    }
    public void Update()
    {
        if (Input.GetKeyDown(statsKeyCode))
        {
            Pannel.SetActive(!Pannel.activeSelf);
            Cursor.visible = Pannel.activeSelf;
        }
        if (Input.GetKeyDown(screenWindowKeyCode))
        {
            Screen.fullScreen = !Screen.fullScreen;
        }
    }

    public void Quit()
    {
#if UNITY_EDITOR
        EditorUtility.UnloadUnusedAssetsImmediate();
        GC.Collect();
#endif
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#elif UNITY_WEBPLAYER
        Application.OpenURL(webplayerQuitURL);
#else
        Application.Quit();
#endif
    }
}
