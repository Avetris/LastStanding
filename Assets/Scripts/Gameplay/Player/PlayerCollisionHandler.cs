using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class PlayerCollisionHandler : NetworkBehaviour
{
    [SerializeField] private Transform m_RootBone = null;

    public GameObject m_ActionTarget;

    public GameObject GeActionTarget()
    {
        return m_ActionTarget;
    }
    #region Server

    [ServerCallback]
    private void OnTriggerEnter(Collider other)
    {
        if ("Usable".Equals(other.gameObject.tag))
        {
            m_ActionTarget = other.gameObject;
            RpcUpdateActionButtonStatus(m_ActionTarget, true);
        }
    }

    [ServerCallback]
    private void OnTriggerExit(Collider other)
    {
        if ("Usable".Equals(other.gameObject.tag) && m_ActionTarget == other.gameObject)
        {
            RpcUpdateActionButtonStatus(m_ActionTarget, false);
            m_ActionTarget = null;
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

        if (m_ActionTarget == null)
        {
            m_ActionTarget = actionBtn;
        }
        m_ActionTarget.GetComponent<ActionObject>().SetIsPlayerNear(status);
        if (!status)
        {
            m_ActionTarget = null;
        }
    }

    #endregion
}
