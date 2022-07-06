using UnityEngine;

/// <summary>
/// Changes the active camera depending on game state
/// </summary>
public class CameraManager : MonoBehaviour
{
    [SerializeField] private GameObject actionCameraGameObject = null;


    #region //Monobehaviour
    private void OnEnable()
    {
        BaseAction.OnAnyActionStarted += OnActionStarted;
        BaseAction.OnAnyActionEnded += OnActionEnded;
    }

    private void OnDisable()
    {
        BaseAction.OnAnyActionStarted -= OnActionStarted;
        BaseAction.OnAnyActionEnded -= OnActionEnded;
    }
    #endregion

    #region //Camera Transitions
    private void OnActionStarted(BaseAction action)
    {
        switch(action)
        {
            case ShootAction shootAction:
                Unit shooterUnit = shootAction.GetUnit();
                ITargetable target = shootAction.GetTarget();
                Vector3 shootDir = (target.GetWorldPosition() - shooterUnit.GetWorldPosition()).normalized;
                Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDir * 0.5f;
                Vector3 cameraPosition = shooterUnit.GetWorldPosition() + shoulderOffset + (shootDir * -1);
                actionCameraGameObject.transform.position = cameraPosition;
                actionCameraGameObject.transform.LookAt(target.GetWorldPosition());
                ShowActionCamera();
                break;
        }
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