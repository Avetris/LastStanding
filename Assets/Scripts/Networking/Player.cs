using UnityEngine;
using Mirror;

public class Player : NetworkBehaviour
{
    #region Server

    #endregion

    #region Client
    public override void OnStartClient()
    {
        if (NetworkServer.active) { return; }
        
        DontDestroyOnLoad(gameObject);

        ((CustomNetworkManager) NetworkManager.singleton).Players.Add(this);
    }

    public override void OnStopClient()
    {
        ClientOnInfoUpdated?.Invoke();

        if (!isClientOnly) { return; }
        
        ((CustomNetworkManager) NetworkManager.singleton).Players.Remove(this);

        if (!hasAuthority) { return; }

        Unit.AuthorityOnUnitSpawned -= AuthorityHandleUnitSpawned;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandleUnitDespawned;

        Building.AuthorityOnBuildingSpawned -= AuthorityHandleBuildingSpawned;
        Building.AuthorityOnBuildingDespawned -= AuthorityHandleBuildingDespawned;
    }
    #endregion
    
}