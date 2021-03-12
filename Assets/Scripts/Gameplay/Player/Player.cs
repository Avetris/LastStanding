using UnityEngine;
using Mirror;
using System;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour
{
    [SerializeField] private GameObject playerCamera;

    [SyncVar(hook = nameof(AuthorityHandlePartyOwnerStateUpdated))]
    private bool isPartyOwner = false;

    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;

    private void Start()
    {       
        SceneManager.activeSceneChanged += OnSceneChanged;
        Scene currentScene = SceneManager.GetActiveScene();
        OnSceneChanged(currentScene, currentScene);
    }
    
    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {        
        if(this == null || gameObject == null){ return; }

        GetComponentInChildren<FaceCamera>().ResetMainCamera();
        playerCamera.SetActive(false);
        if(hasAuthority)
        {
            playerCamera.SetActive(true);
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