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
    [SerializeField] private Camera playerPreviewCamera;

    PlayerInfo playerInfo = null;
    PlayerPreviewCameraController playerPreviewCameraController = null;
    PlayerInfoDisplayer playerInfoDisplayer = null;

    Constants.CustomizeItem currentTab = Constants.CustomizeItem.None;

    private void OnEnable()
    {
        if (playerInfo == null)
        {
            playerInfo = NetworkClient.connection.identity.GetComponent<PlayerInfo>();         
            playerPreviewCameraController = NetworkClient.connection.identity.GetComponent<PlayerPreviewCameraController>();
        }
        playerPreviewCameraController.ChangePreviewCameraStatus(true);
        OnTabChange(Constants.CustomizeItem.Color);
    }

    private void OnDisable()
    {
        OnTabChange(Constants.CustomizeItem.None);
        playerPreviewCameraController.ChangePreviewCameraStatus(false);
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
                foreach ((Color color, int playerId) tuple in LobbyManager.singleton.GetColors())
                {
                    bool selected = tuple.playerId == playerInfo.GetPlayerId();
                    buttons.Add(CreateButton(tuple.color, null, selected || tuple.playerId == -1, selected));
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
    
    private void OnColorAvailabilityChanges(SyncDictionary<Color, int>.Operation op, Color color, int playerId)
    {
        if (currentTab == Constants.CustomizeItem.Color)
        {
            foreach(CustomizeButtonHandler child in customizePanel.GetComponentsInChildren<CustomizeButtonHandler>())
            {
                if(color == child.GetColor())
                {
                    bool selected = playerInfo.GetPlayerId() == playerId;

                    child.ChangeAvailability(playerId == -1 || selected);
                    child.ChangeSelected(selected);
                }
            }
        }
    }

    private void SetColor(Color color)
    {
        playerInfo?.SetDisplayColor(color);
    }

    public void StartPreviewRotate(bool left)
    {
        playerPreviewCameraController.ChangeRotation(left ? Constants.RotationType.Left : Constants.RotationType.Right);
    }

    public void StopPreviewRotate()
    {
        playerPreviewCameraController.ChangeRotation(Constants.RotationType.None);
    }
}