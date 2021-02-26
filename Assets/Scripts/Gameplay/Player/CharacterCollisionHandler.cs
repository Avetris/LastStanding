using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class PlayerCollisionHandler : NetworkBehaviour
{

    [SerializeField] private Button useButton = null;

    private GameObject useTarget;
    private Player player;

    private void Start()
    {
        player = connectionToClient.identity.GetComponent<Player>();
    }

    #region Server

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (!hasAuthority) return;

        if ("Usable".Equals(other.gameObject.tag))
        {
            useTarget = other.gameObject;
            RpcUpdateUseButtonStatus(true);
        }
    }

    [ServerCallback]
    private void OnTriggerExit(Collider other)
    {
        if (!hasAuthority) return;

        if ("Usable".Equals(other.gameObject.tag) && useTarget == other.gameObject)
        {
            useTarget = null;
            RpcUpdateUseButtonStatus(false);
        }
    }

    #endregion

    #region Client
    [ClientRpc]
    private void RpcUpdateUseButtonStatus(bool status)
    {
        useButton.interactable = status;
    }

    #endregion


}