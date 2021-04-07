using UnityEngine;
using Mirror;
using TMPro;
using System;

public class PlayerAnimationController : NetworkBehaviour
{   

    private Animator m_Animator = null;

    const float k_Half = 0.5f;

    private void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

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

    [ServerCallback]
    public void UpdateMoveAnimator(Vector3 move, float turnAmount, float forwardAmount)
    {
        // update the animator parameters
        m_Animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
        m_Animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);
    }
}