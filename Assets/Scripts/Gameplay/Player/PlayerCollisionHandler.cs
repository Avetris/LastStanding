using UnityEngine;
using Mirror;
using System;


public class PlayerCollisionHandler : NetworkBehaviour
{
    public struct ArrowHitData
    {
        public string boneName;
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 hitDirection;
        public float impactEndTime;

        public ArrowHitData(string boneName, Vector3 position, Quaternion rotation, Vector3 hitDirection)
        {
            this.boneName = boneName;
            this.position = position;
            this.rotation = rotation;
            this.hitDirection = hitDirection;

            this.impactEndTime = Time.time + 0.25f;
        }
    }

    [SerializeField] private GameObject m_HittedArrowPrefab = null;

    [SyncVar(hook = nameof(OnArrowHit))]
    ArrowHitData m_HittedArrow = new ArrowHitData();
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

    [Command(requiresAuthority = false)]
    public void CmdHitArrow(string boneName, Vector3 arrowPosition, Quaternion arrowRotation, Vector3 hitDirection)
    {
        m_HittedArrow = new ArrowHitData(boneName, arrowPosition, arrowRotation, hitDirection);
    }

    private Transform FindBoneByName(Transform currentBone, string boneName)
    {
        Transform bone = currentBone.Find(boneName);

        if (bone == null)
        {
            foreach (Transform child in currentBone)
            {
                bone = FindBoneByName(child, boneName);
                if (bone != null)
                {
                    break;
                }
            }
        }
        return bone;
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

    public void OnArrowHit(ArrowHitData oldData, ArrowHitData newData)
    {
        Transform boneTransform = FindBoneByName(transform, newData.boneName);

        Instantiate(m_HittedArrowPrefab,
                    newData.position,
                    newData.rotation,
                    boneTransform);
        Transform bone = FindBoneByName(transform, m_HittedArrow.boneName);
        bone.GetComponent<Rigidbody>().AddForce(m_HittedArrow.hitDirection, ForceMode.Impulse);

    }
    #endregion
}
