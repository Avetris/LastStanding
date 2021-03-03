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

    private int maxPlayers = 10;
    private int currentPlayers;
    
    private SyncDictionary<Color, bool> availableColors = new SyncDictionary<Color, bool>{
        {Color.red, true},
        {Color.magenta, true},
        {Color.blue, true},
        {Color.green, true},
        {Color.white, true},
        {Color.black, true},
        {Color.gray, true},
        {Color.yellow, true},
        {new Color(0.93f, 0.33f, 0.73f), true},
        {new Color(0.94f, 0.49f, 0.05f), true},
        {new Color(0.42f, 0.19f, 0.74f), true},
        {new Color(0.44f, 0.29f, 0.12f), true},
        {new Color(0.22f, 1, 0.86f), true},
        {new Color(0.31f, 0.94f, 0.22f), true},
        {new Color(0.83f, 0.69f, 0.22f), true},
    };

    public void UpdateColorChangeListeners(bool add, SyncDictionary<Color, bool>.SyncDictionaryChanged callback)
    {
        if (add)
        {
            availableColors.Callback += callback;
        }
        else
        {
            availableColors.Callback -= callback;
        }
    }

    public List<(Color color, bool available)> GetColors()
    {
        return availableColors.Select(x => (x.Key, x.Value)).ToList();
    }    

    private void Start()
    {
        CustomNetworkManager.PlayerNumberUpdated += HandleClientConnect;
    }

    private void OnDestroy()
    {
        CustomNetworkManager.PlayerNumberUpdated -= HandleClientConnect;
    }

    [Server]
    public Color GetNextColor()
    {
        Color col = Color.clear;
        foreach (KeyValuePair<Color, bool> entry in availableColors)
        {
            if (entry.Value)
            {
                col = entry.Key;
            }
        }
        availableColors[col] = false;

        return col;
    }

    [Server]
    public bool CanSetColor(Color oldColor, Color newColor)
    {
        bool can = false;

        if (newColor == Color.clear)
        {
            can = true;
        }
        else if (availableColors[newColor])
        {
            can = true;
            availableColors[newColor] = false;
        }

        if (can && oldColor != Color.clear)
        {
            availableColors[oldColor] = true;
        }

        return can;
    }

    [Server]
    private void HandleClientConnect(int playerCount)
    {
        bool enabled = playerCount >= 2;
    }
}