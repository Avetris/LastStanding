using kcp2k;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerPreviewCameraController : MonoBehaviour
{
    [SerializeField] private Transform playerPreviewCamera;
    [SerializeField] private Transform characterTransform;
    [SerializeField] private float rotationSpeed = 70f;

    private DialogDisplayHandler dialogDisplayHandler;

    private Enumerators.RotationType rotating;

    public void ChangePreviewCameraStatus(bool enable)
    {
        playerPreviewCamera.gameObject.SetActive(enable);
        if (enable)
        {
            playerPreviewCamera.rotation = Utils.GetRotationBetweenVectors(playerPreviewCamera.transform.position, characterTransform.position);
        }        
    }

    public void ChangeRotation(Enumerators.RotationType newRotationType)
    {
        rotating = newRotationType;
    }

    private void Update() {
        if(rotating != Enumerators.RotationType.None)
        {
            float direction = rotationSpeed * Time.deltaTime;
            if(rotating == Enumerators.RotationType.Left)
            {
                direction *= -1;
            }
            playerPreviewCamera.RotateAround(characterTransform.position, Vector3.up, direction);
        }
    }
}