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
    [SerializeField] private GameObject character;

    [SyncVar]
    public int playerId = 0;

    [SyncVar(hook = nameof(OnNameChangeHandler))]
    private string displayName = "";
    [SyncVar(hook = nameof(OnColorChangeHandler))]
    private Color displayColor = Color.clear;

    public event Action<Color> ClientOnColorUpdated;
    public event Action<string> ClientOnNameUpdated;

    private SyncDictionary<Enumerators.CustomizeItem, CharacterData> characterData = new SyncDictionary<Enumerators.CustomizeItem, CharacterData>();

    #region Getters / Setters

    public SyncDictionary<Enumerators.CustomizeItem, CharacterData> GetCharacterData()
    {
        return characterData;
    }

    public string GetDisplayName()
    {
        return displayName;
    }

    public void SetDisplayName(string name)
    {
        displayName = name;
    }

    public Color GetDisplayColor()
    {
        return displayColor;
    }

    public void SetDisplayColor(Color color)
    {
        displayColor = color;
    }

    public void SetPlayerId(int id)
    {
        playerId = id;
    }

    public int GetPlayerId()
    {
        return playerId;
    }    

    public Vector3 GetPlayerPosition()
    {
        return character.transform.position;
    }
    #endregion

    #region Server


    [Command]
    public void CmdSetDisplayName(string newName)
    {
        SetDisplayName(newName);
    }

    [Command]
    public void CmdSetDisplayColor(Color newColor)
    {
        if (!hasAuthority && !isServer) { return; }

        bool canChange = LobbyRoomManager.singleton.CanSetColor(playerId, displayColor, newColor);

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
            LobbyRoomManager.singleton.CanSetColor(playerId, displayColor, Color.clear);
        }
        base.OnStopServer();
    }

    #endregion
}