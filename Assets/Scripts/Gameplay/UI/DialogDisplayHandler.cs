using System;
using System.Collections.Generic;
using UnityEngine;

public class DialogDisplayHandler : MonoBehaviour
{

    [SerializeField] private GameObject settingsPanel = null;
    [SerializeField] private GameObject customizePanel = null;
    [SerializeField] private GameObject roomSettingsPanel = null;

    private Constants.DialogType openedPanel = Constants.DialogType.None;

    public event Action<bool> OnDialogChange;

    private void Start()
    {
        // player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();     
    }

    public void ClosePanels()
    {
        settingsPanel?.SetActive(false);
        customizePanel?.SetActive(false);
        roomSettingsPanel?.SetActive(false);

        OnDialogChange?.Invoke(false);
    }


    public void OpenPanel(Constants.DialogType panelToOpen)
    {
        ClosePanels();

        bool open = true;

        switch (panelToOpen)
        {
            case Constants.DialogType.Settings: settingsPanel?.SetActive(true); break;
            case Constants.DialogType.RoomSettings: roomSettingsPanel?.SetActive(true); break;
            case Constants.DialogType.Customize: customizePanel?.SetActive(true); break;
            default: open = false; break;
        }

        if (open)
        {
            OnDialogChange?.Invoke(true);
        }
    }
}