using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizeTabHandler : MonoBehaviour
{
    CustomizePanelHandler m_CustomizePanelHandler;
    // Start is called before the first frame update
    void Start()
    {
        m_CustomizePanelHandler = FindObjectOfType<CustomizePanelHandler>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Enumerators.CustomizeItem tab = (Enumerators.CustomizeItem) (i + 1);
            transform.GetChild(i).GetComponent<Button>().onClick.AddListener(
                () => OnTabListener(tab));
        }
    }

    private void OnTabListener(Enumerators.CustomizeItem tab)
    {
        m_CustomizePanelHandler.ChangeTab(tab);
    }
}
