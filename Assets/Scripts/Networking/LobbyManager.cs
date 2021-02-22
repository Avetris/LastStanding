using UnityEngine;
using Mirror;
using System;

public class LobbyManager : NetworkBehaviour {
    
    [SerializeField] private Transform[] respawnPositions = new Transform[10];

    private int maxPlayers = 10;
    private int currentPlayers;


    


}