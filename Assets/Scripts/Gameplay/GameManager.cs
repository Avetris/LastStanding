using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GameManager : NetworkBehaviour
{

    private int currentPlayersInLife = 0;
    // Start is called before the first frame update
    void Start()
    {
        CustomNetworkManager.PlayerNumberUpdated += HandlePlayerNumberUpdate;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandlePlayerNumberUpdate(int newNumPlayers)
    {
        CheckActivePlayers();
    }

    private void CheckActivePlayers()
    {
        // currentPlayersInLife = 0;
        // foreach(Player player in ((CustomNetworkManager) NetworkManager.singleton).Players)
        // {
        //     if(player.GetComponent<>)
        //     {

        //     }
        // }
    }
}
