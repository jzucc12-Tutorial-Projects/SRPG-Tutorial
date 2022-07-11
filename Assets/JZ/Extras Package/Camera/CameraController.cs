using UnityEngine;
using Cinemachine;

/// <summary>
/// Moves camera based on player input
/// </summary>
public class CameraController : MonoBehaviour
{
    #region //Camera components
    [SerializeField] private CinemachineVirtualCamera vCam = null;
    private CinemachineTransposer transposer = null;
    private InputManager inputManager = null;
    #endregion

    #region //General input
    private bool allowInput = true;
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
        inputManager = FindObjectOfType<InputManager>();
        transposer = vCam.GetCinemachineComponent<CinemachineTransposer>();
        followOffset = transposer.m_FollowOffset;
    }

    private void OnEnable()
    {
        UnitActionSystem.OnSelectedUnitChanged += MoveToUnit;
        EnemyAIHub.StartNewEnemy += MoveToUnit;
    }

    private void OnDisable()
    {
        UnitActionSystem.OnSelectedUnitChanged -= MoveToUnit;
        EnemyAIHub.StartNewEnemy -= MoveToUnit;
    }

    private void Update()
    {
        if(!allowInput) return;
        MoveCamera();
        RotateCamera();
        ZoomCamera();
    }
    #endregion

    #region //Move and rotate camera
    private void MoveToUnit(Unit newUnit)
    {
        if(newUnit == null) return;
        allowInput = !newUnit.IsEnemy();
        var target = newUnit.GetWorldPosition().PlaceOnGrid();
        transform.position = new Vector3(target.x, transform.position.y, target.z);
    }

    private void MoveCamera()
    {
        Vector3 inputMoveDir = inputManager.GetCameraMove();
        Vector3 moveVector = transform.forward * inputMoveDir.y + transform.right * inputMoveDir.x;
        transform.position += moveVector * moveSpeed * Time.deltaTime;
    }

    private void RotateCamera()
    {
        Vector3 rotationVector = Vector2.zero;
        rotationVector.y = inputManager.GetCameraRotate();
        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;
    }
    #endregion

    #region //Zoom camera
    private void ZoomCamera()
    {
        followOffset.y += zoomAmount * inputManager.GetCameraZoom();
        followOffset.y = Mathf.Clamp(followOffset.y, minZoom, maxZoom);
        transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, followOffset, Time.deltaTime * zoomSpeed);
    }
    #endregion
}
