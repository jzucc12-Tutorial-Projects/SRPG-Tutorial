/// <summary>
/// Holds position and score for enemy action determination
/// </summary>
public class EnemyAIAction
{
    public GridCell gridCell = new GridCell();
    public int actionValue = 0;

    public EnemyAIAction(GridCell cell, int actionValue)
    {
        this.gridCell = cell;
        this.actionValue = actionValue;
    }
}
