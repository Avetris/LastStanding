using System;
using System.Collections.Generic;
using UnityEngine;

public class CameraObstacleHandler : MonoBehaviour
{

    [SerializeField] private Transform playerPosition = null;
    [SerializeField] private Transform cameraPosition = null;
    [SerializeField] private LayerMask layerMask = new LayerMask();

    List<GameObject> hidenObjects = new List<GameObject>();

    private void Update()
    {
        foreach (GameObject go in hidenObjects)
        {
            SetMaterial(go, LayerMask.NameToLayer("Hiddeable"));
        }

        bool stop = false;

        Vector3 rayDirection = transform.TransformDirection((playerPosition.position - cameraPosition.position).normalized);
        Debug.DrawRay (cameraPosition.position, rayDirection * 1000, Color.red, Time.deltaTime);

        while (!stop)
        {
            if (Physics.Raycast(cameraPosition.position, rayDirection, out RaycastHit hit, Mathf.Infinity, layerMask))
            {
                
                    Debug.Log(hit.transform.gameObject.name);
                if (hit.transform.gameObject.tag == "Player")
                {
                    stop = hit.transform.gameObject == playerPosition.gameObject;
                }
                else
                {
                    SetMaterial(hit.transform.gameObject, LayerMask.NameToLayer("Hidden"));
                    hidenObjects.Add(hit.transform.gameObject);
                }
            }
            else
            {
                stop = true;
            }
        }

    }

    private void SetMaterial(GameObject go, LayerMask layer)
    {
        go.layer = layer;

    }
}