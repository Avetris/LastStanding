using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class LobbyUIHandler : MonoBehaviour
{

    [SerializeField] private Button startGameButton = null;

    private void Start()
    {
        CustomNetworkManager.PlayerNumberUpdated += HandleClientConnect;
        Player.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;

        startGameButton.interactable = false;
    }

    private void OnDestroy()
    {
        CustomNetworkManager.PlayerNumberUpdated -= HandleClientConnect;
        Player.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
    }

    private void AuthorityHandlePartyOwnerStateUpdated(bool state)
    {
        startGameButton.gameObject.SetActive(state);
    }


    private void HandleClientConnect(int playerCount)
    {
        startGameButton.interactable = playerCount > 1;
    }


    public void StartGame()
    {
        // NetworkClient.connection.identity.GetComponent<RTSPlayer>().CmdStartGame();
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

            // SceneManager.LoadScene(0);
        }
    }

}