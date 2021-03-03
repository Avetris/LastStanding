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

        startGameButton.interactable = false;
    }

    private void OnDestroy()
    {
        Player.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
    }

    private void AuthorityHandlePartyOwnerStateUpdated(bool state)
    {
        startGameButton.gameObject.SetActive(state);
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