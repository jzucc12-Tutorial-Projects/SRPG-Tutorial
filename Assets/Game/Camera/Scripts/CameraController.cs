using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    #region //Camera components
    [SerializeField] private CinemachineVirtualCamera vCam = null;
    private CinemachineTransposer transposer = null;
    #endregion

    #region //Move and rotation variables
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 100f;
    #endregion

    #region //Zoom variables
    [SerializeField] private float zoomAmount = 1f;
    [SerializeField] private float zoomSpeed = 5f;
    private Vector3 followOffset = new Vector3();
    private const float minZoom = 2f;
    private const float maxZoom = 12f;
    #endregion
    

    #region //Monobehaviour
    private void Awake()
    {
        transposer = vCam.GetCinemachineComponent<CinemachineTransposer>();
        followOffset = transposer.m_FollowOffset;
    }

    private void Update()
    {
        MoveCamera();
        RotateCamera();
        ZoomCamera();
    }
    #endregion

    #region //Move and rotate camera
    private void MoveCamera()
    {
        Vector3 inputMoveDir = InputManager.instance.GetCameraMove();
        Vector3 moveVector = transform.forward * inputMoveDir.y + transform.right * inputMoveDir.x;
        transform.position += moveVector * moveSpeed * Time.deltaTime;
    }

    private void RotateCamera()
    {
        Vector3 rotationVector = Vector2.zero;
        rotationVector.y = InputManager.instance.GetCameraRotate();
        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;
    }
    #endregion

    #region //Zoom camera
    private void ZoomCamera()
    {
        followOffset.y += zoomAmount * InputManager.instance.GetCameraZoom();
        followOffset.y = Mathf.Clamp(followOffset.y, minZoom, maxZoom);
        transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, followOffset, Time.deltaTime * zoomSpeed);
    }
    #endregion
}