using UnityEngine;
using Mirror;
using System;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour
{
    [SerializeField] private GameObject m_PlayerUI = null;
    [SerializeField] private GameObject playerCamera;

    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
    private bool isPartyOwner = false;

    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;

    private void Start()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
        Scene currentScene = SceneManager.GetActiveScene();
        OnSceneChanged(currentScene, currentScene);

        if(hasAuthority)
        {
           GetComponentInChildren<CharacterController>().gameObject.layer = LayerMask.NameToLayer("OwnCharacter");
        }
    }

    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        if (this == null || gameObject == null) { return; }

        GetComponentInChildren<FaceCamera>().ResetMainCamera();
        playerCamera.SetActive(false);
        
        playerCamera.SetActive(hasAuthority);
        m_PlayerUI.SetActive(hasAuthority);      

        if(Constants.LobbyScene.Equals(newScene.name))
        {   
            this.Invoke(() => AuthorityOnPartyOwnerStateUpdated?.Invoke(isPartyOwner), .1f);
        }
    }

    public void SetPartyOwner(bool partyOwner)
    {
        isPartyOwner = partyOwner;
    }

    #region Server

    [Command]
    public void CmdStartGame()
    {
        if (!isPartyOwner || !hasAuthority) { return; }

        ((CustomNetworkManager)NetworkManager.singleton).StartGame();
    }

    public override void OnStartServer()
    {
        DontDestroyOnLoad(gameObject);
    }

    #endregion

    #region Client
    public override void OnStartClient()
    {
        if (NetworkServer.active) { return; }

        DontDestroyOnLoad(gameObject);

        ((CustomNetworkManager)NetworkManager.singleton).ChangePlayerList(true, this);
    }

    public override void OnStopClient()
    {
        if (!isClientOnly) { return; }

        ((CustomNetworkManager)NetworkManager.singleton).ChangePlayerList(false, this);

        if (!hasAuthority) { return; }
    }

    private void AuthorityHandlePartyOwnerStateUpdated(bool oldState, bool newState)
    {
        if (!hasAuthority) { return; }

        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
    }
    #endregion
}