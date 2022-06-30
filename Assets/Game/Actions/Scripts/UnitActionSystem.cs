using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem instance { get; private set; }
    public static event Action UpdateUI;

    #region //Unit variables
    public static event Action<Unit> OnSelectedUnitChanged;
    [SerializeField] private Unit selectedUnit = null;
    [SerializeField] private LayerMask unitMask = -1;
    #endregion

    #region //Action variables
    public static event Action actionTaken;
    public static event Action<bool> ChangeBusy;
    public static event Action<BaseAction> OnSelectedActionChanged;
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
        BaseAction.OnAnyActionEnded += CheckActiveAction;
    }

    private void OnDisable()
    {
        InputManager.instance.mouseClick.started -= HandleMouseClick;
        InputManager.instance.altAction.started -= HandleAltClick;
        BaseAction.OnAnyActionEnded += CheckActiveAction;
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
        OnSelectedUnitChanged?.Invoke(newUnit);
        SetSelectedAction(newUnit.GetRootAction());
    }

    public Unit GetSelectedUnit() => selectedUnit;
    #endregion

    #region //Action selection
    public void SetSelectedAction(BaseAction action)
    {
        selectedAction = action;
        OnSelectedActionChanged?.Invoke(action);
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

    public void CheckActiveAction(BaseAction _)
    {
        UpdateUI?.Invoke();
        if(selectedAction.CanSelectAction()) return;
        SetSelectedAction(selectedUnit.GetRootAction());
    }

    public BaseAction GetSelectedAction() => selectedAction;
    #endregion
}