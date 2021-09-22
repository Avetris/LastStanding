using NSubstitute.Exceptions;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerListeningState : State
{

    public PlayerListeningState()
    {
    }

    #region Abstract Methods

    public override void Init()
    {
    }

    public override void Update()
    {
    }

    public override StateType NextState()
    {
        return StateType.None;
    }

    public override void Destroy()
    {
    }
    #endregion Abstract Methods
}