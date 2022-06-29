using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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

    private void OnEnable()
    {
        InputManager.instance.mouseClick.started += HandleMouseClick;
        InputManager.instance.altAction.started += HandleAltClick;
    }

    private void OnDisable()
    {
        InputManager.instance.mouseClick.started -= HandleMouseClick;
        InputManager.instance.altAction.started -= HandleAltClick;
    }

    private void Start()
    {
        SetSelectedUnit(selectedUnit);
        ClearBusy();
    }
    #endregion

    #region //Handle inputs
    private void HandleMouseClick(InputAction.CallbackContext context)
    {
        if(!AllowInput()) return;
        if(TrySelectUnit()) return;
        HandleSelectedAction(false);
    }

    private void HandleAltClick(InputAction.CallbackContext context)
    {
        if(!AllowInput()) return;
        HandleSelectedAction(true);
    }

    private bool AllowInput()
    {
        if(isBusy) return false;
        if(!TurnSystem.instance.IsPlayerTurn()) return false;
        if(EventSystem.current.IsPointerOverGameObject()) return false;
        return true;
    }
    #endregion


    #region //Unit selection
    private bool TrySelectUnit()
    {
        Ray ray = Camera.main.ScreenPointToRay(InputManager.instance.GetMouseScreenPosition());
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

    private void HandleSelectedAction(bool isAltAction)
    {
        GridPosition mouseGridPosition = LevelGrid.instance.GetGridPosition(MouseWorld.GetPosition());
        if(!isAltAction && !selectedAction.IsValidAction(mouseGridPosition)) return;
        if(isAltAction && !selectedAction.CanTakeAltAction()) return;
        if(!selectedUnit.TryTakeAction(selectedAction)) return;

        SetBusy();
        if(isAltAction) selectedAction.TakeAltAction(ClearBusy);
        else selectedAction.TakeAction(mouseGridPosition, ClearBusy);
        actionTaken?.Invoke();
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