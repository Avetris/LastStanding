using System.Collections.Generic;
using UnityEngine;

public class DialogDisplayHandler : MonoBehaviour {

    public enum PanelType{ None, Settings, RoomSettings, Customize}

    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject customizePanel;
    [SerializeField] private GameObject roomSettingsPanel;

    private PanelType openedPanel = PanelType.None;
    
    private void Start() {
        // player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();     
    }

    public void ClosePanels()
    {
        settingsPanel?.SetActive(false);
        customizePanel?.SetActive(false);
        roomSettingsPanel?.SetActive(false);
    }    

    
    public void OpenPanel(PanelType panelToOpen)
    {
        ClosePanels();

        switch(panelToOpen)
        {
            case PanelType.Settings: settingsPanel?.SetActive(true); break;
            case PanelType.RoomSettings: roomSettingsPanel?.SetActive(true); break;
            case PanelType.Customize: customizePanel?.SetActive(true); break;
        }
    }    
}