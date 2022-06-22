using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    #region //Variables
    protected Unit unit = null;
    protected bool isActive = false;
    protected Action onActionFinish;
    #endregion


    #region //Monobehaviour
    protected virtual void Awake()
    {
        unit = GetComponent<Unit>();
    }

    protected virtual void Start() { }
    #endregion

    #region //Action Performing
    public abstract void TakeAction(GridPosition gridPosition, Action onFinish);
    public abstract List<GridPosition> GetValidPositions();
    public virtual bool IsValidAction(GridPosition gridPosition)
    {
        return GetValidPositions().Contains(gridPosition);
    }
    #endregion

    #region //Getters
    public abstract string GetActionName();
    public virtual int GetPointCost() => 1;
    #endregion

}
