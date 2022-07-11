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
    [SerializeField] private float yOffset = 1.5f;
    [SerializeField] private float zOffset = -6;
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
            Unit actingUnit = action.GetUnit();
            Vector3 target = targetCell.GetWorldPosition();
            Vector3 shootDir = (target - actingUnit.GetWorldPosition()).normalized;
            Vector3 cameraPos = actingUnit.GetWorldPosition();
            Vector3 offset = ((shootDir) * zOffset) + new Vector3(0, yOffset, 0);
            camTransform.position = cameraPos + offset;
            camTransform.LookAt(target);
            ShowActionCamera();
        }
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