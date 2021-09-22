using Mirror;
using UnityEngine;

public class SettinsPanelHandler : MonoBehaviour
{
    // private EOSLobby

    private void Start()
    {
    }

    public void Leave()
    {
        NetworkManager.singleton.StopHost();
    }
}