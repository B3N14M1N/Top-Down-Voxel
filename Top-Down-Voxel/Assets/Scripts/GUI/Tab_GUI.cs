using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class Tab_GUI : MonoBehaviour
{
    public Color SelectedColor;
    public Color UnselectedColor;
    private Button Button;
    public GameObject TabContent;

    public void Awake()
    {
        Button = GetComponent<Button>();
        Button.onClick.RemoveAllListeners();
        Button.onClick.AddListener(delegate { Selected(); });
    }

    public void Selected()
    {
        //Debug.Log("Selected tab: " + gameObject.name);
        transform.GetComponentInParent<TabManager_GUI>().OnTabChange();

        TabContent.SetActive(true);

        if (Button != null)
            Button.GetComponent<Image>().color = SelectedColor;
    }

    public void Unselected()
    {
        TabContent.SetActive(false);
        if(Button != null)
            Button.GetComponent<Image>().color = UnselectedColor;
    }
}
