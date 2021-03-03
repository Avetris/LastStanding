using System;
using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class CustomizePanelHandler : MonoBehaviour
{
    [SerializeField] private RectTransform scrollPanelRect = null;
    [SerializeField] private GameObject customizePanel = null;
    [SerializeField] private GameObject buttonPrefab = null;
    [SerializeField] private GameObject horizontalPanelPrefab = null;

    PlayerInfo playerInfo = null;

    Constants.CustomizeItem currentTab = Constants.CustomizeItem.None;

    private void OnEnable()
    {
        if (playerInfo == null)
        {
            playerInfo = NetworkClient.connection.identity.GetComponent<PlayerInfo>();
        }
        OnTabChange(Constants.CustomizeItem.Color);
    }

    private void OnDisable()
    {
        OnTabChange(Constants.CustomizeItem.None);
    }

    public void OnTabChange(Constants.CustomizeItem tab)
    {
        if (tab == currentTab) { return; }
        
        ClearTab();

        if (currentTab == Constants.CustomizeItem.Color)
        {
            LobbyManager.singleton.UpdateColorChangeListeners(false, OnColorAvailabilityChanges);
        }
        else if (tab == Constants.CustomizeItem.Color)
        {
            LobbyManager.singleton.UpdateColorChangeListeners(true, OnColorAvailabilityChanges);
        }

        currentTab = tab;

        List<GameObject> buttons = new List<GameObject>();
        switch (tab)
        {
            case Constants.CustomizeItem.Color:
                foreach ((Color color, bool available) tuple in LobbyManager.singleton.GetColors())
                {
                    bool selected = tuple.color == playerInfo.GetTempColor();
                    buttons.Add(CreateButton(tuple.color, null, selected || tuple.available, selected));
                }
                break;
        }
        if(buttons.Count > 0)
        {
            CreateTab(buttons);
        }
    }

    public void CreateTab(List<GameObject> buttons)
    {
        GameObject horizontalPanel = null;
        int horizontalPanelCount = 0;

        foreach (GameObject btn in buttons)
        {
            if (horizontalPanel == null)
            {
                horizontalPanel = Instantiate(horizontalPanelPrefab, Vector3.zero, Quaternion.identity);
                horizontalPanelCount++;
            }

            btn.transform.SetParent(horizontalPanel.transform, false);

            if (horizontalPanel.transform.childCount == 3)
            {
                horizontalPanel.transform.SetParent(customizePanel.transform, false);
                horizontalPanel = null;
            }
        }
        if (horizontalPanel != null)
        {
            horizontalPanel.transform.SetParent(customizePanel.transform, false);
        }

        scrollPanelRect.sizeDelta = new Vector2(scrollPanelRect.sizeDelta[0], horizontalPanelPrefab.GetComponent<RectTransform>().sizeDelta[1] * horizontalPanelCount);
    }

    public void ClearTab()
    {
        foreach (Transform child in customizePanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private GameObject CreateButton(Color color, Texture texture, bool available, bool selected)
    {
        GameObject btn = Instantiate(buttonPrefab, Vector3.zero, Quaternion.identity);

        CustomizeButtonHandler handler = btn.GetComponent<CustomizeButtonHandler>();
        handler.SetItemType(currentTab);
        handler.SetPlayerInfo(playerInfo);
        if (currentTab == Constants.CustomizeItem.Color)
        {
            handler.SetColor(color);
        }
        else
        {
            handler.SetImage(texture);
        }

        handler.ChangeSelected(selected);
        handler.ChangeAvailability(selected || available);

        return btn;
    }
    
    private void OnColorAvailabilityChanges(SyncDictionary<Color, bool>.Operation op, Color color, bool available)
    {
        if (currentTab == Constants.CustomizeItem.Color)
        {
            foreach(CustomizeButtonHandler child in customizePanel.GetComponentsInChildren<CustomizeButtonHandler>())
            {
                if(color == child.GetColor())
                {
                    bool selected = child.GetColor() == playerInfo.GetTempColor();

                    Debug.Log(playerInfo.GetTempColor());

                    child.ChangeAvailability(available || selected);
                    child.ChangeSelected(selected);
                }
            }
        }
    }

    private void SetColor(Color color)
    {
        playerInfo?.SetDisplayColor(color);
    }
}