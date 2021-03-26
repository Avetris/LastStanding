using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.SceneManagement;
using UnityEngine.Localization.Settings;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private GameOverHandler gameOverHandler = null;

    private int nextPlayerId = 0;

    public Dictionary<Enumerators.Variable, string> gameSetting = new Dictionary<Enumerators.Variable, string>();

    private bool isGameInProgress = false;

    private List<Player> Players { get; } = new List<Player>();

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;
    public static event Action<int> PlayerNumberUpdated;
    public static event Action<Enumerators.Variable> GameSettingsUpdated;

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

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

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

    public void ChangeSetting<T>(Enumerators.Variable variable, T newValue)
    {
        gameSetting[variable] = newValue.ToString();
        GameSettingsUpdated?.Invoke(variable);
    }

    public T GetSetting<T>(Enumerators.Variable variable, T defaultValue)
    {
        if (gameSetting.ContainsKey(variable))
        {
            return (T) Convert.ChangeType(gameSetting[variable], typeof(T));
        }
        else
        {
            return defaultValue;
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
       
        string message = LocalizeManager.singleton.GetText("max_players_limit");
        EventManager.singleton.CreateEvent<string>(Constants.MenuScene, EventType.Message, message);

        ClientOnDisconnected?.Invoke();
    }

    public override void OnStopClient()
    {
        Players.Clear();
    }

    #endregion
}
