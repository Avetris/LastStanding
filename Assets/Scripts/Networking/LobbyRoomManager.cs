using UnityEngine;
using Mirror;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine.SceneManagement;

public class LobbyRoomManager : NetworkBehaviour
{
    #region SINGLETON
    private static LobbyRoomManager _instance;
    public static LobbyRoomManager singleton
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LobbyRoomManager>();
                DontDestroyOnLoad(_instance);
            }
            return _instance;
        }
    }
    #endregion

    private int currentPlayers;
    
    private SyncDictionary<Color, int> playerColors = new SyncDictionary<Color, int>{
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

    private SyncDictionary<Enumerators.GameSetting, string> gameSetting = new SyncDictionary<Enumerators.GameSetting, string>();


    #region Events
    public static event Action<bool> OnStartGameStatusChanges;
    public static event Action<Enumerators.GameSetting> GameSettingsUpdated;
    #endregion
    
   

    private void Start()
    {
        if(FindObjectsOfType<LobbyRoomManager>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        CustomNetworkManager.PlayerNumberUpdated += HandleClientConnect;

        SceneManager.activeSceneChanged += OnSceneChanged;
        Scene currentScene = SceneManager.GetActiveScene();
    }

    private void OnDestroy()
    {
        CustomNetworkManager.PlayerNumberUpdated -= HandleClientConnect;
        SceneManager.activeSceneChanged -= OnSceneChanged;
    }    

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        if (this == null || gameObject == null) { return; }

        if(newScene.name == Constants.LobbyScene)
        {   
            this.Invoke(() => OnStartGameStatusChanges?.Invoke(CanStartGame()), .1f);
        }
    }

    [Server]
    private void HandleClientConnect(int playerCount)
    {
        currentPlayers = playerCount;
        OnStartGameStatusChanges?.Invoke(CanStartGame());
    }

    public void ChangeSetting<T>(Enumerators.GameSetting variable, T newValue)
    {
        gameSetting[variable] = newValue.ToString();
        GameSettingsUpdated?.Invoke(variable);
    }

    public T GetSetting<T>(Enumerators.GameSetting variable, T defaultValue)
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

    public bool CanStartGame()
    {
        return currentPlayers >= GetSetting(Enumerators.GameSetting.MinPlayers, Constants.MinPlayers);
    }

    public void UpdateColorChangeListeners(bool add, SyncDictionary<Color, int>.SyncDictionaryChanged callback)
    {
        if (add)
        {
            playerColors.Callback += callback;
        }
        else
        {
            playerColors.Callback -= callback;
        }
    }

    public List<(Color color, int playerId)> GetColors()
    {
        return playerColors.Select(x => (x.Key, x.Value)).ToList();
    }    

    [Server]
    public Color GetNextColor(int playerId)
    {
        Color col = Color.clear;
        foreach (KeyValuePair<Color, int> entry in playerColors)
        {
            if (entry.Value == -1)
            {
                col = entry.Key;
                break;
            }
        }
        playerColors[col] = playerId;

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
        else if (playerColors[newColor] == -1)
        {
            can = true;
            playerColors[newColor] = playerId;
        }

        if (can && oldColor != Color.clear)
        {
            playerColors[oldColor] = -1;
        }

        return can;
    }
}