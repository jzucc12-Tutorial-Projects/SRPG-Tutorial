using UnityEngine;

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
                Unit targetUnit = shootAction.GetTargetUnit();
                Vector3 cameraCharacterHeight = Vector3.up * 1.7f;
                Vector3 shootDir = (targetUnit.GetWorldPosition() - shooterUnit.GetWorldPosition()).normalized;
                Vector3 shoulderOffset = Quaternion.Euler(0, 90, 0) * shootDir * 0.5f;
                Vector3 cameraPosition = shooterUnit.GetWorldPosition() + cameraCharacterHeight + shoulderOffset + (shootDir * -1);
                actionCameraGameObject.transform.position = cameraPosition;
                actionCameraGameObject.transform.LookAt(targetUnit.GetWorldPosition() + cameraCharacterHeight);
                ShowActionCamera();
                break;
        }

    }

    private void OnActionEnded(BaseAction action)
    {
        if(!(action is ShootAction)) return;
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