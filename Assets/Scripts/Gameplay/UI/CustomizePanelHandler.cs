using System;
using UnityEngine;
using Mirror;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class CustomizePanelHandler : MonoBehaviour
{
    [SerializeField] private RectTransform scrollPanelRect = null;
    [SerializeField] private GameObject customizePanel = null;
    [SerializeField] private GameObject buttonPrefab = null;
    [SerializeField] private GameObject horizontalPanelPrefab = null;
    [SerializeField] private Camera playerPreviewCamera;

    [SerializeField] private Button selectedTabOnEnable = null;

    PlayerInfo playerInfo = null;
    PlayerPreviewCameraController playerPreviewCameraController = null;
    PlayerInfoDisplayer playerInfoDisplayer = null;

    Enumerators.CustomizeItem currentTab = Enumerators.CustomizeItem.None;

    private void OnEnable()
    {
        if (playerInfo == null)
        {
            playerInfo = NetworkClient.connection.identity.GetComponent<PlayerInfo>();         
            playerPreviewCameraController = NetworkClient.connection.identity.GetComponent<PlayerPreviewCameraController>();
        }
        playerPreviewCameraController.ChangePreviewCameraStatus(true);

        StartCoroutine(PressButton());
    }

    private IEnumerator PressButton()
    {
        yield return new WaitForSeconds(0.001f);

        selectedTabOnEnable.onClick.Invoke();
        selectedTabOnEnable.Select();
    }

    private void OnDisable()
    {
        OnTabChange(Enumerators.CustomizeItem.None);
        playerPreviewCameraController.ChangePreviewCameraStatus(false);
    }

    public void ChangeTab(int tab)
    {
        OnTabChange((Enumerators.CustomizeItem) tab);
    }

    public void OnTabChange(Enumerators.CustomizeItem tab)
    {
        if (tab == currentTab) { return; }
        
        ClearTab();

        if (currentTab == Enumerators.CustomizeItem.Color)
        {
            LobbyRoomManager.singleton.UpdateColorChangeListeners(false, OnColorAvailabilityChanges);
        }
        else if (tab == Enumerators.CustomizeItem.Color)
        {
            LobbyRoomManager.singleton.UpdateColorChangeListeners(true, OnColorAvailabilityChanges);
        }

        currentTab = tab;

        List<GameObject> buttons = new List<GameObject>();
        switch (tab)
        {
            case Enumerators.CustomizeItem.Color:
                foreach ((Color color, int playerId) tuple in LobbyRoomManager.singleton.GetColors())
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
        scrollPanelRect.position = new Vector3(scrollPanelRect.position.x, 0, scrollPanelRect.position.z);
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
        if (currentTab == Enumerators.CustomizeItem.Color)
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
        if (currentTab == Enumerators.CustomizeItem.Color)
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
        playerPreviewCameraController.ChangeRotation(left ? Enumerators.RotationType.Left : Enumerators.RotationType.Right);
    }

    public void StopPreviewRotate()
    {
        playerPreviewCameraController.ChangeRotation(Enumerators.RotationType.None);
    }
}