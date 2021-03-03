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
    [SyncVar(hook = nameof(OnNameChangeHandler))]
    private string displayName = "";
    [SyncVar(hook = nameof(OnColorChangeHandler))]
    private Color displayColor = Color.clear;
    [SyncVar]
    private Color tempColor = Color.clear;

    public event Action<Color> ClientOnColorUpdated;
    public event Action<string> ClientOnNameUpdated;

    private SyncDictionary<Constants.CustomizeItem, CharacterData> characterData = new SyncDictionary<Constants.CustomizeItem, CharacterData>();

    #region Getters / Setters

    public SyncDictionary<Constants.CustomizeItem, CharacterData> GetCharacterData()
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
    
    public Color GetTempColor()
    {
        return tempColor; // == Color.clear ? displayColor : tempColor;
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

        tempColor = newColor;

        bool canChange = LobbyManager.singleton.CanSetColor(displayColor, newColor);

        if (canChange)
        {
            SetDisplayColor(newColor);
        }
        tempColor = Color.clear;
    }

    public override void OnStartServer()
    {
        if (!hasAuthority) { return; }

        Color newColor = LobbyManager.singleton.GetNextColor();

        SetDisplayColor(newColor);
    }

    public override void OnStopServer()
    {
        if (!hasAuthority) { return; }

        LobbyManager.singleton.CanSetColor(displayColor, Color.clear);
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

    #endregion
}