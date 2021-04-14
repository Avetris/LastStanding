using UnityEngine;
using Mirror;
using TMPro;
using System;
using System.Runtime.CompilerServices;

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
        CapsuleCollider capsuleCollider = GetComponentInChildren<CapsuleCollider>();

        if(death == 1.0f)
        {
            if (LobbyRoomManager.singleton.GetSetting(Enumerators.GameSetting.DeathCollisions, true))
            {
                capsuleCollider.center = new Vector3(-0.5f, 0.4f, -0.45f);
            }
            else
            {
                capsuleCollider.center = new Vector3(-0.5f, 0, -0.45f);
            }
            capsuleCollider.radius = 0.5f;
            capsuleCollider.direction = 0;
        }
        else
        {
            capsuleCollider.center = new Vector3(0, 1.1f, 0);
            capsuleCollider.radius = 0.35f;
            capsuleCollider.direction = 1;
        }


    }

    [ClientRpc]
    public void RpcUpdateMoveAnimator(Vector3 move, float turnAmount, float forwardAmount)
    {
        // update the animator parameters
        m_Animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
        m_Animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
    }
}