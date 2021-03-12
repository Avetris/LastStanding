using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;

public class ArrowSpawnerManager : NetworkBehaviour
{
    [SerializeField] private GameObject[] ballistics;
    [SerializeField] private GameObject arrowPrefab;

    private PlayerInfo player;

    private void Start() {
        player = NetworkClient.connection.identity.GetComponent<PlayerInfo>();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            Vector3 origin = transform.position;
            Quaternion rotation = Utils.GetRotationBetweenVectors(origin, player.GetPlayerPosition());
            GameObject arrowInstance = Instantiate(arrowPrefab, origin, rotation);
            NetworkServer.Spawn(arrowInstance);
            arrowInstance.GetComponent<ArrowMovement>().Shoot(player.GetPlayerPosition(), origin, 5f);
        }
    }
}
