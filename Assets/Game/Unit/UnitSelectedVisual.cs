using System;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour
{
    [SerializeField] private Unit unit = null;
    private MeshRenderer meshRenderer = null;


    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        UpdateVisual(false);
    }

    private void Start()
    {
        UnitActionSystem.instance.OnSelectedUnitChanged += ChangeSelected;
    }

    private void ChangeSelected(object sender, EventArgs args)
    {
        UpdateVisual(UnitActionSystem.instance.GetSelectedUnit() == unit);
    }

    private void UpdateVisual(bool activate)
    {
        meshRenderer.enabled = activate;
    }
}
