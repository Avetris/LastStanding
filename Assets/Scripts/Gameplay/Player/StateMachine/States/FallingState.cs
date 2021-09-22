using UnityEngine;
using UnityEngine.InputSystem;

public class FallingState : State
{
    private Transform m_Transform;
    private PlayerInfo m_PlayerInfo;
    private PlayerRagdollController m_PlayerRagdollController;
    private PlayerAnimationController m_PlayerAnimationController;
    private PlayerCharacterController m_PlayerCharacterController;

    const float AirSpeed = 5f;		// determines the max speed of the character while airborne
    const float AirControl = 2f;	// determines the response speed of controlling the character while airborne

    public bool m_OnGround;
    float m_TurnAmount;
    float m_ForwardAmount;
    public Vector3 m_AirVelocity;
    private float m_Height = 0;


    public FallingState(Transform transform, PlayerInfo playerInfo, PlayerRagdollController playerRagdollController, PlayerAnimationController playerAnimationController, PlayerCharacterController playerCharacterController)
    {
        m_Transform = transform;
        m_PlayerInfo = playerInfo;
        m_PlayerRagdollController = playerRagdollController;
        m_PlayerAnimationController = playerAnimationController;
        m_PlayerCharacterController = playerCharacterController;
    }

    #region Abstract Methods

    public override void Init()
    {
        m_OnGround = false;
        m_Height = m_Transform.position.y;
    }

    public override void Update()
    {
        Vector3 move = CalculateMove(Vector3.zero);

        // if ragdolled, add a little move
        if (m_PlayerRagdollController != null && m_PlayerRagdollController.IsRagdolled)
            m_PlayerRagdollController.AddExtraMove(move * 100 * Time.deltaTime);

        m_OnGround = true; //!m_JumpPressed && m_PlayerCharacterController.IsGrounded();

        ConvertMove(move);             // converts the relative move vector into local turn & fwd values

        HandleAirborneVelocities(move);

        float newHeight = m_Transform.position.y;
        m_OnGround = Mathf.Abs(m_Height - newHeight) < 1.0f;
        m_Height = newHeight;

        // m_PlayerAnimationController.UpdateAnimator(m_ForwardAmount, m_TurnAmount, m_OnGround, CharacterVelocity); // send input and other state parameters to the animator
    }

    public override StateType NextState()
    {
        return m_OnGround ? StateType.None : StateType.None;
    }

    public override void Destroy() { }
    #endregion Abstract Methods

    private Vector3 CalculateMove(Vector2 direction)
    {
        Vector3 moveVector = direction[1] * Vector3.forward + direction[0] * Vector3.right;

        return moveVector * Constants.WalkSpeed * Time.deltaTime;
    }

    private void ConvertMove(Vector3 move)
    {
        if (move.magnitude > 1f) move.Normalize();
        move = m_Transform.InverseTransformDirection(move);
        move = Vector3.ProjectOnPlane(move, Vector3.up);

        m_TurnAmount = Mathf.Atan2(move.x, move.z);
        m_ForwardAmount = move.z;
    }

    private void HandleAirborneVelocities(Vector3 move)
    {
        Vector3 airMove = new Vector3(move.x * AirSpeed, m_AirVelocity.y, move.z * AirSpeed);
        m_AirVelocity = Vector3.Lerp(m_AirVelocity, airMove, Time.deltaTime * AirControl);
    }
}