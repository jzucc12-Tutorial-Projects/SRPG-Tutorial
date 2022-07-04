using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem instance { get; private set; }
    public static event Action<Unit, BaseAction> UpdateUI;
    private bool allowInput = true;

    #region //Unit variables
    private Unit selectedUnit = null;
    public static event Action<Unit> OnSelectedUnitChanged;
    [SerializeField] private LayerMask unitMask = -1;
    private Unit defaultUnit => UnitManager.instance.GetPlayerList()[0];
    #endregion

    #region //Action variables
    public static event Action actionTaken;
    public static event Action<bool> ChangeBusy;
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
        TurnSystem.IncrementTurn += TurnChange;
    }

    private void OnDisable()
    {
        InputManager.instance.mouseClick.started -= HandleMouseClick;
        InputManager.instance.altAction.started -= HandleAltClick;
        BaseAction.OnAnyActionEnded -= CheckActiveAction;
        TurnSystem.IncrementTurn -= TurnChange;
    }

    private void Start()
    {
        SetSelectedUnit(defaultUnit);
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
        if(!allowInput) return false;
        return !EventSystem.current.IsPointerOverGameObject();
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

    private void TurnChange(bool isPlayerTurn)
    {
        allowInput = isPlayerTurn;
        Unit unit = isPlayerTurn ? defaultUnit : null;
        SetSelectedUnit(unit);
    }

    private void SetSelectedUnit(Unit newUnit)
    {
        selectedUnit = newUnit;
        OnSelectedUnitChanged?.Invoke(newUnit);
        if(newUnit != null) SetSelectedAction(newUnit.GetRootAction());
        else UpdateUI?.Invoke(selectedUnit, selectedAction);
    }
    #endregion

    #region //Action selection
    public void SetSelectedAction(BaseAction action)
    {
        if(selectedAction != null) selectedAction.OnUnSelected();
        selectedAction = action;
        if(selectedAction != null) selectedAction.OnSelected();
        UpdateUI?.Invoke(selectedUnit, selectedAction);
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
        if(!selectedAction.CanSelectAction())
            SetSelectedAction(selectedUnit.GetRootAction());
        else
            UpdateUI?.Invoke(selectedUnit, selectedAction);
    }
    #endregion
}