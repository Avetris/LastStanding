using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System;
using System.Linq;

public class BallisticBehaviour : NetworkBehaviour
{
    [SerializeField] private GameObject arrowPrefab;

    public void ShootArrow(Vector3 to)
    {
        Vector3 from = transform.position;
        Quaternion rotation = Utils.GetRotationBetweenVectors(from, to);
        GameObject arrowInstance = Instantiate(arrowPrefab, from, rotation);
        NetworkServer.Spawn(arrowInstance);
        arrowInstance.GetComponent<ArrowMovement>().Shoot(from, to, 5f);
    }
}
