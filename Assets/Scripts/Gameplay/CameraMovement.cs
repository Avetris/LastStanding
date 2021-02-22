using System;
using UnityEngine;
using Mirror;

public class CameraMovement : NetworkBehaviour {

    [SerializeField] private Transform playerPosition = null;
    [SerializeField] private Transform cameraPosition = null;

    Vector3 difference = Vector3.zero;

    public override void OnStartClient()
    {
        if(hasAuthority){
            cameraPosition.gameObject.SetActive(true);
            difference = cameraPosition.position - playerPosition.position;
        }
    }

    public override void OnStopClient()
    {
        cameraPosition.gameObject.SetActive(false);
    }

    private void Update() {      
        if(hasAuthority){
            cameraPosition.position = new Vector3(playerPosition.position.x + difference.x, cameraPosition.position.y, playerPosition.position.z + difference.z);
        }
    }
    
}