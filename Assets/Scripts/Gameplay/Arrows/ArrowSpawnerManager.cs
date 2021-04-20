using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System;
using System.Linq;

public class ArrowSpawnerManager : NetworkBehaviour
{
    private BallisticBehaviour[] m_Ballistics;

    private float m_CirclePonderation = 2f;
    private int m_NextBallisticToShoot = 0;
    private PlayerInfo m_Player;

    private void Start()
    {
        m_Player = NetworkClient.connection.identity.GetComponent<PlayerInfo>();

        m_Ballistics = FindObjectsOfType<BallisticBehaviour>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CmdShootNextBallistic(m_Player.GetPlayerPosition(), Enumerators.ArrowType.Normal);
        }
    }

    string radius = "10";

    private void OnGUI()
    {
        radius = GUI.TextField(new Rect(0, Screen.height - 40, 100, 20), radius);

        if (GUI.Button(new Rect(110, Screen.height - 40, 100, 20), "Shoot"))
        {
            CmdShootCutCircle(Int16.Parse(radius));
        }

        if (GUI.Button(new Rect(110, Screen.height - 60, 100, 20), "End Game"))
        {
            CmdEndGame();
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdEndGame()
    {
        ((CustomNetworkManager)NetworkManager.singleton).EndGame();
    }

    [Command(requiresAuthority = false)]
    private void CmdShootCutCircle(int radius)
    {
        foreach (Vector3 pos in GetCirclePositions(radius))
        {
            CmdShootNextBallistic(pos, Enumerators.ArrowType.Circle);
        }
    }

    [Command(requiresAuthority = false)]
    private void CmdShootNextBallistic(Vector3 pos, Enumerators.ArrowType arrowType)
    {
        m_Ballistics[m_NextBallisticToShoot++].ShootArrow(pos, arrowType);

        if (m_NextBallisticToShoot >= m_Ballistics.Length)
        {
            m_NextBallisticToShoot = 0;
        }

    }

    private Vector3[] GetCirclePositions(int radius)
    {
        int size = Mathf.RoundToInt(radius * 2 * Mathf.PI * m_CirclePonderation);
        float thetaScale = 1f / (size - 1f);
        Vector3[] positions = new Vector3[size];

        float theta = 0.0f;

        for (int i = 0; i < positions.Length; i++)
        {
            theta += (2.0f * Mathf.PI * thetaScale);
            float x = radius * Mathf.Cos(theta);
            float z = radius * Mathf.Sin(theta);
            positions[i] = new Vector3(x, 0, z);
        }
        return positions;
    }
}
