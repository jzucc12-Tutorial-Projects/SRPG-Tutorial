using UnityEngine;

public class ManaBoltAction : TargetedAction
{
    public override string GetActionName() => "Mana bolt";

    public override EnemyAIAction GetEnemyAIAction(GridCell unitCell, GridCell cell)
    {
        throw new System.NotImplementedException();
    }

    protected override Vector3 GetTargetPosition()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnFacing()
    {
        throw new System.NotImplementedException();
    }
}
