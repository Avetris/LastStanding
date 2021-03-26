using System.Collections;
using System.Collections.Generic;
using Mirror;
using TMPro;
// using Steamworks;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject landingPagePanel = null;
    [SerializeField] private GameObject errorDialog = null;

    [SerializeField] private bool useSteam = false;

    // protected Callback<LobbyCreated_t> lobbyCreated;
    // protected Callback<GameLobbyJoinRequested_t> gameLobbyGameRequested;
    // protected Callback<LobbyEnter_t> lobbyEntered;

    private void Start()
    {
        EventManager.OnEventUpdated += OnEventUpdatedHandler;

        GetEventMessages();

        if (!useSteam) { return; }

        // lobbyCreated = Callback<LobbyCreated_t>.Create(OnLobbyCreated);
        // gameLobbyGameRequested = Callback<GameLobbyJoinRequested_t>.Create(OnGameLobbyJoinRequested);
        // lobbyEntered = Callback<LobbyEnter_t>.Create(OnLobbyEnter);
    }

    private void OnDestroy()
    {
        EventManager.OnEventUpdated -= OnEventUpdatedHandler;
    }

    public void OnEventUpdatedHandler(string sceneName)
    {
       
         if(sceneName == Constants.MenuScene)
        {
            GetEventMessages();
        }
    }

    private void GetEventMessages()
    {
        List<CustomEvent> customEvents = EventManager.singleton.GetEventOfType(Constants.MenuScene, EventType.All, true);
        foreach(CustomEvent customEvent in customEvents)
        {
            if(customEvent.GetEventType() == EventType.Message)
            {
                errorDialog.GetComponentInChildren<TMP_Text>().text = customEvent.GetValue<string>();
                errorDialog.SetActive(true);
            }
        }
    }

    private void PrintErrorDialog(string text)
    {

    }


    public void HostLobby()
    {
        landingPagePanel.SetActive(false);

        if (useSteam)
        {
            // SteamMatchmaking.CreateLobby(ELobbyType.k_ELobbyTypeFriendsOnly, 4);
            return;
        }

        NetworkManager.singleton.StartHost();
    }

    // private void OnLobbyCreated(LobbyCreated_t callback)
    // {
    //     if(callback.m_eResult != EResult.k_EResultOK)
    //     {
    //         landingPagePanel.SetActive(true);
    //         return;
    //     }

    //     NetworkManager.singleton.StartHost();

    //     SteamMatchmaking.SetLobbyData(
    //         new CSteamID(callback.m_ulSteamIDLobby), 
    //         "HostAddress",
    //         SteamUser.GetSteamID().ToString());
    // }

    // private void OnGameLobbyJoinRequested(GameLobbyJoinRequested_t callback)
    // {
    //     SteamMatchmaking.JoinLobby(callback.m_steamIDLobby);
    // }

    // private void OnLobbyEnter(LobbyEnter_t callback)
    // {
    //     if(NetworkServer.active) { return; }

    //     string hostAddress = SteamMatchmaking.GetLobbyData(
    //         new CSteamID(callback.m_ulSteamIDLobby), 
    //         "HostAddress");

    //     NetworkManager.singleton.networkAddress = hostAddress;
    //     NetworkManager.singleton.StartClient();

    //     landingPagePanel.SetActive(false);
    // }
}
