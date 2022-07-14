using UnityEngine;
using Cinemachine;
using System.Collections;

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
    [SerializeField] private bool testMode = true;
    private bool snap = false;
    private bool alignWithTarget = false;
    #endregion

    #region //Movement
    [Header("Movement")]
    [SerializeField, Min(0)] private float moveSpeed = 5f;
    [SerializeField, Min(0)] private float unitMoveMult = 8;
    #endregion

    #region //Rotation
    [Header("Rotation")]
    [SerializeField, Min(0)] private float rotationSpeed = 100f;
    [SerializeField, Min(0)] private float unitRotateMult = 7f;
    #endregion

    #region //Zoom
    [Header("Zoom")]
    [SerializeField, Min(0)] private float zoomAmount = 1f;
    [SerializeField, Min(0)] private float zoomSpeed = 5f;
    [SerializeField, Min(0)] private float unitZoomMult = 2;
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
        BaseAction.OnAnyActionStarted += TrackUnit;
        BaseAction.OnAnyActionEnded += StopTracking;
    }

    private void OnDisable()
    {
        UnitActionSystem.OnSelectedUnitChanged -= MoveToUnit;
        EnemyAIHub.StartNewEnemy -= MoveToUnit;
        BaseAction.OnAnyActionStarted -= TrackUnit;
        BaseAction.OnAnyActionEnded -= StopTracking;
    }

    private void Update()
    {
        //For easy switching when I show this to people
        if(testMode && Input.GetKeyDown(KeyCode.M))
            snap = !snap;

        //For easy switching when I show this to people
        if(testMode && Input.GetKeyDown(KeyCode.N))
            alignWithTarget = !alignWithTarget;

        if(!allowInput) return;
        MoveCamera(inputManager.GetCameraMove(), 1);
        RotateCamera(inputManager.GetCameraRotate(), 1);
        ZoomCamera(inputManager.GetCameraZoom(), 1);
    }
    #endregion

    private void TrackUnit(BaseAction action, GridCell targetCell)
    {
        if(!(action is MoveAction)) return;
        var target = action.GetUnit().GetGridCell().GetWorldPosition();
        Vector3 aim = (targetCell.GetWorldPosition() - target).normalized;
        Vector3 moveTarget = new Vector3(target.x, transform.position.y, target.z); 
        StartCoroutine(Track(action.GetUnit(), target, aim));
    }

    private IEnumerator Track(Unit unit, Vector3 target,  Vector3 aim)
    {
        yield return MoveOverTime(target, aim, false);
        while(true)
        {
            Vector3 moveTarget = unit.GetWorldPosition();
            moveTarget.y = transform.position.y;
            transform.position = moveTarget;
            yield return null;
        }
    }

    private void StopTracking()
    {
        StopAllCoroutines();
        allowInput = true;
    }

    private void MoveToUnit(Unit newUnit)
    {
        if(newUnit == null) return;
        var target = newUnit.GetWorldPosition().PlaceOnGrid();
        Vector3 moveTarget = new Vector3(target.x, transform.position.y, target.z);

        if(snap)
        {
            transform.position = moveTarget;
            transform.forward = Vector3.forward;
            allowInput = !newUnit.IsAI();
        }
        else
        {
            StopAllCoroutines();
            allowInput = false;
            StartCoroutine(MoveOverTime(target, newUnit.transform.forward, !newUnit.IsAI()));
        }

    }

    private IEnumerator MoveOverTime(Vector3 movementTarget, Vector3 rotationTarget, bool input)
    {
        if(!alignWithTarget) rotationTarget = Vector3.forward;

        while(transform.position != movementTarget || transform.forward != rotationTarget)
        {
            transform.position = Vector3.MoveTowards(transform.position, movementTarget, unitMoveMult * moveSpeed * Time.deltaTime);
            transform.forward = Vector3.RotateTowards(transform.forward, rotationTarget, rotationSpeed * unitRotateMult * moveSpeed * Time.deltaTime, 1);
            ZoomCamera(1, unitZoomMult);
            yield return null;
        }
        
        allowInput = input;
    }

    #region //Move with input
    private void MoveCamera(Vector3 moveDir, int mult)
    {
        Vector3 moveVector = transform.forward * moveDir.y + transform.right * moveDir.x;
        transform.position += moveVector * moveSpeed * Time.deltaTime * mult;
    }

    private void RotateCamera(float dir, float mult)
    {
        float rotateAmount = dir * mult * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, rotateAmount, 0);
    }

    private void ZoomCamera(float dir, float mult)
    {
        followOffset.y += zoomAmount * dir * mult;
        followOffset.y = Mathf.Clamp(followOffset.y, minZoom, maxZoom);
        transposer.m_FollowOffset = Vector3.Lerp(transposer.m_FollowOffset, followOffset, Time.deltaTime * zoomSpeed);
    }
    #endregion
}