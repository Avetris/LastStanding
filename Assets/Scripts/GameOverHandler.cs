using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;

public class GameOverHandler : NetworkBehaviour
{
    public static event Action ServerOnGameOver;
    public static event Action<string> ClientOnGameOver;
    
    // private List<UnitBase> bases = new List<UnitBase>();

    #region Server

    public override void OnStartServer()
    {
        // UnitBase.ServerOnBaseSpawn += ServerHandleBaseSpawned;
        // UnitBase.ServerOnBaseDespawn += ServerHandleBaseDespawned;
    }

    public override void OnStopServer()
    {
        // UnitBase.ServerOnBaseSpawn -= ServerHandleBaseSpawned;
        // UnitBase.ServerOnBaseDespawn -= ServerHandleBaseDespawned;
    }

    #endregion

    #region Client

    [ClientRpc]
    private void RpcGameOver(string winner)
    {
        ClientOnGameOver?.Invoke(winner);
    }

    #endregion    
}
