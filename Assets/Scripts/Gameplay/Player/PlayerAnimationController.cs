using UnityEngine;
using Mirror;
using TMPro;
using System;

public class PlayerAnimationController : NetworkBehaviour
{
    [SerializeField] private Animator m_Animator = null;

    const float k_Half = 0.5f;

    private void Awake()
    {
        GetComponent<PlayerInfo>().ClientOnIsAliveUpdated += OnIsAliveChangeHandler;
    }

    private void OnDestroy()
    {
        GetComponent<PlayerInfo>().ClientOnIsAliveUpdated -= OnIsAliveChangeHandler;
    }

    public void OnIsAliveChangeHandler(bool isAlive)
    {
        m_Animator.SetBool("Die", !isAlive);
    }

    public void UpdateCollisionOnDeath(float death)
    {
        GetComponentInChildren<CapsuleCollider>().enabled = death == 1.0f;
        GetComponentInChildren<CharacterController>().enabled = death == 0.0f;

        Vector3 collisionCenter = GetComponentInChildren<CapsuleCollider>().center;

        if (((CustomNetworkManager)NetworkManager.singleton).GetSetting(Enumerators.GameSetting.DeathCollisions, true))
        {
            collisionCenter.y = .4f;
        }
        else
        {
            collisionCenter.y = 0f;
        }
        GetComponentInChildren<CapsuleCollider>().center = collisionCenter;
    }

    [ClientRpc]
    public void RpcUpdateMoveAnimator(Vector3 move, float turnAmount, float forwardAmount)
    {
        // update the animator parameters
        m_Animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
        m_Animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
    }
}