using UnityEngine;
using UnityEngine.PlayerLoop;

public class PlayerStateMachine : MonoBehaviour
{
    private PlayerInfo m_PlayerInfo;
    private PlayerRagdollController m_PlayerRagdollController;
    private PlayerAnimationController m_PlayerAnimationController;
    private PlayerCharacterController m_PlayerCharacterController;

    private State m_CurrentState;

    private void Start()
    {
        ChangeState(StateType.None);
    }


    private void Update()
    {
        if (m_CurrentState != null)
        {
            m_CurrentState.Update();
            ChangeState(m_CurrentState.NextState());
        }
    }

    public void ChangeState(StateType stateType)
    {
        State nextState = null;
        switch (stateType)
        {
            case StateType.Listening: nextState = new PlayerListeningState(); break;
            case StateType.Moving: nextState = new PlayerMovableState(transform, m_PlayerInfo, m_PlayerRagdollController, m_PlayerAnimationController, m_PlayerCharacterController); break;
            case StateType.Falling: nextState = new FallingState(transform, m_PlayerInfo, m_PlayerRagdollController, m_PlayerAnimationController, m_PlayerCharacterController); break;
        }

        if (nextState != null)
        {
            m_CurrentState?.Destroy();
            m_CurrentState = nextState;
        }
    }

    public void CharacterEnable(bool enable)
    {
        m_PlayerCharacterController.Enable(enable);
        if (enable)
            m_PlayerAnimationController.m_FirstAnimatorFrame = true;
    }
}