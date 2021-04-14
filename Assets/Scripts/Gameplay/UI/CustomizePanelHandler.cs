using System;
using UnityEngine;
using Mirror;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

public class CustomizePanelHandler : MonoBehaviour
{
    [SerializeField] private RectTransform m_ScrollPanelRect = null;
    [SerializeField] private GameObject m_CustomizePanel = null;
    [SerializeField] private GameObject m_ButtonPrefab = null;
    [SerializeField] private GameObject m_HorizontalPanelPrefab = null;

    [SerializeField] private Button m_SelectedTabOnEnable = null;

    PlayerInfo m_PlayerInfo = null;
    PlayerPreviewCameraController m_PlayerPreviewCameraController = null;

    Enumerators.CustomizeItem m_CurrentTab = Enumerators.CustomizeItem.None;

    private void OnEnable()
    {
        if (m_PlayerInfo == null)
        {
            m_PlayerInfo = NetworkClient.connection.identity.GetComponent<PlayerInfo>();         
            m_PlayerPreviewCameraController = NetworkClient.connection.identity.GetComponent<PlayerPreviewCameraController>();
        }
        m_PlayerPreviewCameraController.ChangePreviewCameraStatus(true);

        StartCoroutine(PressButton());
    }

    private IEnumerator PressButton()
    {
        yield return new WaitForSeconds(0.001f);

        m_SelectedTabOnEnable.onClick.Invoke();
        m_SelectedTabOnEnable.Select();
    }

    private void OnDisable()
    {
        OnTabChange(Enumerators.CustomizeItem.None);
        m_PlayerPreviewCameraController.ChangePreviewCameraStatus(false);
    }

    public void ChangeTab(int tab)
    {
        OnTabChange((Enumerators.CustomizeItem) tab);
    }

    public void OnTabChange(Enumerators.CustomizeItem tab)
    {
        if (tab == m_CurrentTab) { return; }
        
        ClearTab();

        if (m_CurrentTab == Enumerators.CustomizeItem.Color)
        {
            LobbyRoomManager.singleton.UpdateColorChangeListeners(false, OnColorAvailabilityChanges);
        }
        else if (tab == Enumerators.CustomizeItem.Color)
        {
            LobbyRoomManager.singleton.UpdateColorChangeListeners(true, OnColorAvailabilityChanges);
        }

        m_CurrentTab = tab;

        List<GameObject> buttons = new List<GameObject>();
        switch (tab)
        {
            case Enumerators.CustomizeItem.Color:
                foreach ((Color color, int playerId) tuple in LobbyRoomManager.singleton.GetColors())
                {
                    bool selected = tuple.playerId == m_PlayerInfo.GetPlayerId();
                    buttons.Add(CreateButton(tuple.color, null, selected || tuple.playerId == -1, selected));
                }
                break;
        }
        if(buttons.Count > 0)
        {
            CreateTab(buttons);
        }
        m_ScrollPanelRect.position = new Vector3(m_ScrollPanelRect.position.x, 0, m_ScrollPanelRect.position.z);
    }

    public void CreateTab(List<GameObject> buttons)
    {
        GameObject horizontalPanel = null;
        int horizontalPanelCount = 0;

        foreach (GameObject btn in buttons)
        {
            if (horizontalPanel == null)
            {
                horizontalPanel = Instantiate(m_HorizontalPanelPrefab, Vector3.zero, Quaternion.identity);
                horizontalPanelCount++;
            }

            btn.transform.SetParent(horizontalPanel.transform, false);

            if (horizontalPanel.transform.childCount == 3)
            {
                horizontalPanel.transform.SetParent(m_CustomizePanel.transform, false);
                horizontalPanel = null;
            }
        }
        if (horizontalPanel != null)
        {
            horizontalPanel.transform.SetParent(m_CustomizePanel.transform, false);
        }

        m_ScrollPanelRect.sizeDelta = new Vector2(m_ScrollPanelRect.sizeDelta[0], m_HorizontalPanelPrefab.GetComponent<RectTransform>().sizeDelta[1] * horizontalPanelCount);
    }

    public void ClearTab()
    {
        foreach (Transform child in m_CustomizePanel.transform)
        {
            Destroy(child.gameObject);
        }
    }

    private GameObject CreateButton(Color color, Texture texture, bool available, bool selected)
    {
        GameObject btn = Instantiate(m_ButtonPrefab, Vector3.zero, Quaternion.identity);

        CustomizeButtonHandler handler = btn.GetComponent<CustomizeButtonHandler>();
        handler.SetItemType(m_CurrentTab);
        handler.SetPlayerInfo(m_PlayerInfo);
        if (m_CurrentTab == Enumerators.CustomizeItem.Color)
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
        if (m_CurrentTab == Enumerators.CustomizeItem.Color)
        {
            foreach(CustomizeButtonHandler child in m_CustomizePanel.GetComponentsInChildren<CustomizeButtonHandler>())
            {
                if(color == child.GetColor())
                {
                    bool selected = m_PlayerInfo.GetPlayerId() == playerId;

                    child.ChangeAvailability(playerId == -1 || selected);
                    child.ChangeSelected(selected);
                }
            }
        }
    }

    private void SetColor(Color color)
    {
        m_PlayerInfo?.SetDisplayColor(color);
    }

    public void StartPreviewRotate(bool left)
    {
        m_PlayerPreviewCameraController.ChangeRotation(left ? Enumerators.RotationType.Left : Enumerators.RotationType.Right);
    }

    public void StopPreviewRotate()
    {
        m_PlayerPreviewCameraController.ChangeRotation(Enumerators.RotationType.None);
    }
}