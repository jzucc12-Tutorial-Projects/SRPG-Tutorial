using UnityEngine;

public class ActionBusyUI : MonoBehaviour
{
    [SerializeField] private GameObject busyUI = null;

    
    private void OnEnable()
    {
        UnitActionSystem.ChangeBusy += SetUI;
    }

    private void OnDisable()
    {
        UnitActionSystem.ChangeBusy -= SetUI;
    }

    private void SetUI(bool busy)
    {
        busyUI.SetActive(busy);
    }
}
