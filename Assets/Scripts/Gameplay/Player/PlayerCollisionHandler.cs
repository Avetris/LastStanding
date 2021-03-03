using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class PlayerCollisionHandler : NetworkBehaviour
{
    [SerializeField] private GameObject playerUI = null;
    
    public GameObject actionTarget;

    public GameObject GeActionTarget()
    {
        return actionTarget;
    }

    private void Start() {
        if(hasAuthority)
        {
            playerUI.SetActive(true);
        }
    }
    
    #region Server

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if ("Usable".Equals(other.gameObject.tag))
        {
            actionTarget = other.gameObject;
            RpcUpdateActionButtonStatus(actionTarget, true);
        }
    }

    [ServerCallback]
    private void OnTriggerExit(Collider other)
    {
        if ("Usable".Equals(other.gameObject.tag) && actionTarget == other.gameObject)
        {
            RpcUpdateActionButtonStatus(actionTarget, false);
            actionTarget = null;
        }
    }

    #endregion

    #region Client
    [ClientRpc]
    private void RpcUpdateActionButtonStatus(GameObject actionBtn, bool status)
    {
        if(!hasAuthority) { return; }
        if(actionTarget == null)
        {
            actionTarget = actionBtn;
        }
        actionTarget.GetComponent<ActionObject>().SetIsPlayerNear(status);
    }

    #endregion


}