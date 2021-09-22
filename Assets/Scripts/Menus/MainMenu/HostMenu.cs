using System.Collections.Generic;
using Epic.OnlineServices.Lobby;
using Mirror;
using UnityEngine;
using UnityEngine.UI;

public class HostMenu : MonoBehaviour
{
    private const string TAG = "HostMenu";

    [Header("Buttons & Data Pickers")]
    [SerializeField] private Slider m_MaxPlayers = null;
    [SerializeField] private Toggle m_PermissionToggle = null;
    [SerializeField] private Toggle m_HideLobbyCode = null;
    [SerializeField] private Slider m_ArrowSpawnNumber = null;
    [SerializeField] private Slider m_ArrowSpawnInterval = null;
    [SerializeField] private Slider m_ArrowCircleCloseRadius = null;
    [SerializeField] private Slider m_ArrowCircleSpawnInterval = null;

    private MenuDialogsHandler m_MenuDialogsHandler;
    private EOSLobby m_EOSLobby;

    private void Start()
    {
        m_MenuDialogsHandler = FindObjectOfType<MenuDialogsHandler>();
        m_EOSLobby = FindObjectOfType<EOSLobby>();

        m_EOSLobby.CreateLobbySucceeded += OnCreateLobbySuccess;
        m_EOSLobby.CreateLobbyFailed += OnCreateLobbyFailure;
    }

    private void OnDestroy()
    {
        m_EOSLobby.CreateLobbySucceeded -= OnCreateLobbySuccess;
        m_EOSLobby.CreateLobbyFailed -= OnCreateLobbyFailure;
    }


    public void StartHost()
    {
        Debug.Log("Starting Host");
        LobbyPermissionLevel permission = !m_PermissionToggle.isOn ? LobbyPermissionLevel.Joinviapresence : LobbyPermissionLevel.Publicadvertised;
        AttributeData[] attributes = new AttributeData[]{
            new AttributeData { Key = Enumerators.GameSetting.Arrow_Spawn_Number.ToString(), Value = m_ArrowSpawnNumber.value},
            new AttributeData { Key = Enumerators.GameSetting.Arrow_Spawn_Interval.ToString(), Value = m_ArrowSpawnInterval.value},
            new AttributeData { Key = Enumerators.GameSetting.Arrow_Circle_Close_Radius.ToString(), Value = m_ArrowCircleCloseRadius.value},
            new AttributeData { Key = Enumerators.GameSetting.Arrow_Circle_Spawn_Interval.ToString(), Value = m_ArrowCircleSpawnInterval.value},
            new AttributeData { Key = Enumerators.GameSetting.Hide_Lobby_Code.ToString(), Value = m_HideLobbyCode.isOn}
        };
        Debug.Log("Data Pîcker");
        Debug.Log(m_MenuDialogsHandler);
        m_MenuDialogsHandler.ShowLoadingDialog(TextCodes.Creating);        
        Debug.Log("Show Loading");
        m_EOSLobby.CreateLobby((uint)m_MaxPlayers.value, permission, false, (uint)m_MaxPlayers.value, attributes);
    }

    #region Callbacks

    private void OnCreateLobbySuccess(List<Attribute> attributes)
    {
        m_MenuDialogsHandler.HideDialogs();
        FindObjectOfType<NetworkManager>().StartHost();
    }

    private void OnCreateLobbyFailure(string errorMessage)
    {
        m_MenuDialogsHandler.HideDialogs();
        m_MenuDialogsHandler.ShowErrorDialog(TextCodes.Error.Lobby.Create);
        LogManager.Debug(TAG, "OnCreateLobbyFailure", errorMessage);
    }

    #endregion
}
