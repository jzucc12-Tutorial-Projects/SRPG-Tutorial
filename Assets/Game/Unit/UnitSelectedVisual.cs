using System;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour
{
    #region //Variables
    [SerializeField] private Unit unit = null;
    private MeshRenderer meshRenderer = null;
    #endregion


    #region //Monobehaviour
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        UpdateVisual(false);
    }

    private void OnEnable()
    {
        UnitActionSystem.instance.OnSelectedUnitChanged += ChangeSelected;
    }

    private void DisEnable()
    {
        UnitActionSystem.instance.OnSelectedUnitChanged -= ChangeSelected;
    }
    #endregion

    #region //Updating visuals
    private void ChangeSelected(object sender, EventArgs args)
    {
        UpdateVisual(UnitActionSystem.instance.GetSelectedUnit() == unit);
    }

    private void UpdateVisual(bool activate)
    {
        meshRenderer.enabled = activate;
    }
    #endregion
}