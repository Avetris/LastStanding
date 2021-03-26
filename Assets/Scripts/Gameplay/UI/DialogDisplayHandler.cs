using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogDisplayHandler : MonoBehaviour
{

    [SerializeField] private GameObject settingsPanel = null;
    [SerializeField] private GameObject customizePanel = null;
    [SerializeField] private GameObject roomSettingsPanel = null;

    private Enumerators.DialogType openedPanel = Enumerators.DialogType.None;

    public event Action<bool> OnDialogChange;

    private void Start()
    {
        // player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();     
    }

    public Enumerators.DialogType GetOpenedPanel()
    {
        return openedPanel;
    }

    public void ClosePanels()
    {
        settingsPanel?.SetActive(false);
        customizePanel?.SetActive(false);
        roomSettingsPanel?.SetActive(false);

        OnDialogChange?.Invoke(false);
        openedPanel = Enumerators.DialogType.None;
    }


    public void OpenPanel(Enumerators.DialogType panelToOpen)
    {
        ClosePanels();

        bool open = true;

        switch (panelToOpen)
        {
            case Enumerators.DialogType.Settings: settingsPanel?.SetActive(true); break;
            case Enumerators.DialogType.RoomSettings: roomSettingsPanel?.SetActive(true); break;
            case Enumerators.DialogType.Customize: customizePanel?.SetActive(true); break;
            default: open = false; break;
        }

        if (open)
        {
            openedPanel = panelToOpen;
            OnDialogChange?.Invoke(true);
        }
    }
}