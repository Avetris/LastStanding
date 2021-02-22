using UnityEngine;
using Mirror;
using TMPro;

public class Character : NetworkBehaviour
{    
    [SerializeField] private TMP_Text nameText = null;

    private Player player;

    bool isAlive = true;

    private void Start() {        
        player = NetworkClient.connection.identity.GetComponent<Player>();
        player.ClientOnInfoUpdated += ClientHandleInfoUpdated;
    }

    private void OnDestroy() {
        player.ClientOnInfoUpdated -= ClientHandleInfoUpdated;
    }
     
    #region Server
    

    #endregion

    #region Client
    public override void OnStartClient()
    {
        if (NetworkServer.active) { return; }        
    }

    public override void OnStopClient()
    {

    }

    private void ClientHandleStatusUpdated(bool oldIsAlive, bool newIsAlive)
    {
        if(newIsAlive)
        {
            // controls.Enable();
        }
        else
        {
            // controls.Disable();
        }
    }    

    private void ClientHandleInfoUpdated()
    {
        nameText.SetText(player.GetDisplayName());
        nameText.SetText(player.GetDisplayName());
    }    
    #endregion

}