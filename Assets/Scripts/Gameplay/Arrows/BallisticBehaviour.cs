using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System;
using System.Linq;

public class BallisticBehaviour : NetworkBehaviour
{
    [SerializeField] private GameObject m_ArrowPrefab;

    public void ShootArrow(Vector3 to, Enumerators.ArrowType arrowType)
    {
        Vector3 from = transform.position;
        Quaternion rotation = Utils.GetRotationBetweenVectors(from, to);
        GameObject arrowInstance = GetArrowType(from, rotation, arrowType);
        NetworkServer.Spawn(arrowInstance);
        arrowInstance.GetComponent<ArrowMovement>().Shoot(from, to, 5f);
    }
    
    private GameObject GetArrowType(Vector3 from, Quaternion rotation, Enumerators.ArrowType arrowType)
    {
        GameObject arrow = null;
        switch(arrowType)
        {
            case Enumerators.ArrowType.Circle:
                arrow = m_ArrowPrefab; 
                arrow.transform.localScale = new Vector3(1f, 1f, 1f);
                break;
            case Enumerators.ArrowType.Normal: 
                arrow = m_ArrowPrefab; 
                arrow.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                break;
            case Enumerators.ArrowType.Fire: 
                arrow = m_ArrowPrefab; 
                arrow.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                break;
        }       
        return  Instantiate(arrow, from, rotation);
    }
}
