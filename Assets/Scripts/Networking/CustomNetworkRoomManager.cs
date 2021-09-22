using System;
using Mirror;
using UnityEngine;

public class CustomNetworkRoomManager : NetworkRoomManager
{
    [SerializeField] private GameObject m_LobbyRoomManagerPrefab = null;
    private EOSLobby m_EOSLobby;

    public int[] m_RespawnPlayersPoints = new int[Constants.MaxPlayers];
    private (Color color, int playerId)[] m_PlayerColors = new (Color color, int playerId)[] {
        (Color.red, -1),
        ( Color.magenta, -1),
        ( Color.blue, -1),
        ( Color.green, -1),
        ( Color.white, -1),
        ( Color.black, -1),
        ( Color.gray, -1),
        ( Color.yellow, -1),
        ( Color.cyan, -1),
        ( Color.grey, -1),
    };

    public static event Action PlayerOnAddToRoom;
    public static event Action ClientOnDisconnected;
    public static event Action<int> PlayerNumberUpdated;
    public static event Action<bool> OnStartGameStatusChanges;

    public override void Start()
    {
        m_EOSLobby = FindObjectOfType<EOSLobby>();

        foreach (GameObject spawnablePrefab in Resources.LoadAll<GameObject>("Prefabs/Spawnable"))
        {
            if (spawnablePrefab.GetComponent<NetworkIdentity>() != null)
                spawnPrefabs.Add(spawnablePrefab);
        }

        m_RespawnPlayersPoints = new int[Constants.MaxPlayers];
        for (int i = 0; i < m_RespawnPlayersPoints.Length; i++) m_RespawnPlayersPoints[i] = -1;

        base.Start();
    }

    public override void OnRoomStartServer()
    {
        NetworkServer.Spawn(Instantiate(m_LobbyRoomManagerPrefab, Vector3.zero, Quaternion.identity));

        minPlayers = Constants.MinPlayers;
        maxConnections = LobbyRoomManager.instance.GetSetting<int>(Enumerators.GameSetting.Max_Players, Constants.MaxPlayers);
    }

    public override GameObject OnRoomServerCreateRoomPlayer(NetworkConnection conn)
    {
        Transform spawnPosition = startPositions[GetNextRespawnPosition(conn.connectionId, true)];

        GameObject newRoomGameObject = Instantiate(roomPlayerPrefab.gameObject, spawnPosition.position, spawnPosition.rotation);

        newRoomGameObject.GetComponent<PlayerInfo>().SetPlayerId(conn.connectionId);

        Debug.Log($"Id Room player {conn.connectionId}");

        PlayerInfo playerInfo = newRoomGameObject.GetComponent<PlayerInfo>();
        playerInfo.SetDisplayName($"Player {conn.connectionId}");
        playerInfo.SetDisplayColor(GetNextColor(conn.connectionId, true));

        return newRoomGameObject;
    }

    public override GameObject OnRoomServerCreateGamePlayer(NetworkConnection conn, GameObject roomPlayer)
    {
        int idPlayer = conn.connectionId;
        Transform spawnPosition = startPositions[GetNextRespawnPosition(idPlayer, false)];

        GameObject newPlayerGameObject = Instantiate(playerPrefab, spawnPosition.position, spawnPosition.rotation);

        return newPlayerGameObject;
    }

    public override bool OnRoomServerSceneLoadedForPlayer(NetworkConnection conn, GameObject roomPlayer, GameObject gamePlayer)
    {
        gamePlayer.GetComponent<PlayerInfo>().SetData(roomPlayer.GetComponent<PlayerInfo>());
        return true;
    }

    public override void OnRoomClientEnter()
    {
        foreach (RoomPlayer roomPlayer in FindObjectsOfType<RoomPlayer>())
        {
            roomPlayer.GetComponent<RoomPlayer>().RpcChangeState(true);
        }
    }

    public override void OnRoomClientExit()
    {
        foreach (RoomPlayer roomPlayer in FindObjectsOfType<RoomPlayer>())
        {
            roomPlayer.GetComponent<RoomPlayer>().RpcChangeState(false);
        }
    }

    public override void OnRoomServerSceneChanged(string sceneName)
    {
        // spawn the initial batch of Rewards
        Debug.Log("OnRoomServerSceneChanged");

        if (sceneName == GameplayScene)
        {
            // Spawner.InitialSpawn();
        }
    }

    public Color GetNextColor(int idPlayer, bool isNew = false)
    {
        int respawnPos = 0;
        if (isNew)
        {
            for (; respawnPos < m_PlayerColors.Length; respawnPos++)
            {
                if (m_PlayerColors[respawnPos].playerId < 0)
                {
                    m_PlayerColors[respawnPos].playerId = idPlayer;
                    break;
                }
            }
        }
        else
        {
            for (; respawnPos < m_PlayerColors.Length; respawnPos++)
            {
                if (m_PlayerColors[respawnPos].playerId == idPlayer)
                {
                    break;
                }
            }
        }
        return m_PlayerColors[respawnPos].color;
    }

    public int GetNextRespawnPosition(int idPlayer, bool isNew = false)
    {
        int respawnPos = 0;
        if (isNew)
        {
            for (; respawnPos < m_RespawnPlayersPoints.Length; respawnPos++)
            {
                if (m_RespawnPlayersPoints[respawnPos] < 0)
                {
                    m_RespawnPlayersPoints[respawnPos] = idPlayer;
                    break;
                }
            }
        }
        else
        {
            for (; respawnPos < m_RespawnPlayersPoints.Length; respawnPos++)
            {
                if (m_RespawnPlayersPoints[respawnPos] == idPlayer)
                {
                    break;
                }
            }
        }
        return respawnPos;
    }

    private void RemoveSpawnPosition(int idPlayer)
    {
        for (int respawnPos = 0; respawnPos < m_RespawnPlayersPoints.Length; respawnPos++)
        {
            if (m_RespawnPlayersPoints[respawnPos] == idPlayer)
            {
                m_RespawnPlayersPoints[respawnPos] = -1;
                break;
            }
        }
    }

    public override void OnRoomStopClient()
    {
        base.OnRoomStopClient();

        m_EOSLobby.LeaveLobby();

        CloseSession();
    }

    public override void OnRoomStopHost()
    {
        base.OnRoomStopHost();

        CloseSession();
    }

    public override void OnRoomStopServer()
    {
        base.OnRoomStopServer();

        CloseSession();
    }

    private void CloseSession()
    {
        m_EOSLobby.LeaveLobby();
    }

    public override void OnRoomServerPlayersReady()
    {
        OnStartGameStatusChanges?.Invoke(true);
        base.OnRoomServerPlayersReady();

    }

    public override void OnRoomServerPlayersNotReady()
    {
        OnStartGameStatusChanges?.Invoke(false);
        base.OnRoomServerPlayersNotReady();
    }

    public void StartGame()
    {
        ReadyStatusChanged();
        if (!allPlayersReady) { return; }

        ServerChangeScene(GameplayScene);
    }

    public void EndGame()
    {
        // m_IsGameInProgress = false;
        // ServerChangeScene(Constants.LobbyScene);
    }
}