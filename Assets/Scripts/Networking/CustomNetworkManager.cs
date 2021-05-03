using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.SceneManagement;
using System.Linq;
using Messaging;

public class CustomNetworkManager : NetworkManager
{
    [SerializeField] private GameOverHandler m_GameOverHandler = null;

    private int m_nextPlayerId = 0;

    private bool m_IsGameInProgress = false;

    // public List<RoomPlayer> Players { get; } = new List<RoomPlayer>();
    public int[] m_RespawnPlayersPoints = new int[0];

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

    // [Server]
    // public void ChangePlayerList(Player player, bool add)
    // {
        // if (add)
        // {
        //     Players.Add(player);
        // }
        // else
        // {
        //     Players.Remove(player);
        //     RemoveSpawnPosition(player.GetComponent<PlayerInfo>().GetPlayerId());
        // }
        // PlayerNumberUpdated?.Invoke(Players.Count);
    // }

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (m_IsGameInProgress)
        {            
            Notification notif = new Notification(){content = "Testing message"};

            conn.Send(notif, Channels.Reliable);

            Debug.Log("Message Sended");

            conn.Disconnect();
            return;
        }

        if (m_RespawnPlayersPoints == null)
        {
            int maxPlayers = LobbyRoomManager.singleton.GetSetting<int>(Enumerators.GameSetting.MaxPlayers, Constants.MaxPlayers);
            m_RespawnPlayersPoints = new int[maxPlayers];
            for (int i = 0; i < maxPlayers; i++) m_RespawnPlayersPoints[i] = -1;
        }
    }

    public override void OnStopServer()
    {
        LobbyRoomManager lobbyRoomManager = LobbyRoomManager.singleton;
        if (lobbyRoomManager != null)
        {
            Destroy(lobbyRoomManager.gameObject);
        }

        // Players.Clear();

        m_IsGameInProgress = false;
    }

    public void StartGame()
    {
        // if (!LobbyRoomManager.singleton.CanStartGame()) { return; }

        m_IsGameInProgress = true;

        ServerChangeScene("GameScene");
    }

    public void EndGame()
    {
        m_IsGameInProgress = false;
        ServerChangeScene(Constants.LobbyScene);
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        // Player player = conn.identity.GetComponent<Player>();

        // ChangePlayerList(conn.identity.GetComponent<Player>(), true);

        // PlayerInfo playerInfo = conn.identity.GetComponent<PlayerInfo>();

        // playerInfo.SetDisplayName($"Player {Players.Count}");
        // playerInfo.SetPlayerId(m_nextPlayerId);
        // playerInfo.SetDisplayColor(LobbyRoomManager.singleton.GetNextColor(m_nextPlayerId));

        // player.transform.position = GetNextRespawnPosition(m_nextPlayerId, true);

        // m_nextPlayerId++;

        // player.SetPartyOwner(Players.Count == 1);

        // PlayerNumberUpdated?.Invoke(Players.Count);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        // if (SceneManager.GetActiveScene().name.StartsWith(Constants.GameScene))
        // {
        // GameOverHandler gameOverHandleInstance = Instantiate(gameOverHandler);
        // NetworkServer.Spawn(gameOverHandleInstance.gameObject);

        // foreach (Player player in Players)
        // {
        //     player.transform.position = GetNextRespawnPosition(player.GetComponent<PlayerInfo>().GetPlayerId());
        // }
        // }
    }

    public Vector3 GetNextRespawnPosition(int idPlayer, bool isNew = false)
    {
        int respawnPos = 0;
        if (isNew)
        {
            for (; respawnPos < m_RespawnPlayersPoints.Count(); respawnPos++)
            {
                if (m_RespawnPlayersPoints[respawnPos] <= 0)
                {
                    m_RespawnPlayersPoints[respawnPos] = idPlayer;
                    break;
                }
            }
        }
        else
        {
            for (; respawnPos < m_RespawnPlayersPoints.Count(); respawnPos++)
            {
                if (m_RespawnPlayersPoints[respawnPos] == idPlayer)
                {
                    break;
                }
            }
        }
        return respawnPos >= 0 ? startPositions[respawnPos].position : Vector3.zero;
    }

    private void RemoveSpawnPosition(int idPlayer)
    {
        for (int respawnPos = 0; respawnPos < m_RespawnPlayersPoints.Count(); respawnPos++)
        {
            if (m_RespawnPlayersPoints[respawnPos] == idPlayer)
            {
                m_RespawnPlayersPoints[respawnPos] = -1;
                break;
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
        Debug.Log("Cliente desconectado");
        LobbyRoomManager lobbyRoomManager = LobbyRoomManager.singleton;
        if (lobbyRoomManager != null)
        {
            Debug.Log("Destroying lobby room manager");
            Destroy(lobbyRoomManager.gameObject);
        }
        else
        {            
            Debug.Log("lobby room manager not found");
        }
        base.OnClientDisconnect(conn);

        // string message = GetExitError();
        // Debug.Log("Error " + message);
        // if (message != null)
        // {
        //     EventManager.singleton.CreateEvent<string>(Constants.MenuScene, EventType.Message, message);
        // }

        ClientOnDisconnected?.Invoke();
    }

    public override void OnStopClient()
    {
        // Players.Clear();
    }

    #endregion


    public string GetExitError()
    {
        string error = null;


        Debug.Log("GetExitError " + m_IsGameInProgress);
        Debug.Log("GetExitError " + SceneManager.GetActiveScene().name);
        Debug.Log("GetExitError " + LobbyRoomManager.singleton);


        if (m_IsGameInProgress)
        {
            Debug.Log("Game not available");
            error = LocalizeManager.singleton.GetText("game_not_available");
        }
        // else if (LobbyRoomManager.singleton.GetSetting<int>(Enumerators.GameSetting.MaxPlayers, Constants.MaxPlayers) == Players.Count - 1)
        {
            Debug.Log("max_players_limit");
            error = LocalizeManager.singleton.GetText("max_players_limit");
        }
        return error;
    }
}
