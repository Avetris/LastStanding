using UnityEngine;
using Mirror;
using System;

public struct CharacterData
{
    public string prefabName;
    public Color color;
}

public enum PlayerStatus { Alive, Dead, FallDown }

public class PlayerInfo : NetworkBehaviour
{
    [SerializeField] private Renderer m_Character;

    [SyncVar]
    private int m_PlayerId = 0;

    [SyncVar(hook = nameof(OnNameChangeHandler))]
    private string m_DisplayName = "";
    [SyncVar(hook = nameof(OnColorChangeHandler))]
    private Color m_DisplayColor = Color.clear;

    [SyncVar(hook = nameof(OnStatusChange))]
    public PlayerStatus m_Status = PlayerStatus.Alive;

    [SyncVar]
    public bool m_IsRunning = false;

    private SyncDictionary<Enumerators.CustomizeItem, CharacterData> m_CharacterData = new SyncDictionary<Enumerators.CustomizeItem, CharacterData>();

    public event Action<Color> ClientOnColorUpdated;
    public event Action<string> ClientOnNameUpdated;
    public event Action ClientOnStatusChange;

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

    public void SetRunning(bool isRunning)
    {
        m_IsRunning = isRunning;
    }

    public bool IsRunning()
    {
        return m_IsRunning;
    }

    [Command]
    public void CmdFallDown()
    {
        m_Status = PlayerStatus.FallDown;
    }

    public void Kill()
    {
        CmdKill();
    }

    [Command]
    private void CmdKill()
    {
        m_Status = PlayerStatus.Dead;
    }

    [Command]
    public void CmdResurrect()
    {
        m_Status = PlayerStatus.Alive;
    }

    public bool IsAlive()
    {
        return m_Status != PlayerStatus.Dead;
    }

    public bool HasFallenDown()
    {
        return m_Status == PlayerStatus.FallDown;
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
        m_Status = PlayerStatus.Alive;
    }


    [Command]
    public void CmdSetDisplayName(string newName)
    {
        SetDisplayName(newName);
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

    public void OnStatusChange(PlayerStatus oldStatus, PlayerStatus newStatus)
    {
        ClientOnStatusChange?.Invoke();
    }

    #endregion
}