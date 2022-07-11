using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Parent class to all actions
/// </summary>
public abstract class BaseAction : MonoBehaviour
{
    #region //Cached  components
    protected Unit unit = null;
    protected UnitWeapon unitWeapon = null;
    protected LevelGrid levelGrid = null;
    protected ActionTooltip tooltip = new ActionTooltip();
    #endregion

    #region //Action state
    [Header("Base Action")]
    [SerializeField] private int apCost = 1;
    private BaseAction altAction = null;
    #endregion

    #region //Events
    protected Action OnActionFinish;
    public static event Action<BaseAction, GridCell> OnAnyActionStarted;
    public static event Action OnAnyActionEnded;
    #endregion


    #region //Monobehaviour
    protected virtual void Awake()
    {
        unit = GetComponent<Unit>();
        unitWeapon = GetComponent<UnitWeapon>();
        levelGrid = FindObjectOfType<LevelGrid>();
        SpecificTooltipSetup();
    }
    protected virtual void OnEnable() { }
    protected virtual void OnDisable() { }
    protected virtual void Start() { }
    #endregion

    #region //Action performing
    public abstract void TakeAction(GridCell gridCell, Action onFinish);

    protected void ActionStart(Action onFinish, GridCell gridCell)
    {
        OnActionFinish = onFinish;
        OnAnyActionStarted?.Invoke(this, gridCell);
    }

    protected void ActionFinish()
    {
        OnActionFinish?.Invoke();
        OnAnyActionEnded?.Invoke();
        OnActionFinish = null;
    }

    protected void CallLog(string text)
    {
        ActionLogListener.Publish(text);
    }
    
    public void SetAltAction(BaseAction altAction) => this.altAction = altAction;
    #endregion

    #region //Action Selection
    /// <summary>
    /// Cells the action can be performed in from current cell
    /// </summary>
    /// <returns></returns>
    public List<GridCell> GetValidCells() => GetValidCells(unit.GetGridCell());
    /// <summary>
    /// Cells that the action can be performed in from reference cell
    /// </summary>
    /// <param name="unitCell"></param>
    /// <returns></returns>
    public abstract List<GridCell> GetValidCells(GridCell unitCell);

    /// <summary>
    /// If the unit is allowed to take the action at the given location
    /// </summary>
    /// <param name="gridCell"></param>
    /// <returns></returns>
    public virtual bool CanTakeAction(GridCell gridCell)
    {
        return CanSelectAction() && GetValidCells().Contains(gridCell);
    }

    /// <summary>
    /// If the action button is allowed to be clicked on
    /// </summary>
    /// <returns></returns>
    public bool CanSelectAction() => CanSelectAction(unit.GetAP());
    public virtual bool CanSelectAction(int currentAP) => currentAP >= GetAPCost(currentAP);
    #endregion

    #region //Enemy action
    public EnemyAIAction GetBestAIAction(GridCell referenceCell)
    {
        List<EnemyAIAction> actionList = new List<EnemyAIAction>();

        foreach(var targetCell in GetValidCells(referenceCell))
        {
            EnemyAIAction enemyAction = GetEnemyAIAction(referenceCell, targetCell);
            actionList.Add(enemyAction);
        }

        if(actionList.Count <= 0) return null;
        actionList.Sort((EnemyAIAction a, EnemyAIAction b) => b.score - a.score);
        return actionList[0];
    }

    public abstract EnemyAIAction GetEnemyAIAction(GridCell unitCell, GridCell cell);
    #endregion

    #region //Tooltip
    private void SetupTooltip()
    {
        tooltip.costText = $"{apCost}AP";
        SpecificTooltipSetup();
    }
    protected abstract void SpecificTooltipSetup();
    public ActionTooltip GetToolTip() => tooltip;
    #endregion

    #region //Getters
    public Unit GetUnit() => unit;
    public virtual int GetQuantity() => -1; //Leave at -1 for an infinite amount
    public abstract string GetActionName();
    public int GetAPCost() => GetAPCost(unit.GetAP());
    public virtual int GetAPCost(int currentAP) => apCost;
    public bool HasAltAction() => altAction != null;
    public BaseAction GetAltAction() => altAction;
    #endregion
}