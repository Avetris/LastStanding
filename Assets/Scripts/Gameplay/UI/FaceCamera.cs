using UnityEngine;

public class FaceCamera : MonoBehaviour
{
    private Transform mainCameraTransform;

    // Start is called before the first frame update
    private void Start()
    {
        ResetMainCamera(Camera.main.transform);
    }

    public void ResetMainCamera(Transform transform)
    {
        mainCameraTransform = transform;
    }

    private void LateUpdate()
    {
        if (mainCameraTransform != null)
        {
            transform.LookAt(
                transform.position + mainCameraTransform.rotation * Vector3.forward,
                mainCameraTransform.rotation * Vector3.up);
        }
    }
}
