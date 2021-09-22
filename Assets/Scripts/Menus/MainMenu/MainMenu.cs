using System.Collections.Generic;
using Epic.OnlineServices.Lobby;
using EpicTransport;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

enum PanelType { None, MainMenu, CreateLobby, LobbyList };

public class MainMenu : MonoBehaviour
{
    private const string TAG = "MainMenu";
    [Header("Networking")]
    [SerializeField] private GameObject m_NetworkManagerPrefab = null;

    [Header("Panels")]
    [SerializeField] private GameObject m_MainMenuPanel = null;
    [SerializeField] private GameObject m_CreateLobbyPanel = null;
    [SerializeField] private GameObject m_LobbyListPanel = null;


    [Header("Buttons & Data Pickers")]
    [SerializeField] private Button m_JoinLobbyButton = null;
    [SerializeField] private TMP_InputField m_CodeTextField = null;

    private MenuDialogsHandler m_MenuDialogsHandler;
    private EOSLobby m_EOSLobby;
    private List<LobbyDetails> m_FoundLobbies = new List<LobbyDetails>();
    private List<Epic.OnlineServices.Lobby.Attribute> lobbyData = new List<Epic.OnlineServices.Lobby.Attribute>();

    private void Start()
    {
        m_EOSLobby = FindObjectOfType<EOSLobby>();
        if (m_EOSLobby == null)
        {
            GameObject go = Instantiate(m_NetworkManagerPrefab, Vector3.zero, Quaternion.identity);
#if UNITY_EDITOR
            go.GetComponent<EOSSDKComponent>().devAuthToolCredentialName = "Quatvm";
#else
                    go.GetComponent<EOSSDKComponent>().devAuthToolCredentialName = "Quatvm1";

#endif
            m_EOSLobby = go.GetComponent<EOSLobby>();

        }
        m_MenuDialogsHandler = FindObjectOfType<MenuDialogsHandler>();

        ChangePanel(PanelType.MainMenu);

        EventManager.OnEventUpdated += OnEventUpdatedHandler;

        m_EOSLobby.FindCodeLobbySucceeded += OnFindingCodeLobbySuccess;

        m_EOSLobby.FindCodeLobbyFailed += OnFindingCodeLobbyFailure;
        m_EOSLobby.FindLobbiesFailed += OnFindLobbiesFailure;
        m_EOSLobby.JoinLobbyFailed += OnJoinLobbyFailure;
        m_EOSLobby.JoinLobbySucceeded += OnJoinLobbySuccedded;

        m_CodeTextField.onValueChanged.AddListener(OnCodeTextFieldDeselect);
        m_CodeTextField.characterLimit = Constants.LobbyCodeLength;

        GetEventMessages();
    }

    private void OnDestroy()
    {
        EventManager.OnEventUpdated -= OnEventUpdatedHandler;
        m_EOSLobby.FindCodeLobbySucceeded -= OnFindingCodeLobbySuccess;
        m_EOSLobby.FindCodeLobbyFailed -= OnFindingCodeLobbyFailure;
        m_EOSLobby.FindLobbiesFailed -= OnFindLobbiesFailure;
        m_EOSLobby.JoinLobbyFailed -= OnJoinLobbyFailure;
        m_EOSLobby.JoinLobbySucceeded -= OnJoinLobbySuccedded;
    }

    public void OnEventUpdatedHandler(string sceneName)
    {
        if (sceneName == Constants.MenuScene)
        {
            GetEventMessages();
        }
    }

    private void GetEventMessages()
    {
        List<CustomEvent> customEvents = EventManager.singleton.GetEventOfType(Constants.MenuScene, EventType.All, true);
        foreach (CustomEvent customEvent in customEvents)
        {
            if (customEvent.GetEventType() == EventType.Message)
            {
                m_MenuDialogsHandler.ShowErrorDialog(customEvent.GetValue<string>());
            }
        }
    }

    public void ChangePanelFromUI(string panelName)
    {
        PanelType panelType = PanelType.None;
        if (System.Enum.TryParse<PanelType>(panelName, true, out panelType))
        {
            ChangePanel(panelType);
        }
        else
        {
            LogManager.Error(TAG, "OpenPanelFromUI", $"Panel name not found. Panel name provided: {panelName}");
        }
    }

    private void ChangePanel(PanelType panelToOpen)
    {
        HidePanels();
        m_MenuDialogsHandler.HideDialogs();

        switch (panelToOpen)
        {
            case PanelType.MainMenu: m_MainMenuPanel?.SetActive(true); break;
            case PanelType.CreateLobby: m_CreateLobbyPanel?.SetActive(true); break;
            case PanelType.LobbyList: m_LobbyListPanel?.SetActive(true); break;
        }
    }

    public void HidePanels()
    {
        m_MainMenuPanel?.SetActive(false);
        m_CreateLobbyPanel?.SetActive(false);
        m_LobbyListPanel?.SetActive(false);
    }

    public void ConnectWithHost()
    {
        m_MenuDialogsHandler.ShowLoadingDialog(TextCodes.Joining);
        m_EOSLobby.SearchLobbyById(m_CodeTextField.text);
    }

    private void OnFindLobbiesFailure(string errorMessage)
    {
        m_MenuDialogsHandler.HideDialogs();
        m_MenuDialogsHandler.ShowErrorDialog(TextCodes.Error.Lobby.Find);
        LogManager.Debug(TAG, "OnFindLobbiesFailure", errorMessage);
    }

    private void OnFindingCodeLobbyFailure(bool success, string errorMessage)
    {
        m_MenuDialogsHandler.HideDialogs();
        m_MenuDialogsHandler.ShowErrorDialog(success ? TextCodes.Error.Lobby.FindCodeNotFOund : TextCodes.Error.Lobby.FindCode);
        LogManager.Debug(TAG, "OnFindingCodeLobbyFailure", errorMessage);
    }

    private void OnFindingCodeLobbySuccess(LobbyDetails lobbyDetail)
    {
        m_EOSLobby.JoinLobby(lobbyDetail, new string[] { Constants.LobbyName });
    }

    private void OnJoinLobbyFailure(string errorMessage)
    {
        m_MenuDialogsHandler.HideDialogs();
        m_MenuDialogsHandler.ShowErrorDialog(TextCodes.Error.Lobby.Join);
        LogManager.Debug(TAG, "OnJoinLobbyFailure", errorMessage);
    }

    private void OnJoinLobbySuccedded(List<Attribute> lobbyAttributes)
    {
        NetworkManager netManager = m_EOSLobby.GetComponent<NetworkManager>();

        netManager.networkAddress = lobbyAttributes.Find((x) => x.Data.Key == Enumerators.GameSetting.Host_Address.ToString()).Data.Value.AsUtf8;
        netManager.StartClient();
    }

    private void OnCodeTextFieldDeselect(string code)
    {
        m_JoinLobbyButton.interactable = code.Length == Constants.LobbyCodeLength;
    }
}
