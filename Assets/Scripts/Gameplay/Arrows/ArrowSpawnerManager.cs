using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System;
using System.Linq;

public class ArrowSpawnerManager : NetworkBehaviour
{
    private BallisticBehaviour[] ballistics;

    private float circlePonderation = 2f;
    private int nextBallisticToShoot = 0;
    private PlayerInfo player;

    private void Start() {
        player = NetworkClient.connection.identity.GetComponent<PlayerInfo>();

        ballistics = FindObjectsOfType<BallisticBehaviour>();
    }

    private void Update() {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            ShootNextBallistic(player.GetPlayerPosition(), Enumerators.ArrowType.Normal);
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
            ShootNextBallistic(pos, Enumerators.ArrowType.Circle);
        }
    }

    private void ShootNextBallistic(Vector3 pos, Enumerators.ArrowType arrowType)
    {
        ballistics[nextBallisticToShoot++].ShootArrow(pos, arrowType);

        if(nextBallisticToShoot >= ballistics.Length)
        {
            nextBallisticToShoot = 0;
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
}
