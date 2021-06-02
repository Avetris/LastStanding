using System;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerCharacterController : NetworkBehaviour
{
    [SerializeField] private float m_Speed = 200f;
    [SerializeField] private float m_RunSpeed = 500f;
    [SerializeField] float m_MovingTurnSpeed = 360;
    [SerializeField] float m_StationaryTurnSpeed = 180;

    private CharacterController m_CharacterController = null;

    Vector2 m_CurrentMoveDirection = Vector2.zero;

    // constants:
    const float JumpPower = 5f;		// determines the jump force applied when jumping (and therefore the jump height)
    const float AirSpeed = 5f;		// determines the max speed of the character while airborne
    const float AirControl = 2f;	// determines the response speed of controlling the character while airborne
    const float StationaryTurnSpeed = 180f;	// additional turn speed added when the player is stationary (added to animation root rotation)
    const float MovingTurnSpeed = 360f;		// additional turn speed added when the player is moving (added to animation root rotation)
    const float RunCycleLegOffset = 0.2f;	// animation cycle offset (0-1) used for determining correct leg to jump off

    // parameters needed to control character
    bool _onGround; // Is the character on the ground
    Vector3 _moveInput;
    bool _crouch;
    bool _jump;
    float _turnAmount;
    float _forwardAmount;
    bool _enabled = true;
    protected Vector3 _airVelocity;
    protected bool _jumpPressed = false;

    [ClientCallback]
    private void Awake()
    {
        m_CharacterController = GetComponent<CharacterController>();
    }

    public void UpdatePlayerPosition(Vector3 deltaPos)
    {
        if (m_CharacterController.enabled)
        {
            m_CharacterController.Move(deltaPos);

            if (!m_CharacterController.isGrounded)
                return;
        }
        _airVelocity = Vector3.zero;
    }

    public bool IsGrounded()
    {
        return m_CharacterController.isGrounded;
    }

    public Vector3 GetVelocity()
    {
        return m_CharacterController.velocity;
    }

    public void Enable(bool enable)
    {
        m_CharacterController.enabled = enable;
    }
}
