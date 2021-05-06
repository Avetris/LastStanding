using UnityEngine;
using Mirror;
using System;

public struct CharacterData
{
    public string prefabName;
    public Color color;
}

public class PlayerInfo : NetworkBehaviour
{
    [SerializeField] private Renderer m_Character;

    [SyncVar]
    private int m_PlayerId = 0;

    [SyncVar(hook = nameof(OnNameChangeHandler))]
    private string m_DisplayName = "";
    [SyncVar(hook = nameof(OnColorChangeHandler))]
    private Color m_DisplayColor = Color.clear;

    [SyncVar(hook = nameof(OnIsAliveChange))]
    private bool m_IsAlive = true;

    private SyncDictionary<Enumerators.CustomizeItem, CharacterData> m_CharacterData = new SyncDictionary<Enumerators.CustomizeItem, CharacterData>();

    public event Action<Color> ClientOnColorUpdated;
    public event Action<string> ClientOnNameUpdated;
    public event Action<bool> ClientOnIsAliveUpdated;

    #region Getters / Setters

    public SyncDictionary<Enumerators.CustomizeItem, CharacterData> GetCharacterData()
    {
        return m_CharacterData;
    }

    public string GetDisplayName()
    {
        return m_DisplayName;
    }

    public void SetDisplayName(string name)
    {
        m_DisplayName = name;
    }

    public Color GetDisplayColor()
    {
        return m_DisplayColor;
    }

    public void SetDisplayColor(Color color)
    {
        m_DisplayColor = color;
    }

    public void SetPlayerId(int id)
    {
        m_PlayerId = id;
    }

    public int GetPlayerId()
    {
        return m_PlayerId;
    }    

    public Vector3 GetPlayerPosition()
    {
        Vector3 pos = transform.position;
        pos.y = m_Character.bounds.center.y;
        return pos;
    }

    public void Kill()
    {
        m_IsAlive = false;
    }

    public void Resurrect()
    {
        m_IsAlive = true;
    }

    public bool IsAlive()
    {
        return m_IsAlive;
    }
    #endregion

    #region Server

    public void SetData(PlayerInfo playerInfo)
    {
        m_CharacterData = playerInfo.m_CharacterData;
        m_Character = playerInfo.m_Character;
        m_DisplayName = playerInfo.m_DisplayName;
        m_DisplayColor = playerInfo.GetDisplayColor();
        m_PlayerId = playerInfo.m_PlayerId;
        m_IsAlive = true;
    }


    [Command]
    public void CmdSetDisplayName(string newName)
    {
        SetDisplayName(newName);
    }

    [Command]
    public void CmdSetDisplayColor(Color newColor)
    {
        if (!hasAuthority && !isServer) { return; }

        bool canChange = LobbyRoomManager.singleton.CanSetColor(m_PlayerId, m_DisplayColor, newColor);

        if (canChange)
        {
            SetDisplayColor(newColor);
        }
    }
    #endregion

    #region Client

    public void OnNameChangeHandler(string oldName, string newName)
    {
        ClientOnNameUpdated?.Invoke(newName);
    }

    public void OnColorChangeHandler(Color oldColor, Color newColor)
    {
        ClientOnColorUpdated?.Invoke(newColor);
    }

    public override void OnStopServer()
    {
        if(LobbyRoomManager.singleton != null)
        {
            LobbyRoomManager.singleton.CanSetColor(m_PlayerId, m_DisplayColor, Color.clear);
        }
        base.OnStopServer();
    }

    public void OnIsAliveChange(bool oldIsAlive, bool newIsAlive)
    {
        ClientOnIsAliveUpdated?.Invoke(newIsAlive);
    }

    #endregion
}