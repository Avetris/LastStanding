using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class PlayerCollisionHandler : NetworkBehaviour
{
    [SerializeField] private Transform m_RootBone = null;

    private GameObject actionTarget;

    public GameObject GeActionTarget()
    {
        return actionTarget;
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

    public Transform GetHittedBone(Vector3 hitPosition)
    {
        float closestPos = Mathf.Infinity;
        Transform closestBone = null;
        foreach (Transform child in m_RootBone) {
            if(Vector3.Distance(child.position, hitPosition) < closestPos) {
                closestPos = Vector3.Distance(child.position, hitPosition);
                closestBone = child;
            }
        }

        return closestBone;
    }

    #endregion

    #region Client
    [ClientRpc]
    private void RpcUpdateActionButtonStatus(GameObject actionBtn, bool status)
    {
        if (!hasAuthority) { return; }

        if (actionTarget == null)
        {
            actionTarget = actionBtn;
        }
        actionTarget.GetComponent<ActionObject>().SetIsPlayerNear(status);
        if (!status)
        {
            actionTarget = null;
        }
    }

    #endregion
}
