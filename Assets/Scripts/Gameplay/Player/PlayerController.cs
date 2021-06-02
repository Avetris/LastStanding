using System;
using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerController : NetworkBehaviour
{
    [SerializeField] private float m_Speed = 200f;
    [SerializeField] private float m_RunSpeed = 500f;
    [SerializeField] float m_MovingTurnSpeed = 360;
    [SerializeField] float m_StationaryTurnSpeed = 180;

    Controls m_Controls;

    PlayerInfo m_PlayerInfo = null;
    PlayerPreviewCameraController m_PlayerPreviewCameraController = null;
    PlayerAnimationController m_PlayerAnimationController = null;
    PlayerCharacterController m_PlayerCharacterController = null;
    PlayerRagdollController m_PlayerRagdollController = null;

    Vector2 m_CurrentMoveDirection = Vector2.zero;

    // constants:
    const float JumpPower = 5f;		// determines the jump force applied when jumping (and therefore the jump height)
    const float AirSpeed = 5f;		// determines the max speed of the character while airborne
    const float AirControl = 2f;	// determines the response speed of controlling the character while airborne
    const float StationaryTurnSpeed = 180f;	// additional turn speed added when the player is stationary (added to animation root rotation)
    const float MovingTurnSpeed = 360f;		// additional turn speed added when the player is moving (added to animation root rotation)

    // parameters needed to control character
    public bool m_OnGround; // Is the character on the ground
    bool m_Jump;
    float m_TurnAmount;
    float m_ForwardAmount;
    public Vector3 m_AirVelocity;
    public bool m_JumpPressed = false;

    public Vector3 CharacterVelocity { get { return m_OnGround ? PlayerVelocity : m_AirVelocity; } }

    private void Awake()
    {
        m_PlayerInfo = GetComponent<PlayerInfo>();
        m_PlayerPreviewCameraController = GetComponent<PlayerPreviewCameraController>();
        m_PlayerAnimationController = GetComponent<PlayerAnimationController>();
        m_PlayerCharacterController = GetComponent<PlayerCharacterController>();
        m_PlayerRagdollController = GetComponent<PlayerRagdollController>();
        m_PlayerInfo.ClientOnStatusChange += HandleStatusChange;
    }

    [ClientCallback]
    private void Start()
    {
        InitInput();
    }

    private void OnDestroy()
    {
        m_PlayerInfo.ClientOnStatusChange -= HandleStatusChange;
    }

    [Client]
    private void InitInput()
    {
        m_Controls = new Controls();

        m_Controls.Player.Move.performed += SetMoveInput;
        m_Controls.Player.Move.canceled += SetMoveInput;

        m_Controls.Player.Run.performed += SetRunInput;
        m_Controls.Player.Run.canceled += SetRunInput;

        m_Controls.Player.Action.performed += SetDoActionInput;

        HandleStatusChange();
    }

    private void HandleStatusChange()
    {
        if (hasAuthority && m_PlayerInfo.IsAlive())
        {
            m_Controls.Enable();
        }
        else
        {
            m_Controls.Disable();
        }
    }

    private void SetMoveInput(InputAction.CallbackContext ctx)
    {
        if (LobbyRoomManager.singleton.IsPaused()) { return; }

        DialogDisplayHandler dialogHandler = FindObjectOfType<DialogDisplayHandler>();
        if (dialogHandler == null || dialogHandler.GetOpenedPanel() == Enumerators.DialogType.None)
        {
            if (!m_PlayerInfo.HasFallenDown())
            {
                m_CurrentMoveDirection = ctx.ReadValue<Vector2>();
            }
            else
            {
                m_PlayerRagdollController.IsRagdolled = false;
                m_CurrentMoveDirection = Vector2.zero;
                m_PlayerInfo.SetRunning(false);
            }
        }
        else if (dialogHandler != null && dialogHandler.GetOpenedPanel() == Enumerators.DialogType.Customize)
        {
            float previewRotation = ctx.ReadValue<Vector2>().x;
            if (previewRotation != 0)
            {
                m_PlayerPreviewCameraController.ChangeRotation(
                    previewRotation > 0 ? Enumerators.RotationType.Right : Enumerators.RotationType.Left);
            }
            else
            {
                m_PlayerPreviewCameraController.ChangeRotation(Enumerators.RotationType.None);
            }
            m_CurrentMoveDirection = Vector2.zero;
        }
        else
        {
            m_CurrentMoveDirection = Vector2.zero;
        }
    }

    private void SetRunInput(InputAction.CallbackContext ctx)
    {
        m_PlayerInfo.SetRunning(ctx.ReadValue<float>() > 0);
    }

    private void SetDoActionInput(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() > 0)
        {
            DialogDisplayHandler dialogHandler = FindObjectOfType<DialogDisplayHandler>();
            // if (dialogHandler == null || dialogHandler.GetOpenedPanel() == Enumerators.DialogType.None)
            // {
            //     GetComponentInChildren<GameplayButtonsHandler>().OnClick();
            // }
        }
    }

    protected Vector3 PlayerVelocity { get { return m_PlayerCharacterController.GetVelocity(); } }

    private bool PlayerTouchGound()
    {
        return m_PlayerCharacterController.IsGrounded();
    }

    private void FixedUpdate()
    {
        Vector3 move = CalculateMove(m_CurrentMoveDirection);

        // pass all parameters to the character control script
        m_Jump = m_JumpPressed;
        m_JumpPressed = false;

        // if ragdolled, add a little move
        if (m_PlayerRagdollController != null && m_PlayerRagdollController.IsRagdolled)
            m_PlayerRagdollController.AddExtraMove(move * 100 * Time.deltaTime);

        m_OnGround = true; //!m_JumpPressed && PlayerTouchGound();

        ApplyExtraTurnRotation();       // this is in addition to root rotation in the animations
        ConvertMove(move);             // converts the relative move vector into local turn & fwd values

        // control and velocity handling is different when grounded and airborne:
        if (m_OnGround)
            HandleGroundedVelocities();
        else
            HandleAirborneVelocities(move);

        m_PlayerAnimationController.UpdateAnimator(m_ForwardAmount, m_TurnAmount, m_OnGround, CharacterVelocity); // send input and other state parameters to the animator
    }

    private Vector3 CalculateMove(Vector2 direction)
    {
        Vector3 moveVector = direction[1] * Vector3.forward + direction[0] * Vector3.right;

        float currentSpeed = m_PlayerInfo.IsRunning() ? m_RunSpeed : m_Speed;

        return moveVector * currentSpeed * Time.deltaTime;
    }

    private void HandleGroundedVelocities()
    {
        // check whether conditions are right to allow a jump
        if (!(m_Jump & m_PlayerAnimationController.IsGrounded()))
            return;

        // jump!
        Vector3 newVelocity = CharacterVelocity;
        newVelocity.y += JumpPower;
        m_AirVelocity = newVelocity;

        m_Jump = false;
        m_OnGround = false;
        m_JumpPressed = true;
    }

    private void ConvertMove(Vector3 move)
    {
        if (move.magnitude > 1f) move.Normalize();
        move = transform.InverseTransformDirection(move);
        move = Vector3.ProjectOnPlane(move, Vector3.up);

        m_TurnAmount = Mathf.Atan2(move.x, move.z);
        m_ForwardAmount = move.z;
    }

    private void ApplyExtraTurnRotation()
    {
        if (!m_PlayerAnimationController.IsGrounded())
            return;

        // help the character turn faster (this is in addition to root rotation in the animation)
        float turnSpeed = Mathf.Lerp(StationaryTurnSpeed, MovingTurnSpeed,
                                     m_ForwardAmount);
        transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
    }

    private void HandleAirborneVelocities(Vector3 move)
    {
        Vector3 airMove = new Vector3(move.x * AirSpeed, m_AirVelocity.y, move.z * AirSpeed);
        m_AirVelocity = Vector3.Lerp(m_AirVelocity, airMove, Time.deltaTime * AirControl);
    }

    public void CharacterEnable(bool enable)
    {
        m_PlayerCharacterController.Enable(enable);
        if (enable)
            m_PlayerAnimationController.m_FirstAnimatorFrame = true;
    }
}
