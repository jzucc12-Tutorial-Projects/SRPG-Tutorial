using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    #region //Variables
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

    #region //Action Performing
    public virtual void TakeAction(GridPosition gridPosition, Action onFinish)
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

    public abstract List<GridPosition> GetValidPositions();
    public virtual bool IsValidAction(GridPosition gridPosition)
    {
        return GetValidPositions().Contains(gridPosition);
    }
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
    public abstract string GetActionName();
    public virtual int GetPointCost() => 1;
    #endregion

}
