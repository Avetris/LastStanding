using Epic.OnlineServices;
using Mirror;
using System;

public class LobbyRoomManager : NetworkBehaviour
{
    private static string TAG = "LobbyRoomManager";

    EOSLobby m_EOSLobby = null;
    private SyncDictionary<Enumerators.GameSetting, object> m_GameSetting = new SyncDictionary<Enumerators.GameSetting, object>();

    #region Singleton
    private static LobbyRoomManager s_Instance;
    public static LobbyRoomManager instance
    {
        get
        {
            if (s_Instance == null)
            {
                s_Instance = FindObjectOfType<LobbyRoomManager>();
                DontDestroyOnLoad(s_Instance);
            }
            return s_Instance;
        }
    }
    #endregion

    #region Events
    public static event Action<Enumerators.GameSetting> GameSettingsUpdated;
    public static event Action OnGameFinished;
    public static event Action<int> OnPlayersAliveChanged;
    public static event Action<bool> OnShowCodeChanged;
    #endregion

    private void Start()
    {
        m_EOSLobby = FindObjectOfType<EOSLobby>();
        m_GameSetting.Callback += SyncDictionaryChanged;

        GetLobbyAttributes();
    }

    private void OnDestroy()
    {
        m_GameSetting.Callback -= SyncDictionaryChanged;
    }

    private void GetLobbyAttributes()
    {
        foreach (Epic.OnlineServices.Lobby.Attribute attribute in m_EOSLobby.GetCurrentData())
        {
            Enumerators.GameSetting key;
            if (Enum.TryParse<Enumerators.GameSetting>(attribute.Data.Key, true, out key))
            {
                AttributeType type = attribute.Data.Value.ValueType;
                switch (type)
                {
                    case AttributeType.Boolean: UpdateSetting(key, attribute.Data.Value.AsBool); break;
                    case AttributeType.String: UpdateSetting(key, attribute.Data.Value.AsUtf8); break;
                    case AttributeType.Int64: UpdateSetting(key, attribute.Data.Value.AsInt64); break;
                    case AttributeType.Double: UpdateSetting(key, attribute.Data.Value.AsDouble); break;
                }
            }
            else
            {
                LogManager.Error(TAG, "Start", attribute.Data.Key);
            }
        }
    }

    #region GameSettings

    public void UpdateSetting<T>(Enumerators.GameSetting variable, T newValue)
    {
        m_GameSetting[variable] = newValue;
        UnityEngine.Debug.Log($"Changing data {variable} --> {newValue}");
        GameSettingsUpdated?.Invoke(variable);
    }

    public T GetSetting<T>(Enumerators.GameSetting variable, T defaultValue)
    {
        if (m_GameSetting.ContainsKey(variable))
        {
            return (T)Convert.ChangeType(m_GameSetting[variable], typeof(T));
        }
        else
        {
            return defaultValue;
        }
    }


    public void SyncDictionaryChanged(SyncDictionary<Enumerators.GameSetting, object>.Operation op, Enumerators.GameSetting key, object value)
    {
        UnityEngine.Debug.Log($"SyncDictionaryChanged {op} --> {key} --> {value}");
        CustomNetworkRoomManager networkRoomManager = ((CustomNetworkRoomManager)NetworkManager.singleton);
        switch (key)
        {
            case Enumerators.GameSetting.Max_Players:
                networkRoomManager.maxConnections = (int)Convert.ChangeType(value, typeof(int));
                networkRoomManager.ReadyStatusChanged();
                break;
            case Enumerators.GameSetting.Hide_Lobby_Code:
                OnShowCodeChanged?.Invoke((bool)value);
                break;
        }
        if (op == SyncIDictionary<Enumerators.GameSetting, object>.Operation.OP_SET)
        {
            UpdateServerAttribute(value.GetType(), key.ToString(), value);
        }
    }

    private void UpdateServerAttribute(Type type, string key, object value)
    {
        if (type == typeof(int))
        {
            m_EOSLobby.UpdateLobbyAttribute(key, (int)value);
        }
        else if (type == typeof(bool))
        {
            m_EOSLobby.UpdateLobbyAttribute(key, (bool)value);
        }
        else if (type == typeof(string))
        {
            m_EOSLobby.UpdateLobbyAttribute(key, (string)value);
        }
        else if (type == typeof(double))
        {
            m_EOSLobby.UpdateLobbyAttribute(key, (double)value);
        }
    }
    #endregion

    private int m_CurrentPlayers;
    [SyncVar(hook = nameof(OnNumberPlayerAliveChanged))]
    private int m_CurrentPlayersAlive;
    [SyncVar]
    private bool m_IsPaused = false;

    public bool IsPaused()
    {
        return m_IsPaused;
    }

    [Server]
    private void CheckAlivePlayers()
    {
        int currentPlayersAlive = 0;
        // foreach (Player player in ((CustomNetworkRoomManager)NetworkManager.singleton).Players)
        // {
        //     if (player.GetComponent<PlayerInfo>().IsAlive())
        //     {
        //         currentPlayersAlive++;
        //     }
        // }
        if (m_CurrentPlayersAlive != currentPlayersAlive)
        {
            m_CurrentPlayersAlive = currentPlayersAlive;
        }
    }

    private void OnNumberPlayerAliveChanged(int oldPlayerNumber, int newPlayerNumber)
    {
        OnPlayersAliveChanged?.Invoke(newPlayerNumber);

        if (newPlayerNumber < GetSetting<int>(Enumerators.GameSetting.Min_Players, Constants.MinPlayers))
        {
            m_IsPaused = true;
            OnGameFinished?.Invoke();
        }
    }
}