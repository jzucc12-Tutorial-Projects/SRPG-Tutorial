using UnityEngine;

/// <summary>
/// Forces an object to face the camera
/// </summary>
public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private bool invert = true;
    private Transform cameraTransform = null;

    
    private void Awake()
    {
        cameraTransform = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if(invert)
        {
            Vector3 dirToCamera = (cameraTransform.position - transform.position).normalized;
            transform.LookAt(transform.position + dirToCamera * -1);
        }
        else
        {
            transform.LookAt(cameraTransform);
        }
    }
}
