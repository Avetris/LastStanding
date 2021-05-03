using UnityEngine;
using Mirror;
using System.Linq;
using System.Collections.Generic;
using System;

public class LobbyRoomManager : NetworkBehaviour
{
    #region Singleton
    private static LobbyRoomManager s_Instance;
    public static LobbyRoomManager singleton
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
    #endregion

    #region GameSettings
    private SyncDictionary<Enumerators.GameSetting, string> m_GameSetting = new SyncDictionary<Enumerators.GameSetting, string>();

    public void UpdateSetting<T>(Enumerators.GameSetting variable, T newValue)
    {
        m_GameSetting[variable] = newValue.ToString();
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


    public void SyncDictionaryChanged(SyncDictionary<Enumerators.GameSetting, string>.Operation op, Enumerators.GameSetting setting, string value)
    {

        CustomNetworkRoomManager networkRoomManager = ((CustomNetworkRoomManager) NetworkManager.singleton);
        switch(setting)
        {
            case Enumerators.GameSetting.MinPlayers:
                networkRoomManager.minPlayers = (int) Convert.ChangeType(value, typeof(int));
                networkRoomManager.ReadyStatusChanged();
                break;
            case Enumerators.GameSetting.MaxPlayers:
                networkRoomManager.maxConnections = (int) Convert.ChangeType(value, typeof(int));
                networkRoomManager.ReadyStatusChanged();
                break;            
        }

    }
    #endregion

    #region Color
    private SyncDictionary<Color, int> m_PlayerColors = new SyncDictionary<Color, int>{
        {Color.red, -1},
        {Color.magenta, -1},
        {Color.blue, -1},
        {Color.green, -1},
        {Color.white, -1},
        {Color.black, -1},
        {Color.gray, -1},
        {Color.yellow, -1},
        {new Color(0.93f, 0.33f, 0.73f), -1},
        {new Color(0.94f, 0.49f, 0.05f), -1},
        {new Color(0.42f, 0.19f, 0.74f), -1},
        {new Color(0.44f, 0.29f, 0.12f), -1},
        {new Color(0.22f, 1, 0.86f), -1},
        {new Color(0.31f, 0.94f, 0.22f), -1},
        {new Color(0.83f, 0.69f, 0.22f), -1},
    };

    public void UpdateColorChangeListeners(bool add, SyncDictionary<Color, int>.SyncDictionaryChanged callback)
    {
        if (add)
        {
            m_PlayerColors.Callback += callback;
        }
        else
        {
            m_PlayerColors.Callback -= callback;
        }
    }

    public List<(Color color, int playerId)> GetColors()
    {
        return m_PlayerColors.Select(x => (x.Key, x.Value)).ToList();
    }

    [Server]
    public Color GetNextColor(int playerId)
    {
        Color col = Color.clear;
        foreach (KeyValuePair<Color, int> entry in m_PlayerColors)
        {
            if (entry.Value == -1)
            {
                col = entry.Key;
                break;
            }
        }
        m_PlayerColors[col] = playerId;

        return col;
    }

    [Server]
    public bool CanSetColor(int playerId, Color oldColor, Color newColor)
    {
        bool can = false;

        if (newColor == Color.clear)
        {
            can = true;
        }
        else if (m_PlayerColors[newColor] == -1)
        {
            can = true;
            m_PlayerColors[newColor] = playerId;
        }

        if (can && oldColor != Color.clear)
        {
            m_PlayerColors[oldColor] = -1;
        }

        return can;
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

        if (newPlayerNumber < GetSetting<int>(Enumerators.GameSetting.MinPlayers, Constants.MinPlayers))
        {
            m_IsPaused = true;
            OnGameFinished?.Invoke();
        }
    }
}