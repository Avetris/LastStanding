using UnityEngine;
using Mirror;
using TMPro;
using System;
using System.Runtime.CompilerServices;
using UnityEngine.Localization.SmartFormat.GlobalVariables;

public class PlayerAnimationController : NetworkBehaviour
{
    const float RunCycleLegOffset = 0.2f;	// animation cycle offset (0-1) used for determining correct leg to jump off

    private Animator m_Animator = null;

    private PlayerCharacterController m_PlayerCharacterController = null;
    private PlayerRagdollController m_PlayerRagdollController = null;
    private PlayerController m_PlayerController = null;

    public bool m_FirstAnimatorFrame = true;  // needed for prevent changing position in first animation frame

    const float k_Half = 0.5f;

    public string currenAnimation;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        m_Animator = GetComponent<Animator>();
        m_PlayerController = GetComponent<PlayerController>();
        m_PlayerCharacterController = GetComponent<PlayerCharacterController>();
        m_PlayerRagdollController = GetComponent<PlayerRagdollController>();
    }

    private void Update() {
        currenAnimation = m_Animator.GetCurrentAnimatorClipInfo(0)[0].clip.name;
    }

    void OnAnimatorMove()
    {
        if (Time.deltaTime < Mathf.Epsilon)
            return;

        Vector3 deltaPos;
        Vector3 deltaGravity = Physics.gravity * Time.deltaTime;
        m_PlayerController.m_AirVelocity += deltaGravity;

        if (m_PlayerController.m_OnGround)
        {
            deltaPos = m_Animator.deltaPosition;
            deltaPos.y -= 5f * Time.deltaTime;
        }
        else
        {
            deltaPos = m_PlayerController.m_AirVelocity * Time.deltaTime;
        }

        if (m_FirstAnimatorFrame)
        {
            // if Animator just started, Animator move character
            // so you need to zeroing movement
            deltaPos = new Vector3(0f, deltaPos.y, 0f);
            m_FirstAnimatorFrame = false;
        }

        m_PlayerCharacterController.UpdatePlayerPosition(deltaPos);

        // apply animator rotation
        transform.rotation *= m_Animator.deltaRotation;
        m_PlayerController.m_JumpPressed = false;
    }


    public void UpdateAnimator(float forwardAmount, float turnAmount, bool onGround, Vector3 characterVelocity)
    {
        // Here we tell the animator what to do based on the current states and inputs.

        // update the animator parameters
        UpdateMoveAnimator(turnAmount, forwardAmount, onGround);
        if (!onGround) // if flying
        {
            m_Animator.SetFloat("Jump", characterVelocity.y);
        }
        else
        {
            // calculate which leg is behind, so as to leave that leg trailing in the jump animation
            // (This code is reliant on the specific run cycle offset in our animations,
            // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
            float runCycle = Mathf.Repeat(
                m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime + RunCycleLegOffset, 1);

            float jumpLeg = (runCycle < 0.5f ? 1 : -1) * forwardAmount;
            m_Animator.SetFloat("JumpLeg", jumpLeg);
        }
    }

    public bool IsGrounded()
    {
        return m_Animator.GetCurrentAnimatorStateInfo(0).IsName("Grounded");
    }

    public void UpdateMoveAnimator(float turnAmount, float forwardAmount, bool isOnGround)
    {
        // update the animator parameters
        m_Animator.SetFloat("Forward", forwardAmount, 0.1f, Time.deltaTime);
        m_Animator.SetFloat("Turn", turnAmount, 0.1f, Time.deltaTime);

        m_Animator.SetBool("OnGround", isOnGround);
    }

    public void SetGetUp(bool getUpBack)
    {
        m_Animator.SetTrigger(getUpBack ? "GetUpFromBack" : "GetUpFromBelly");
    }

    public void Enable(bool enable)
    {
        m_Animator.enabled = enable;
    }

    public Transform GetBone(HumanBodyBones bone)
    {
        return m_Animator.GetBoneTransform(bone);
    }
}