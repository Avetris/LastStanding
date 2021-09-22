using UnityEngine;
using UnityEngine.UI;
using Mirror;
using TMPro;

public class LobbyRoomUIHandler : NetworkBehaviour
{
    [SerializeField] private TMP_Text m_LobbyCodeText = null;
    [SerializeField] private Button m_StartGameButton = null;
    EOSLobby m_EOSLobby;

    private void Start()
    {
        m_EOSLobby = FindObjectOfType<EOSLobby>();
        // Player.AuthorityOnPartyOwnerStateUpdated += AuthorityHandlePartyOwnerStateUpdated;
        CustomNetworkRoomManager.OnStartGameStatusChanges += HandleStartGameStatus;
        LobbyRoomManager.OnShowCodeChanged += HandleShowCodeStatus;

        RpcChangeShowCodeStatus(LobbyRoomManager.instance.GetSetting<bool>(Enumerators.GameSetting.Hide_Lobby_Code, true));

        m_StartGameButton.interactable = false;
    }

    private void OnDestroy()
    {
        // Player.AuthorityOnPartyOwnerStateUpdated -= AuthorityHandlePartyOwnerStateUpdated;
        CustomNetworkRoomManager.OnStartGameStatusChanges -= HandleStartGameStatus;
        LobbyRoomManager.OnShowCodeChanged -= HandleShowCodeStatus;
    }

    private void AuthorityHandlePartyOwnerStateUpdated(bool state)
    {
        m_StartGameButton.gameObject.SetActive(state);
    }

    public void HandleStartGameStatus(bool status)
    {
        RpcChangeStartGameStatus(status);
    }

    public void HandleShowCodeStatus(bool hide)
    {
        RpcChangeShowCodeStatus(hide);
    }

    [ClientRpc]
    public void RpcChangeStartGameStatus(bool status)
    {
        m_StartGameButton.interactable = status;
    }

    [ClientRpc]
    public void RpcChangeShowCodeStatus(bool hide)
    {
        if (hide)
        {
             LocalizeManager.Instance.SetLocalText("lobby_code_placeholder", m_LobbyCodeText);
        }
        else
        {
            m_LobbyCodeText.text = m_EOSLobby.GetLobbyData(Enumerators.GameSetting.Lobby_Id).AsUtf8;
        }
    }

    public void CopyLobbyCode()
    {
        GUIUtility.systemCopyBuffer = m_EOSLobby.GetLobbyData(Enumerators.GameSetting.Lobby_Id).AsUtf8;
    }

    public void StartGame()
    {
        // NetworkClient.connection.identity.GetComponent<Player>().CmdStartGame();
    }
}