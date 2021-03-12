using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private GameOverHandler gameOverHandler = null;

    private int nextPlayerId = 0;

    private bool isGameInProgress = false;

    private List<Player> Players { get; } = new List<Player>();

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
        LobbyManager lobbyManager = LobbyManager.singleton;
        if (lobbyManager != null)
        {
            Destroy(lobbyManager.gameObject);
        }

        Players.Clear();

        isGameInProgress = false;
    }

    public void StartGame()
    {
        if (!LobbyManager.singleton.CanStartGame()) { return; }

        isGameInProgress = true;

        ServerChangeScene("GameScene");
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        Player player = conn.identity.GetComponent<Player>();

        Players.Add(conn.identity.GetComponent<Player>());

        PlayerInfo playerInfo = conn.identity.GetComponent<PlayerInfo>();

        playerInfo.SetDisplayName($"Player {Players.Count}");
        playerInfo.SetPlayerId(nextPlayerId);
        playerInfo.SetDisplayColor(LobbyManager.singleton.GetNextColor(nextPlayerId));

        nextPlayerId++;

        player.SetPartyOwner(Players.Count == 1);

        PlayerNumberUpdated?.Invoke(Players.Count);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("GameScene"))
        {
            // GameOverHandler gameOverHandleInstance = Instantiate(gameOverHandler);
            // NetworkServer.Spawn(gameOverHandleInstance.gameObject);

            foreach (Player player in Players)
            {
                player.transform.position = GetStartPosition().position;
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

        Debug.Log("Exit");

        ClientOnDisconnected?.Invoke();
    }

    public override void OnStopClient()
    {
        Players.Clear();
    }

    #endregion
}
