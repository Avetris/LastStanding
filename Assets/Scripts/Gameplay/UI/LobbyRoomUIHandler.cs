using UnityEngine;
using UnityEngine.UI;
using Mirror;
using UnityEngine.SceneManagement;

public class LobbyRoomUIHandler : NetworkBehaviour
{

    [SerializeField] private Button startGameButton = null;

    private void Start()
    {
        Player.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
        LobbyRoomManager.OnStartGameStatusChanges += HandleStartGameStatus;

        startGameButton.interactable = false;
    }

    private void OnDestroy()
    {
        Player.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
        LobbyRoomManager.OnStartGameStatusChanges -= HandleStartGameStatus;
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
}