using System;
/// <summary>
/// Holds data on an action the ai will take
/// </summary>
public class EnemyAIAction
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

    public void PerformAction(Action onComplete)
    {
        var select = action as IOnSelectAction;
        if(select != null) select.OnSelected();
        action.TakeAction(targetCell, onComplete);
        if(select != null) select.OnUnSelected();
    }
}