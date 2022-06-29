using System;
using UnityEngine;

public class GrenadeAction : TargetedAction
{
    #region //Variables
    [SerializeField] private Grenade grenadePrefab = null;
    #endregion


    #region //Action taking
    public override EnemyAIAction GetEnemyAIAction(GridPosition position)
    {
        return new EnemyAIAction(position, 0);
    }

    public override void TakeAction(GridPosition gridPosition, Action onFinish)
    {
        var grenade = Instantiate(grenadePrefab, unit.GetWorldPosition(), Quaternion.identity);
        grenade.SetUp(LevelGrid.instance.GetWorldPosition(gridPosition), ActionFinish);
        ActionStart(onFinish);
    }
    #endregion

    #region //Getters
    public override string GetActionName() => "Grenade";
    #endregion
}
