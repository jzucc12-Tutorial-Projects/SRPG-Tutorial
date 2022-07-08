using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

/// <summary>
/// Handles player input related to units and actions
/// </summary>
public class UnitActionSystem : MonoBehaviour
{
    public static event Action<Unit, BaseAction> UpdateUI;

    #region //Input
    private bool allowInput = true;
    private MouseWorld mouseWorld = null;
    private InputManager inputManager = null;
    #endregion

    #region //Unit variables
    private UnitManager unitManager = null;
    private Unit selectedUnit = null;
    public static event Action<Unit> OnSelectedUnitChanged;
    [SerializeField] private LayerMask unitMask = -1;
    private Unit defaultUnit => unitManager.GetRootPlayer();
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
        inputManager = FindObjectOfType<InputManager>();
        unitManager = FindObjectOfType<UnitManager>();
        mouseWorld = FindObjectOfType<MouseWorld>();
    }

    private void OnEnable()
    {
        inputManager.mouseClick.started += HandleMouseClick;
        BaseAction.OnAnyActionEnded += OnActionFinish;
        Unit.UnitDead += OnPlayerDeath;
        UnitManager.GameOver += GameOver;
        TurnSystem.IncrementTurnLate += TurnChange;
        Pause.OnPause += Halt;
    }

    private void OnDisable()
    {
        inputManager.mouseClick.started -= HandleMouseClick;
        BaseAction.OnAnyActionEnded -= OnActionFinish;
        Unit.UnitDead -= OnPlayerDeath;
        UnitManager.GameOver -= GameOver;
        TurnSystem.IncrementTurnLate -= TurnChange;
        Pause.OnPause -= Halt;
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
        HandleSelectedAction();
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
        Ray ray = Camera.main.ScreenPointToRay(inputManager.GetMouseScreenPosition());
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
        UIChange();
    }

    private void OnPlayerDeath(Unit unit)
    {
        if(unit.IsEnemy()) return;
        if(unit == selectedUnit) 
        {
            SetSelectedUnit(defaultUnit);
            ClearBusy();
        }
    }

    private void SetSelectedUnit(Unit newUnit)
    {
        selectedUnit = newUnit;
        OnSelectedUnitChanged?.Invoke(newUnit);
        if(newUnit != null) SetSelectedAction(newUnit.GetRootAction());
        else UIChange();
    }
    #endregion

    #region //Action selection
    public void SetSelectedAction(BaseAction action)
    {
        if(selectedAction != null) selectedAction.OnUnSelected();
        selectedAction = action;
        if(selectedAction != null) selectedAction.OnSelected();
        UIChange();
        IsSelectedValid();
    }

    private void HandleSelectedAction()
    {
        if(selectedAction == null) return;
        GridCell targetCell = mouseWorld.GetGridCell();
        if(!selectedUnit.TryTakeAction(selectedAction, targetCell)) return;

        SetBusy();
        selectedAction.TakeAction(targetCell, ClearBusy);
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

    private void OnActionFinish()
    {
        if(IsSelectedValid())
            UIChange();
    }

    private bool IsSelectedValid()
    {
        if(selectedAction == null) return true;
        if(!selectedAction.CanSelectAction())
        {
            SetSelectedAction(selectedUnit.GetRootAction());
            return false;
        }
        return true;
    }

    private void UIChange()
    {
        UpdateUI?.Invoke(selectedUnit, selectedAction);
        mouseWorld.RefreshMouse();
    }
    #endregion

    #region //Halting
    private void GameOver()
    {
        Halt(true);
    }

    private void Halt(bool halt)
    {
        allowInput = !halt;
        if(halt) UpdateUI?.Invoke(null, null);
    }
    #endregion
}