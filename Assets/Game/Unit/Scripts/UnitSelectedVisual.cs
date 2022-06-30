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
        UnitActionSystem.OnSelectedUnitChanged += ChangeSelected;
    }

    private void OnDisable()
    {
        UnitActionSystem.OnSelectedUnitChanged -= ChangeSelected;
    }
    #endregion

    #region //Updating visuals
    private void ChangeSelected(Unit newUnit)
    {
        UpdateVisual(newUnit == unit);
    }

    private void UpdateVisual(bool activate)
    {
        meshRenderer.enabled = activate;
    }
    #endregion
}