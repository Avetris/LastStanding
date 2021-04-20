using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Settings;
using System.Linq;
using System.Runtime.CompilerServices;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private GameOverHandler gameOverHandler = null;

    private int nextPlayerId = 0;

    private bool isGameInProgress = false;

    public List<Player> Players { get; } = new List<Player>();

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;
    public static event Action<int> PlayerNumberUpdated;

    public override void Start()
    {
        foreach (GameObject spawnablePrefab in Resources.LoadAll<GameObject>("Prefabs/Gameplay"))
        {
            if (spawnablePrefab.GetComponent<NetworkIdentity>() != null)
                spawnPrefabs.Add(spawnablePrefab);
        }
        base.Start();
    }

    #region Server
    [Server]
    public void ChangePlayerList(bool add, Player player)
    {
        if (add)
        {
            Players.Add(player);
        }
        else
        {
            Players.Remove(player);
        }
        PlayerNumberUpdated?.Invoke(Players.Count);
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (!isGameInProgress) { return; }

        conn.Disconnect();
    }

    public override void OnStopServer()
    {
        LobbyRoomManager lobbyRoomManager = LobbyRoomManager.singleton;
        if (lobbyRoomManager != null)
        {
            Destroy(lobbyRoomManager.gameObject);
        }

        Players.Clear();

        isGameInProgress = false;
    }

    public void StartGame()
    {
        if (!LobbyRoomManager.singleton.CanStartGame()) { return; }

        isGameInProgress = true;

        ServerChangeScene("GameScene");
    }

    public void EndGame()
    { 
        isGameInProgress = false;
        ServerChangeScene(Constants.LobbyScene);
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        Debug.Log("OnServerAddPlayer");

        Player player = conn.identity.GetComponent<Player>();

        Players.Add(conn.identity.GetComponent<Player>());

        PlayerInfo playerInfo = conn.identity.GetComponent<PlayerInfo>();

        playerInfo.SetDisplayName($"Player {Players.Count}");
        playerInfo.SetPlayerId(nextPlayerId);
        playerInfo.SetDisplayColor(LobbyRoomManager.singleton.GetNextColor(nextPlayerId));

        nextPlayerId++;

        player.SetPartyOwner(Players.Count == 1);

        PlayerNumberUpdated?.Invoke(Players.Count);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith(Constants.GameScene))
        {
            // GameOverHandler gameOverHandleInstance = Instantiate(gameOverHandler);
            // NetworkServer.Spawn(gameOverHandleInstance.gameObject);

            for (int i = 0; i < Players.Count; i++)
            {
                // GameObject newPlayer = Instantiate(playerPrefab);
                // newPlayer.GetComponent<PlayerInfo>().SetData(Players[i].GetComponent<PlayerInfo>());

                // // Instantiate the new player object and broadcast to clients
                // NetworkServer.ReplacePlayerForConnection(Players[i].connectionToClient, newPlayer);

                // // Remove the previous player object that's now been replaced
                // NetworkServer.Destroy(Players[i].gameObject);

                // Players[i] = newPlayer.GetComponent<Player>();
                
                // player.transform.position = GetStartPosition().position;
                // player.ResetEveryPosition(GetStartPosition().position);
            }
        }
    }

    #endregion

    #region Client

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        string message = GetExitError();
        if (message != null)
        {
            EventManager.singleton.CreateEvent<string>(Constants.MenuScene, EventType.Message, message);
        }

        ClientOnDisconnected?.Invoke();
    }

    public override void OnStopClient()
    {
        Players.Clear();
    }

    #endregion


    public string GetExitError()
    {
        string error = null;

        if (!isGameInProgress) { error = LocalizeManager.singleton.GetText("max_players_limit"); }
        else if (LobbyRoomManager.singleton.GetSetting<int>(Enumerators.GameSetting.MaxPlayers, Constants.MaxPlayers) == Players.Count - 1)
        {
            error = LocalizeManager.singleton.GetText("game_not_available");
        }
        return error;
    }
}
