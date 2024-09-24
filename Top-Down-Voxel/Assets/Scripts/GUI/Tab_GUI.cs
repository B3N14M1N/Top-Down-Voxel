using System;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
[RequireComponent(typeof(Button))]
public class Tab_GUI : MonoBehaviour
{
    public Button Button {  get; private set; }
    public GameObject TabContent;

    public Color TabColor 
    {
        get
        {
            if (Button != null)
            {
                return Button.image.color;
            }
            return Color.white;
        }
        set
        {
            if (Button != null) 
            {
                Button.image.color = value;
            }
        }
    }

    public bool Active
    {
        get
        {
            if (TabContent == null)
                return false;
            return TabContent.activeSelf;
        }
        set
        {
            if (TabContent != null)
                TabContent.SetActive(value);
        }
    }

    public void Awake()
    {
        Button = GetComponent<Button>();
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(delegate { Selected(); });
    }

    public void Selected()
    {
        transform.GetComponentInParent<TabManager_GUI>().OnTabChange(this);
    }
}