using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.SceneManagement;

public class LobbyUIHandler : NetworkBehaviour
{

    [SerializeField] private Button startGameButton = null;

    private void Start()
    {
        Player.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
        LobbyManager.OnStartGameStatusChanges += HandleStartGameStatus;

        startGameButton.interactable = false;
    }

    private void OnDestroy()
    {
        Player.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
        LobbyManager.OnStartGameStatusChanges -= HandleStartGameStatus;
    }

    private void AuthorityHandlePartyOwnerStateUpdated(bool state)
    {
        startGameButton.gameObject.SetActive(state);
    }

    public void HandleStartGameStatus(bool status)
    {
        RpcChangeStartGameStatus(status);
    }

    [ClientRpc]
    public void RpcChangeStartGameStatus(bool status)
    {
        startGameButton.interactable = status;
    }


    public void StartGame()
    {
        NetworkClient.connection.identity.GetComponent<Player>().CmdStartGame();
    }

    public void LeaveLobby()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();

            SceneManager.LoadScene(0);
        }
    }

}