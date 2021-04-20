using kcp2k;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPreviewCameraController : MonoBehaviour
{
    [SerializeField] private Transform m_PlayerPreviewCamera;
    [SerializeField] private float m_RotationSpeed = 70f;

    private DialogDisplayHandler dialogDisplayHandler;

    private Enumerators.RotationType rotating;

    public void ChangePreviewCameraStatus(bool enable)
    {
        m_PlayerPreviewCamera.gameObject.SetActive(enable);
    }

    public void ChangeRotation(Enumerators.RotationType newRotationType)
    {
        rotating = newRotationType;
    }

    private void Update() {
        if(rotating != Enumerators.RotationType.None)
        {
            float direction = m_RotationSpeed * Time.deltaTime;
            if(rotating == Enumerators.RotationType.Left)
            {
                direction *= -1;
            }
            m_PlayerPreviewCamera.RotateAround(transform.position, Vector3.up, direction);
        }
    }
}