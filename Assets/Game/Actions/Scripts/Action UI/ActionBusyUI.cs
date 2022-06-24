using UnityEngine;

public class ActionBusyUI : MonoBehaviour
{
    [SerializeField] private GameObject busyUI = null;

    
    private void OnEnable()
    {
        UnitActionSystem.instance.ChangeBusy += SetUI;
    }

    private void OnDisable()
    {
        UnitActionSystem.instance.ChangeBusy -= SetUI;
    }

    private void SetUI(bool busy)
    {
        busyUI.SetActive(busy);
    }
}
