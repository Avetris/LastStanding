using UnityEngine;
using Mirror;
using System;
using TMPro;

public class RoomPlayer : NetworkRoomPlayer
{
    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        GetComponent<CharacterController>().gameObject.layer = LayerMask.NameToLayer("Player");
    }
    #region Server

    [Command]
    public void CmdStartGame()
    {
        if (!hasAuthority) { return; }

        ((CustomNetworkRoomManager)NetworkManager.singleton).StartGame();
    }
    #endregion

    #region Client

    private void OnAuthorityPartyOwnerStateUpdated(bool oldState, bool newState)
    {
        if (!hasAuthority) { return; }

        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
    }

    [Client]
    public void RpcChangeState(bool active)
    {
        Debug.Log($"OnActiveUpdated {active}");

        GetComponentInChildren<TMP_Text>().enabled = active;
        GetComponent<Animator>().enabled = active;
        GetComponent<CapsuleCollider>().enabled = active;
        GetComponent<SkinnedMeshRenderer>().enabled = active;
        GetComponent<PlayerAnimationController>().enabled = active;
        GetComponent<PlayerCollisionHandler>().enabled = active;
        GetComponent<PlayerController>().enabled = active;

        Debug.Log(GetComponent<PlayerController>().enabled);

        Debug.Log($"Is client: {isClient}");
    }
    #endregion
}