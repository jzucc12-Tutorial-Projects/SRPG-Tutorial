using System;
using UnityEngine;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem instance { get; private set; }
    public event EventHandler OnSelectedUnitChanged;
    private Unit selectedUnit = null;
    [SerializeField] private LayerMask unitMask = -1;


    private void Awake()
    {
        if(instance != null) Destroy(gameObject);
        else instance = this;
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(0))
        {
            if(TrySelectUnit()) return;
            if(selectedUnit == null) return;
            selectedUnit.ChangePosition(MouseWorld.GetPosition());
        }
    }

    private bool TrySelectUnit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool didHit = Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, unitMask);
        if(!didHit || !hit.collider.TryGetComponent<Unit>(out Unit unit)) return false;
        SetSelectedUnit(unit);
        return true;
    }

    private void SetSelectedUnit(Unit newUnit)
    {
        selectedUnit = newUnit;
        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetSelectedUnit() => selectedUnit;
}
