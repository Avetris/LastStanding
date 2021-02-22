using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUI = null;
    [SerializeField] private Button startGameButton = null;
    [SerializeField] private TMP_Text[] playerNameTexts = new TMP_Text[4];

    private void Start()
    {
        CustomNetworkManager.ClientOnConnected += HandleClientConnect;
        Player.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
        // Player.ClientOnInfoUpdated += ClientHandleInfoUpdated;
    }

    private void OnDestroy()
    {
        CustomNetworkManager.ClientOnConnected -= HandleClientConnect;
        Player.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
    }

    private void AuthorityHandlePartyOwnerStateUpdated(bool state)
    {
        startGameButton.gameObject.SetActive(state);
    }

    private void HandleClientConnect()
    {
        lobbyUI.SetActive(true);
    }

    private void ClientHandleInfoUpdated()
    {
        List<Player> players = ((CustomNetworkManager)NetworkManager.singleton).Players;

        Debug.Log($"Players Count {players.Count}");

        for(int i = 0; i < playerNameTexts.Length; i++)
        {
            if(i < players.Count)
            {
                playerNameTexts[i].text = players[i].GetDisplayName();
            }
            else
            {
                playerNameTexts[i].text = "Waiting For Players...";
            }
        }

        startGameButton.interactable = players.Count > 1;
    }

    public void StartGame()
    {
        NetworkClient.connection.identity.GetComponent<Player>().CmdStartGame();
    }

    public void LeaveLobby()
    {
        if(NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();

            SceneManager.LoadScene(0);
        }
    }
}
