using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class CharacterCollisionHandler : NetworkBehaviour
{

    [SerializeField] private Button actionButton = null;

    private GameObject actionTarget;
    
    #region Server

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if ("Usable".Equals(other.gameObject.tag))
        {
            actionTarget = other.gameObject;
            RpcUpdateActionButtonStatus(true);
        }
    }

    [ServerCallback]
    private void OnTriggerExit(Collider other)
    {
        if ("Usable".Equals(other.gameObject.tag) && actionTarget == other.gameObject)
        {
            actionTarget = null;
            RpcUpdateActionButtonStatus(false);
        }
    }

    #endregion

    #region Client
    [ClientRpc]
    private void RpcUpdateActionButtonStatus(bool status)
    {
        if(!hasAuthority) { return; }
        actionButton.interactable = status;
    }

    #endregion


}