using UnityEngine;
using Mirror;
using System;

public class Player : NetworkBehaviour
{
    [SerializeField] private GameObject characterPrefab = null;

    public event Action ClientOnInfoUpdated;

    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;

    [SyncVar(hook = nameof(OnDisplayNameChangeHandler))]
    private string displayName;
    [SyncVar(hook = nameof(OnDisplayColorChangeHandler))]
    private Color displayColor;    
    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
    private bool isPartyOwner = false;

    public GameObject GetCharacterPrefab()
    {
        return characterPrefab;
    }

    public string GetDisplayName()
    {
        return displayName;
    }

    public void SetDisplayName(string name)
    {
        displayName = name;
    }

    public void SetDisplayColor(Color color)
    {
        displayColor = color;
    }

    public void SetPartyOwner(bool partyOwner)
    {
        isPartyOwner = partyOwner;
    }

    #region Server

    [Command]
    public void CmdStartGame()
    {

    }

    [Command]    
    public void CmdSetDisplayName(string newName)
    {
        SetDisplayName(newName);
    }

    [Server]
    public void SpawnPlayerCharacter(Vector3 respawnPosition)
    {
        GameObject characterInstance = Instantiate(characterPrefab, respawnPosition, Quaternion.identity);
        characterInstance.name = displayName;
        characterInstance.GetComponentInChildren<Renderer>().material.SetColor("_BaseColor", displayColor);
        NetworkServer.Spawn(characterInstance, connectionToClient);
    }

    #endregion

    #region Client
    public override void OnStartClient()
    {
        if (NetworkServer.active) { return; }

        DontDestroyOnLoad(gameObject);

        ((CustomNetworkManager)NetworkManager.singleton).Players.Add(this);
    }

    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();

        if (!isClientOnly) { return; }

        ((CustomNetworkManager)NetworkManager.singleton).Players.Remove(this);

        if (!hasAuthority) { return; }
    }

    public void OnDisplayNameChangeHandler(string oldName, string newName)
    {
        ClientOnInfoUpdated?.Invoke();
    }

    public void OnDisplayColorChangeHandler(Color oldColor, Color newColor)
    {
        ClientOnInfoUpdated?.Invoke();
    }    

    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
    {
        if(!hasAuthority) { return; }

        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
    }

    [ContextMenu("Change Name")]
    void ChangeName(string newName)
    {
        CmdSetDisplayName(newName);
    }
    #endregion

}