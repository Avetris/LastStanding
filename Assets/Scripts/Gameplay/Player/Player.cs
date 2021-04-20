using UnityEngine;
using Mirror;
using System;
using UnityEngine.SceneManagement;

public class Player : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnAuthorityPartyOwnerStateUpdated))]
    private bool m_IsPartyOwner = false;

    public static event Action<bool> AuthorityOnPartyOwnerStateUpdated;

    private void Start()
    {
        SceneManager.activeSceneChanged += OnSceneChanged;
        Scene currentScene = SceneManager.GetActiveScene();
        OnSceneChanged(currentScene, currentScene);

        if (hasAuthority)
        {
            GetComponent<CharacterController>().gameObject.layer = LayerMask.NameToLayer("OwnCharacter");
        }
    }


    private void OnSceneChanged(Scene oldScene, Scene newScene)
    {
        if (this == null || gameObject == null) { return; }

        if(!hasAuthority) { return; }

        if (Constants.LobbyScene.Equals(newScene.name))
        {
            this.Invoke(() => AuthorityOnPartyOwnerStateUpdated?.Invoke(m_IsPartyOwner), .1f);
        }
        else
        {
            this.Invoke(() => AuthorityOnPartyOwnerStateUpdated?.Invoke(false), .1f);
        }
    }

    public void SetPartyOwner(bool partyOwner)
    {
        m_IsPartyOwner = partyOwner;
    }

    #region Server

    [Command]
    public void CmdStartGame()
    {
        if (!m_IsPartyOwner || !hasAuthority) { return; }

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

    private void OnAuthorityPartyOwnerStateUpdated(bool oldState, bool newState)
    {
        if (!hasAuthority) { return; }

        AuthorityOnPartyOwnerStateUpdated?.Invoke(newState);
    }
    #endregion
}