using UnityEngine;

public class PlayerPreviewCameraController : MonoBehaviour
{
    [SerializeField] private Transform playerPreviewCamera;
    [SerializeField] private Transform characterTransform;
    [SerializeField] private float rotationSpeed = 70f;

    private Constants.RotationType rotating;

    public void ChangePreviewCameraStatus(bool enable)
    {
        playerPreviewCamera.gameObject.SetActive(enable);
        if (enable)
        {
            Vector3 relativePos = characterTransform.position - playerPreviewCamera.transform.position;

            // the second argument, upwards, defaults to Vector3.up
            Quaternion rotation = Quaternion.LookRotation(relativePos, Vector3.up);
            playerPreviewCamera.rotation = rotation;
        }        
    }

    public void ChangeRotation(Constants.RotationType newRotationType)
    {
        rotating = newRotationType;
    }

    private void Update() {
        if(rotating != Constants.RotationType.None)
        {
            float direction = rotationSpeed * Time.deltaTime;
            if(rotating == Constants.RotationType.Left)
            {
                direction *= -1;
            }
            playerPreviewCamera.RotateAround(characterTransform.position, Vector3.up, direction);
        }
    }
}