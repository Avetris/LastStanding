using UnityEngine;
using UnityEngine.InputSystem;

public class MovableState : State
{
    private Transform m_Transform;
    private PlayerAnimationController m_PlayerAnimationController;
    private PlayerCharacterController m_PlayerCharacterController;

    protected PlayerInfo m_PlayerInfo;
    protected private PlayerRagdollController m_PlayerRagdollController;

    // constants:
    const float StationaryTurnSpeed = 180f;	// additional turn speed added when the player is stationary (added to animation root rotation)
    const float MovingTurnSpeed = 360f;		// additional turn speed added when the player is moving (added to animation root rotation)

    // parameters needed to control character
    float m_TurnAmount;
    float m_ForwardAmount;

    bool m_OnGround = true;
    float m_Height = 0;

    protected Vector2 m_CurrentMoveDirection = Vector2.zero;

    public MovableState(Transform transform, PlayerInfo playerInfo, PlayerRagdollController playerRagdollController, PlayerAnimationController playerAnimationController, PlayerCharacterController playerCharacterController)
    {
        m_Transform = transform;
        m_PlayerInfo = playerInfo;
        m_PlayerRagdollController = playerRagdollController;
        m_PlayerAnimationController = playerAnimationController;
        m_PlayerCharacterController = playerCharacterController;

        m_Height = transform.position.y;
    }

    #region Abstract Methods

    public override void Init() { }

    public override void Update()
    {
        Vector3 move = CalculateMove(m_CurrentMoveDirection);

        // if ragdolled, add a little move
        if (m_PlayerRagdollController != null && m_PlayerRagdollController.IsRagdolled)
            m_PlayerRagdollController.AddExtraMove(move * 100 * Time.deltaTime);

        ApplyExtraTurnRotation();       // this is in addition to root rotation in the animations
        ConvertMove(move);             // converts the relative move vector into local turn & fwd values

        // m_PlayerAnimationController.UpdateAnimator(m_ForwardAmount, m_TurnAmount, m_OnGround, CharacterVelocity); // send input and other state parameters to the animator

        float newHeight = m_Transform.position.y;
        m_OnGround = Mathf.Abs(m_Height - newHeight) < 1.0f;
        m_Height = newHeight;
    }

    public override StateType NextState()
    {
        return m_OnGround ? StateType.None : StateType.Falling;
    }

    public override void Destroy() { }
    #endregion Abstract Methods

    private Vector3 CalculateMove(Vector2 direction)
    {
        Vector3 moveVector = direction[1] * Vector3.forward + direction[0] * Vector3.right;

        float currentSpeed = m_PlayerInfo.IsRunning() ? Constants.RunSpeed : Constants.WalkSpeed;

        return moveVector * currentSpeed * Time.deltaTime;
    }

    private void ConvertMove(Vector3 move)
    {
        if (move.magnitude > 1f) move.Normalize();
        move = m_Transform.InverseTransformDirection(move);
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
        m_Transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
    }
}