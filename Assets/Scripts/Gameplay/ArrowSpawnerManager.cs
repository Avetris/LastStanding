using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System;
using System.Linq;

public class ArrowSpawnerManager : NetworkBehaviour
{
    [SerializeField] private GameObject[] ballistics;
    [SerializeField] private GameObject arrowPrefab;

    private float circlePonderation = 2f;
    private int nextBallisticToShoot = 0;
    private PlayerInfo player;

    private void Start() {
        player = NetworkClient.connection.identity.GetComponent<PlayerInfo>();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            SpawnArrow(transform.position, player.GetPlayerPosition());
        }
    }

    string radius = "10";

    private void OnGUI() {
        radius = GUI.TextField(new Rect(0, Screen.height - 40, 100, 20), radius);

        if(GUI.Button(new Rect(110, Screen.height - 40, 100, 20), "Shoot"))
        {
            ShootCutCircle(Int16.Parse(radius));
        }
        
    }

    private void ShootCutCircle(int radius)
    {

        foreach(Vector3 pos in GetCirclePositions(radius))
        {   
            SpawnArrow(ballistics[nextBallisticToShoot].transform.position, pos);

            nextBallisticToShoot++;

            if(nextBallisticToShoot >= ballistics.Length)
            {
                nextBallisticToShoot = 0;
            }
        }
    }

    private Vector3[] GetCirclePositions(int radius)
    {
        int size = Mathf.RoundToInt(radius * 2 * Mathf.PI * circlePonderation);
        float thetaScale = 1f / (size - 1f);
        Vector3[] positions = new Vector3[size];

        float theta = 0.0f;

        for(int i = 0; i < positions.Length; i++){          
            theta += (2.0f * Mathf.PI * thetaScale);         
            float x = radius * Mathf.Cos(theta);
            float z = radius * Mathf.Sin(theta);          
            positions[i] = new Vector3(x, 0, z);
        }
        return positions;
    }

    private void SpawnArrow(Vector3 from, Vector3 to)
    {
        Quaternion rotation = Utils.GetRotationBetweenVectors(from, to);
        GameObject arrowInstance = Instantiate(arrowPrefab, from, rotation);
        NetworkServer.Spawn(arrowInstance);
        arrowInstance.GetComponent<ArrowMovement>().Shoot(from, to, 5f);
    }
}
