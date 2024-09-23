using System.Collections.Generic;
using UnityEngine;

public class TabManager_GUI : MonoBehaviour
{
    public List<Tab_GUI> tabs;
    public delegate void TabChanged();
    public event TabChanged OnTabChanged;

    public void Start()
    {
        for (int i = 0; i < tabs.Count; i++)
        {
            tabs[i].Unselected();
            if (i == 0)
                tabs[i].Selected();
        }

        OnTabChanged += CloseTabs;
    }

    private void CloseTabs()
    {
        foreach (var tab in tabs)
        {
            tab.Unselected();
        }
    }

    public void OnTabChange()
    {
        OnTabChanged?.Invoke();
    }
}
