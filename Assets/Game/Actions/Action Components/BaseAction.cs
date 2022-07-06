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
    protected ActionTooltip toolTip = new ActionTooltip();
    #endregion

    #region //Action state
    [Header("Base Action")]
    [SerializeField] private int apCost = 1;
    [Tooltip("True if you don't want to count diagonals in range")] [SerializeField] protected bool circularRange = true;
    [Tooltip("True if the unit can target itself")] [SerializeField] protected bool includeSelf  = false;
    #endregion

    #region //Events
    protected Action OnActionFinish;
    public static event Action<BaseAction> OnAnyActionStarted;
    public static event Action OnAnyActionEnded;
    #endregion


    #region //Monobehaviour
    protected virtual void Awake()
    {
        unit = GetComponent<Unit>();
        unitWeapon = GetComponent<UnitWeapon>();
        levelGrid = FindObjectOfType<LevelGrid>();
        SetUpToolTip();
    }

    protected virtual void Start() { }
    #endregion

    #region //Action performing
    public abstract void TakeAction(GridCell gridCell, Action onFinish);

    //Not needed for all actions. Reloading a weapon is an example of an alt action.
    public virtual void TakeAltAction(Action onFinish) { } 

    protected void ActionStart(Action onFinish)
    {
        OnActionFinish = onFinish;
        OnAnyActionStarted?.Invoke(this);
    }

    protected void ActionFinish()
    {
        OnActionFinish?.Invoke();
        OnAnyActionEnded?.Invoke();
    }

    protected void CallLog(string text)
    {
        ActionLogListener.Publish(text);
    }
    #endregion

    #region //Action Selection
    public virtual void OnSelected() { }
    public virtual void OnUnSelected() { }
    public abstract List<GridCell> GetValidCells(); //Cells that the action can be performed in
    public virtual bool CanTakeAction(GridCell gridCell)
    {
        return CanSelectAction() && GetValidCells().Contains(gridCell);
    }
    public virtual bool CanSelectAction() => unit.GetActionPoints() >= GetPointCost();
    public virtual bool CanTakeAltAction() => false;
    #endregion

    #region //Enemy action
    public EnemyAIAction GetBestAIAction()
    {
        List<EnemyAIAction> actionList = new List<EnemyAIAction>();

        foreach(var cell in GetValidCells())
        {
            EnemyAIAction enemyAction = GetEnemyAIAction(cell);
            actionList.Add(enemyAction);
        }

        if(actionList.Count <= 0) return null;
        actionList.Sort((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue);
        return actionList[0];
    }

    public abstract EnemyAIAction GetEnemyAIAction(GridCell cell);
    #endregion

    #region //Tooltip
    protected virtual void SetUpToolTip()
    {
        toolTip.costText = $"{apCost}AP";
    }
    public ActionTooltip GetToolTip() => toolTip;
    #endregion

    #region //Getters
    public virtual int GetQuantity() => -1; //Leave at -1 for an infinite amount
    public abstract string GetActionName();
    public virtual int GetPointCost() => apCost;
    public bool HasCircularRange() => circularRange;
    public bool IncludeSelf() => includeSelf;
    #endregion
}
