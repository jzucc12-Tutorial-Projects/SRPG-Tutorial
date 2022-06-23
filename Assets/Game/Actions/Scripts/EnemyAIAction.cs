public class EnemyAIAction
{
    public GridPosition gridPosition = new GridPosition();
    public int actionValue = 0;

    public EnemyAIAction(GridPosition position, int actionValue)
    {
        this.gridPosition = position;
        this.actionValue = actionValue;
    }
}
