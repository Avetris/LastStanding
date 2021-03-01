using UnityEngine;
using Mirror;
using System;

public class LobbyManager : NetworkBehaviour {

    private LobbyUIHandler lobbyUIHandler;
    
    private int maxPlayers = 10;
    private int currentPlayers;

    private void Start()
    {
        
        
        CustomNetworkManager.PlayerNumberUpdated += HandleClientConnect;
    }

    private void OnDestroy()
    {
        CustomNetworkManager.PlayerNumberUpdated -= HandleClientConnect;
    }
    
    [Server]
    private void HandleClientConnect(int playerCount)
    {
        bool enabled = playerCount >= 2;

        lobbyUIHandler.RpcChangeStartGameStatus(enabled);
    }
}