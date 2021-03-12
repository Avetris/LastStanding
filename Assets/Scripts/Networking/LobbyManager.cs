using UnityEngine;
using Mirror;
using System.Linq;
using System.Collections.Generic;
using System;
using System.Collections;

public class LobbyManager : NetworkBehaviour
{
    #region SINGLETON
    private static LobbyManager _instance;
    public static LobbyManager singleton
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LobbyManager>();
            }
            return _instance;
        }
    }
    #endregion

    private int minimunPlayers = 1;
    private int maxPlayers = 10;
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

    public static event Action<bool> OnStartGameStatusChanges;
    

    private void Start()
    {
        CustomNetworkManager.PlayerNumberUpdated += HandleClientConnect;
    }

    private void OnDestroy()
    {
        CustomNetworkManager.PlayerNumberUpdated -= HandleClientConnect;
    }    

    [Server]
    private void HandleClientConnect(int playerCount)
    {
        currentPlayers = playerCount;
        OnStartGameStatusChanges?.Invoke(CanStartGame());
    }

    public bool CanStartGame()
    {
        return currentPlayers >= minimunPlayers;
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