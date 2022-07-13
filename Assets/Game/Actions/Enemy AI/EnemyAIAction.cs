using System;
/// <summary>
/// Holds data on an action the ai will take
/// </summary>
public class EnemyAIAction : IEquatable<EnemyAIAction>
{
    public BaseAction action = null;
    public GridCell targetCell = new GridCell();
    public int score = 0;


    public EnemyAIAction(BaseAction action, GridCell cell, int actionValue)
    {
        this.action = action;
        this.targetCell = cell;
        this.score = actionValue;
    }

    public EnemyAIAction(EnemyAIAction source)
    {
        this.action = source.action;
        this.targetCell = source.targetCell;
        this.score = source.score;
    }

    public int ActionAPCost() => action.GetAPCost();

    public void PerformAction(Action onComplete)
    {
        var select = action as IOnSelectAction;
        if(select != null) select.OnSelected();
        action.TakeAction(targetCell, onComplete);
        if(select != null) select.OnUnSelected();
    }

    public bool TryAlt()
    {
        if(!action.HasAltAction()) return false;
        return action.GetAltAction().GetAPCost() <= action.GetAPCost();
    }

    public BaseAction GetAltAction() => action.GetAltAction();

    public bool Equals(EnemyAIAction other)
    {
        if(this.action != other.action) return false;
        if(this.targetCell != other.targetCell) return false;
        if(this.score != other.score) return false;
        return true;
    }
}