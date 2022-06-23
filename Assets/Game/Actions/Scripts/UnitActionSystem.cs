using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem instance { get; private set; }

    #region //Unit variables
    public event EventHandler OnSelectedUnitChanged;
    [SerializeField] private Unit selectedUnit = null;
    [SerializeField] private LayerMask unitMask = -1;
    #endregion

    #region //Action variables
    public event Action actionTaken;
    public event Action<bool> ChangeBusy;
    public event Action OnSelectedActionChanged;
    private BaseAction selectedAction = null;
    private bool isBusy = false;
    #endregion


    #region //Monobehaviour
    private void Awake()
    {
        if(instance != null) Destroy(gameObject);
        else instance = this;
    }

    private void Start()
    {
        SetSelectedUnit(selectedUnit);
        ClearBusy();
    }

    private void Update()
    {
        if(isBusy) return;
        if(!Input.GetMouseButtonDown(0)) return;
        if(EventSystem.current.IsPointerOverGameObject()) return;
        if(!TurnSystem.instance.IsPlayerTurn()) return;
        
        if(TrySelectUnit()) return;
        HandleSelectedAction();
    }
    #endregion

    #region //Unit selection
    private bool TrySelectUnit()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        bool didHit = Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, unitMask);
        if(!didHit || !hit.collider.TryGetComponent<Unit>(out Unit unit)) return false;
        if(unit.IsEnemy()) return false;
        if(unit == selectedUnit) return false;
        SetSelectedUnit(unit);
        return true;
    }

    private void SetSelectedUnit(Unit newUnit)
    {
        selectedUnit = newUnit;
        SetSelectedAction(newUnit.GetAction<MoveAction>());
        OnSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    public BaseAction GetSelectedAction() => selectedAction;
    #endregion

    #region //Action selection
    public void SetSelectedAction(BaseAction action)
    {
        selectedAction = action;
        OnSelectedActionChanged?.Invoke();
    }

    private void HandleSelectedAction()
    {
        GridPosition mouseGridPosition = LevelGrid.instance.GetGridPosition(MouseWorld.GetPosition());
        if(!selectedAction.IsValidAction(mouseGridPosition)) return;
        if(!selectedUnit.TryTakeAction(selectedAction)) return;
        selectedAction.TakeAction(mouseGridPosition, ClearBusy);
        actionTaken?.Invoke();
        SetBusy();
    }

    private void SetBusy()
    {
        isBusy = true;
        ChangeBusy?.Invoke(true);
    }
    private void ClearBusy()
    {
        isBusy = false;
        ChangeBusy?.Invoke(false);
    }
    
    public Unit GetSelectedUnit() => selectedUnit;
    #endregion
    

}