using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    #region //Base variables
    [Header("Base Action")]
    [SerializeField] private int apCost = 1;
    [SerializeField] protected bool circularRange = true;
    [SerializeField] protected bool includeSelf  = false;
    protected Unit unit = null;
    protected bool isActive = false;
    protected Action OnActionFinish;
    public static event Action<BaseAction> OnAnyActionStarted;
    public static event Action<BaseAction> OnAnyActionEnded;
    #endregion


    #region //Monobehaviour
    protected virtual void Awake()
    {
        unit = GetComponent<Unit>();
    }

    protected virtual void Start() { }
    #endregion

    #region //Action performing
    public abstract void TakeAction(GridPosition gridPosition, Action onFinish);

    //Not needed for all actions. Reloading a weapon is an example of an alt action.
    public virtual void TakeAltAction(Action onFinish) { } 

    protected void ActionStart(Action onFinish)
    {
        isActive = true;
        OnActionFinish = onFinish;
        OnAnyActionStarted?.Invoke(this);
    }

    protected void ActionFinish()
    {
        isActive = false;
        OnActionFinish?.Invoke();
        OnAnyActionEnded?.Invoke(this);
    }

    public void CallLog(string text)
    {
        ActionLogListener.Publish(text);
    }
    #endregion

    #region //Action Selection
    public virtual void OnSelected() { }
    public virtual void OnUnSelected() { }
    public abstract List<GridPosition> GetValidPositions();
    public virtual bool IsValidAction(GridPosition gridPosition)
    {
        return CanSelectAction() && GetValidPositions().Contains(gridPosition);
    }
    public virtual bool CanSelectAction() => unit.GetActionPoints() >= GetPointCost();
    public virtual bool CanTakeAltAction() => false;
    #endregion

    #region //Enemy action
    public EnemyAIAction GetBestAIAction()
    {
        List<EnemyAIAction> actionList = new List<EnemyAIAction>();

        foreach(var position in GetValidPositions())
        {
            EnemyAIAction enemyAction = GetEnemyAIAction(position);
            actionList.Add(enemyAction);
        }

        if(actionList.Count <= 0) return null;
        actionList.Sort((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue);
        return actionList[0];
    }

    public abstract EnemyAIAction GetEnemyAIAction(GridPosition position);
    #endregion

    #region //Getters
    public virtual int GetQuantity() => -1; //Leave at -1 for an infinite amount
    public abstract string GetActionName();
    public virtual int GetPointCost() => apCost;
    public bool HasCircularRange() => circularRange;
    public bool IncludeSelf() => includeSelf;
    #endregion
}
