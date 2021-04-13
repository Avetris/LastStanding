using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerControlller : NetworkBehaviour
{
    [SerializeField] private float m_Speed = 200f;
    [SerializeField] private float m_RunSpeed = 500f;
    [SerializeField] private float m_RotationSpeed = 5f;
    [SerializeField] private float m_RotationRunSpeed = 10f;
    [SerializeField] float m_MovingTurnSpeed = 360;
    [SerializeField] float m_StationaryTurnSpeed = 180;
    [SerializeField] float m_RunCycleLegOffset = 0.2f;
    [SerializeField] private CharacterController m_Controller = null;

    PlayerPreviewCameraController m_PlayerPreviewCameraController = null;
    PlayerAnimationController m_PlayerAnimationController;
    Controls controls;

    Vector2 currentMoveDirection = Vector2.zero;
    private bool isRunning = false;

    public bool IsRunning()
    {
        return isRunning;
    }

    [ClientCallback]
    private void Start()
    {
        InitInput();
        m_PlayerPreviewCameraController = GetComponent<PlayerPreviewCameraController>();
        m_PlayerAnimationController = GetComponent<PlayerAnimationController>();
        GetComponent<PlayerInfo>().ClientOnIsAliveUpdated += HandleIsAliveUpdate;
    }

    private void OnDestroy()
    {
        GetComponent<PlayerInfo>().ClientOnIsAliveUpdated -= HandleIsAliveUpdate;
    }

    private void HandleIsAliveUpdate(bool isAlive)
    {
        if (isAlive && hasAuthority)
        {
            controls.Enable();
        }
        else
        {
            controls.Disable();
        }
    }

    [Client]
    private void InitInput()
    {
        controls = new Controls();

        controls.Player.Move.performed += SetMoveInput;
        controls.Player.Move.canceled += SetMoveInput;

        controls.Player.Run.performed += SetRunInput;
        controls.Player.Run.canceled += SetRunInput;

        controls.Player.Action.performed += SetDoActionInput;

        controls.Enable();
    }

    private void SetMoveInput(InputAction.CallbackContext ctx)
    {
        DialogDisplayHandler dialogHandler = FindObjectOfType<DialogDisplayHandler>();
        if (dialogHandler == null || dialogHandler.GetOpenedPanel() == Enumerators.DialogType.None)
        {
            currentMoveDirection = ctx.ReadValue<Vector2>();
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
            currentMoveDirection = Vector2.zero;
        }
        else
        {
            currentMoveDirection = Vector2.zero;
        }
    }

    private void SetRunInput(InputAction.CallbackContext ctx)
    {
        isRunning = ctx.ReadValue<float>() > 0;
    }

    private void SetDoActionInput(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() > 0)
        {
            DialogDisplayHandler dialogHandler = FindObjectOfType<DialogDisplayHandler>();
            if (dialogHandler == null || dialogHandler.GetOpenedPanel() == Enumerators.DialogType.None)
            {
                GetComponentInChildren<GameplayButtonsHandler>().OnClick();
            }
        }
    }

    #region Server
    [Command]
    public void CmdMove(Vector2 direction, bool isRunning)
    {
        Vector3 moveVector = direction[1] * Vector3.forward + direction[0] * Vector3.right;
        // pass all parameters to the character control script
        Move(moveVector * (isRunning ? 1f : 0.5f));

        float currentSpeed = isRunning ? m_RunSpeed : m_Speed;

        m_Controller.SimpleMove(moveVector * currentSpeed * Time.deltaTime);
    }

    [ServerCallback]
    public void Move(Vector3 move)
    {
        // convert the world relative moveInput vector into a local-relative
        // turn amount and forward amount required to head in the desired
        // direction.
        if (move.magnitude > 1f) move.Normalize();
        move = m_Controller.transform.transform.InverseTransformDirection(move);

        move = Vector3.ProjectOnPlane(move, Vector3.up);
        float turnAmount = Mathf.Atan2(move.x, move.z);
        float forwardAmount = move.z;

        // help the character turn faster (this is in addition to root rotation in the animation)
        float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, forwardAmount);
        m_Controller.transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);

        // send input and other state parameters to the animator
        m_PlayerAnimationController.RpcUpdateMoveAnimator(move, turnAmount, forwardAmount);
    }
    #endregion

    #region Client

    [ClientCallback]
    private void Update()
    {
        if (!hasAuthority) { return; }
        CmdMove(currentMoveDirection, isRunning);
    }

    public override void OnStartClient()
    {
        if (NetworkServer.active) { return; }
    }

    public override void OnStopClient()
    {

    }
    #endregion

}