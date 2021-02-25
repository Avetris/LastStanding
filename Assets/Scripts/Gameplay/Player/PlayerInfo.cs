using UnityEngine;
using Mirror;
using TMPro;
using System;

public struct CharacterData
{
    public string prefabName;
    public Color color;
}

public class PlayerInfo : NetworkBehaviour
{
    [SyncVar(hook=nameof(OnNameChangeHandler))]
    private string displayName;
    [SyncVar(hook=nameof(OnColorChangeHandler))]
    private Color displayColor;

    public event Action<Color> ClientOnColorUpdated;
    public event Action<string> ClientOnNameUpdated;
    
    private SyncDictionary<CharacterParts, CharacterData> characterData  = new SyncDictionary<CharacterParts, CharacterData>();

    #region Getters / Setters

    public SyncDictionary<CharacterParts, CharacterData> GetCharacterData()
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
    #endregion

    #region Server


    [Command]    
    public void CmdSetDisplayColor(Color newColor)
    {
        SetDisplayColor(newColor);
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

    #endregion
}