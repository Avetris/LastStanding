using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovableState : MovableState
{
    private Controls m_Controls;


    public PlayerMovableState(
        Transform transform,
        PlayerInfo playerInfo,
        PlayerRagdollController playerRagdollController,
        PlayerAnimationController playerAnimationController,
        PlayerCharacterController playerCharacterController)
        : base(transform, playerInfo, playerRagdollController, playerAnimationController, playerCharacterController)
    {
    }

    #region Abstract Methods

    public override void Init()
    {
        m_Controls = new Controls();

        m_Controls.Player.Move.performed += SetMoveInput;
        m_Controls.Player.Move.canceled += SetMoveInput;

        m_Controls.Player.Run.performed += SetRunInput;
        m_Controls.Player.Run.canceled += SetRunInput;

        m_Controls.Player.Action.performed += SetDoActionInput;

        m_Controls.Enable();
    }

    public override void Destroy()
    {
        m_Controls.Player.Move.performed -= SetMoveInput;
        m_Controls.Player.Move.canceled -= SetMoveInput;

        m_Controls.Player.Run.performed -= SetRunInput;
        m_Controls.Player.Run.canceled -= SetRunInput;

        m_Controls.Player.Action.performed -= SetDoActionInput;
    }
    #endregion Abstract Methods

    private void SetMoveInput(InputAction.CallbackContext ctx)
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

    private void SetRunInput(InputAction.CallbackContext ctx)
    {
        m_PlayerInfo.SetRunning(ctx.ReadValue<float>() > 0);
    }
    private void SetDoActionInput(InputAction.CallbackContext ctx)
    {
        if (ctx.ReadValue<float>() > 0)
        {
            // DialogDisplayHandler dialogHandler = FindObjectOfType<DialogDisplayHandler>();
            // if (dialogHandler == null || dialogHandler.GetOpenedPanel() == Enumerators.DialogType.None)
            // {
            //     GetComponentInChildren<GameplayButtonsHandler>().OnClick();
            // }
        }
    }
}