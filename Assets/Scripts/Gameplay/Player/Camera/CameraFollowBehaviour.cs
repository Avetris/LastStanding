using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CameraFollowBehaviour : NetworkBehaviour
{
    [SerializeField] private float m_SmoothSpeed = 0.125f;
    [SerializeField] private Vector3 offset = Vector3.zero;

    private Transform m_PlayerTransform;

    private float m_InitialHeight = 0.0f;

    // Update is called once per frame
    void Update()
    {
        if (m_PlayerTransform == null)
        {
            if (NetworkClient.connection.identity != null)
            {
                m_PlayerTransform = NetworkClient.connection.identity.transform;
                
                m_InitialHeight = m_PlayerTransform.position.y + offset.y; 

                foreach (FaceCamera faceCamera in FindObjectsOfType<FaceCamera>())
                {
                    faceCamera.ResetMainCamera(transform);
                }
            }
        }
    }

    private void LateUpdate()
    {
        if (m_PlayerTransform != null)
        {
            Vector3 desiredPosition = m_PlayerTransform.position + offset;
            desiredPosition.y = m_InitialHeight;
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, m_SmoothSpeed);

            transform.position = smoothedPosition;

            transform.LookAt(m_PlayerTransform);
        }
    }
}
