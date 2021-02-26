using UnityEngine;
using Mirror;
using System;

public enum CharacterParts {HEAD, BODY, LEG, FOOT};

public class Player : NetworkBehaviour
{
    private bool isPartyOwner = false;

    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;

    public void SetPartyOwner(bool partyOwner)
    {
        isPartyOwner = partyOwner;
    }

    #region Server

    [Command]
    public void CmdStartGame()
    {

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
        if (!isClientOnly) { return; }

        ((CustomNetworkManager)NetworkManager.singleton).Players.Remove(this);

        if (!hasAuthority) { return; }
    }

    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
    {
        if(!hasAuthority) { return; }

        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
    }
    #endregion
}