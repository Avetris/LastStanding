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

    private PlayerInfo m_PlayerInfo = null;

    private void Start()
    {
        m_PlayerInfo = GetComponent<PlayerInfo>();
    }

    public GameObject GeActionTarget()
    {
        return m_ActionTarget;
    }

    [ClientCallback]
    private void OnTriggerEnter(Collider other)
    {
        if (!hasAuthority) { return; }
        if ("Usable".Equals(other.gameObject.tag))
        {
            m_ActionTarget = other.gameObject;
            UpdateActionButtonStatus(m_ActionTarget, true);
        }
        else if ("Player".Equals(other.gameObject.tag) && m_PlayerInfo.IsAlive() && !m_PlayerInfo.HasFallenDown())
        {
            OnAnotherPlayerHit(other.gameObject);
        }
    }

    [ClientCallback]
    private void OnTriggerExit(Collider other)
    {
        if (!hasAuthority) { return; }
        if ("Usable".Equals(other.gameObject.tag) && m_ActionTarget == other.gameObject)
        {
            UpdateActionButtonStatus(m_ActionTarget, false);
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

    private void UpdateActionButtonStatus(GameObject actionBtn, bool status)
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

    /**
        |      Player 1     |      Player 1     |         |  
        |-------------------|-------------------|  Falls? |
        | Running |   Pos   | Running |   Pos   |         |
        |---------|---------|---------|---------|---------|
        |---------|---------|---------|---------|---------|
        |    X    |   b/f   |    X    |    f    |    X    |
        |---------|---------|---------|---------|---------|
        |    X    |    f    |    X    |   b/l   |         |
        |---------|---------|---------|---------|---------|
        |         |   b/f   |    X    |    f    |    X    |
        |---------|---------|---------|---------|---------|
        |    X    |    f    |         |   b/f   |         |
        |---------|---------|---------|---------|---------|
        |    X    |    f    |         |    l    |    X    |
        |---------|---------|---------|---------|---------|
        |         |    l    |    X    |    f    |         |
        |---------|---------|---------|---------|---------|
    **/
    private void OnAnotherPlayerHit(GameObject other)
    {
        PlayerInfo otherPlayerInfo = other.GetComponent<PlayerInfo>();
        if (otherPlayerInfo == null || otherPlayerInfo.netId == m_PlayerInfo.netId
        || !otherPlayerInfo.IsAlive() || otherPlayerInfo.HasFallenDown()) { return; }

        bool otherIsRunning = otherPlayerInfo.IsRunning();
        bool iAmRunning = m_PlayerInfo.IsRunning();

        float angle = Vector3.Angle(transform.forward, other.transform.position - transform.position);
        float angleTarget = Vector3.Angle(other.transform.forward, transform.position - other.transform.position);

        bool inFrontOfTarget = Mathf.Abs(angle) <= Constants.PlayerPushAngle;
        bool targetFrontOfUs = Mathf.Abs(angleTarget) <= Constants.PlayerPushAngle;

        bool laterally = !inFrontOfTarget && Mathf.Abs(angle) <= Constants.PlayerPushLateralAngle;
        bool targetLaterally = !targetFrontOfUs && Mathf.Abs(angleTarget) <= Constants.PlayerPushLateralAngle;

        // Debug.Log($"Angle {angle} -->  {angleTarget} || {laterally} --> {targetLaterally}");

        if ((iAmRunning && otherIsRunning && targetFrontOfUs) ||
            (!iAmRunning && otherIsRunning && targetFrontOfUs && !laterally) ||
            (iAmRunning && !otherIsRunning && targetLaterally))
        {
            m_PlayerInfo.CmdFallDown();
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
}
