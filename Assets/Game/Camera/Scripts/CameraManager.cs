using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Changes the active camera depending on game state
/// </summary>
public class CameraManager : MonoBehaviour
{
    #region //Camera variables
    [SerializeField] private List<string> aimAtTargetActions = new List<string>();
    [SerializeField] private GameObject actionCameraGameObject = null;
    private Transform camTransform => actionCameraGameObject.transform;
    #endregion

    #region //Positioning variables
    [SerializeField] private Vector3 offset = Vector3.zero;
    [SerializeField] private Vector3 blockingOffset = Vector3.zero;
    private Unit initiator = null;
    #endregion


    #region //Monobehaviour
    private void OnEnable()
    {
        BaseAction.OnAnyActionStarted += OnActionStarted;
        BaseAction.OnAnyActionEnded += OnActionEnded;
        Unit.UnitDead += CheckRevert;
    }

    private void OnDisable()
    {
        BaseAction.OnAnyActionStarted -= OnActionStarted;
        BaseAction.OnAnyActionEnded -= OnActionEnded;
        Unit.UnitDead -= CheckRevert;
    }
    #endregion

    #region //Camera Transitions
    private void OnActionStarted(BaseAction action, GridCell targetCell)
    {
        initiator = null;
        if(aimAtTargetActions.Contains(action.GetType().ToString()))
        {
            initiator = action.GetUnit();
            
            Vector3 target = targetCell.GetWorldPosition();
            Vector3 aimDir = (target - initiator.GetWorldPosition()).normalized;
            Vector3 cameraPos = initiator.GetWorldPosition();

            //Increase camera aim if a wall is blocking the target
            Vector3 testPos = cameraPos + GetOffset(offset, aimDir);
            if(Physics.Raycast(testPos, aimDir, (testPos - aimDir).magnitude, GridGlobals.obstacleMask))
                camTransform.position = cameraPos + GetOffset(blockingOffset, aimDir);
            else
                camTransform.position = cameraPos + GetOffset(offset, aimDir);

            camTransform.LookAt(target);
            ShowActionCamera();
        }
    }

    private Vector3 GetOffset(Vector3 offset, Vector3 aim)
    {
        return ((aim) * offset.z) + new Vector3(0, offset.y, 0);
    }

    private void CheckRevert(Unit deadUnit)
    {
        if(initiator == deadUnit)
            HideActionCamera();
    }

    private void OnActionEnded()
    {
        if(!actionCameraGameObject.activeInHierarchy) return;
        HideActionCamera();
    }

    private void ShowActionCamera()
    {
        actionCameraGameObject.SetActive(true);
    }

    private void HideActionCamera()
    {
        actionCameraGameObject.SetActive(false);
    }
    #endregion
}