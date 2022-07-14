using System;
using System.Collections;
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
    #endregion

    #region //Action variables
    public static event Action actionTaken;
    public static event Action<bool> ChangeBusy;
    private BaseAction selectedAction = null;
    private bool isBusy = false;
    private bool doubleClick = false;
    #endregion

    #region //Turn changing
    private bool team1Turn = true;
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
        inputManager.mouseClick.performed += HandleMouseClick;
        inputManager.changeTeammates.started += HandleShiftUnit;
        inputManager.doubleClick.performed += Double;
        BaseAction.OnAnyActionEnded += OnActionFinish;
        Unit.UnitDead += OnPlayerDeath;
        UnitManager.GameOver += GameOver;
        TurnSystem.IncrementTurnLate += TurnChange;
        Pause.OnPause += OnPause;
    }

    private void OnDisable()
    {
        inputManager.mouseClick.performed -= HandleMouseClick;
        inputManager.changeTeammates.started -= HandleShiftUnit;
        BaseAction.OnAnyActionEnded -= OnActionFinish;
        Unit.UnitDead -= OnPlayerDeath;
        UnitManager.GameOver -= GameOver;
        TurnSystem.IncrementTurnLate -= TurnChange;
        Pause.OnPause -= OnPause;
    }

    private void Start()
    {
        SetSelectedUnit(GetDefaultUnit(team1Turn));
        ClearBusy();
    }
    #endregion

    #region //Handle inputs
    private void HandleMouseClick(InputAction.CallbackContext context)
    {
        if(!AllowInput()) return;

        //Only allow action selection on single click
        if(!doubleClick) 
        {
            doubleClick = true;
            StartCoroutine(WaitForDoubleClick());
        }
        else 
        {
            doubleClick = false;
            StopAllCoroutines();
        }
    }

    private IEnumerator WaitForDoubleClick()
    {
        yield return new WaitForSeconds(0.2f);
        HandleSelectedAction();
        doubleClick = false;
    }


    private void Double(InputAction.CallbackContext context)
    {
        TrySelectUnit();
    }

    private void HandleShiftUnit(InputAction.CallbackContext context)
    {
        if(!AllowInput()) return;
        if(selectedUnit == null) return;
        int shift = (int)inputManager.changeTeammates.ReadValue<float>();
        var newUnit = unitManager.GetShiftUnit(selectedUnit, team1Turn, shift);
        SetSelectedUnit(newUnit);
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
        if(team1Turn ^ unit.IsTeam1()) return false;
        if(unit == selectedUnit) return false;
        SetSelectedUnit(unit);
        return true;
    }

    private void OnPlayerDeath(Unit unit)
    {
        if(unit.IsAI()) return;
        if(unit == selectedUnit) 
        {
            SetSelectedUnit(GetDefaultUnit(team1Turn));
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

    private Unit GetDefaultUnit(bool isTeam1) => unitManager.GetRootUnit(isTeam1);
    #endregion

    #region //Action selection
    public void SetSelectedAction(BaseAction action)
    {
        var select = selectedAction as IOnSelectAction;
        if(select != null) select.OnUnSelected();
        selectedAction = action;
        select = selectedAction as IOnSelectAction;
        if(select != null) select.OnSelected();
        UIChange();
        IsSelectedValid();
    }

    private void HandleSelectedAction()
    {
        if(selectedUnit == null || selectedAction == null) return;
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

    #region //Turn changing
    private void TurnChange(bool team1Turn)
    {
        this.team1Turn = team1Turn;
        allowInput = !IsAITurn();
        Unit unit = IsAITurn() ? null : GetDefaultUnit(team1Turn);
        SetSelectedUnit(unit);
        UIChange();
    }
    private bool IsAITurn() => GameGlobals.IsAI(team1Turn);
    #endregion

    #region //Halting
    private void GameOver()
    {
        UpdateUI?.Invoke(null, null);
        OnPause(true);
    }

    private void OnPause(bool pause)
    {
        allowInput = !pause;
    }
    #endregion
}