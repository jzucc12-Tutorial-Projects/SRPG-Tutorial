using System;
using System.Collections.Generic;
using UnityEngine;

public class GrenadeAction : BaseAction
{
    #region //Variables
    [SerializeField] private int throwDistance = 4;
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
        grenade.Setup(gridPosition, ActionFinish);
        base.TakeAction(gridPosition, onFinish);
    }

    public override List<GridPosition> GetValidPositions()
    {
        var validPositions = new List<GridPosition>();

        foreach(var position in LevelGrid.instance.CheckGridRange(unit.GetGridPosition(), throwDistance))
        {
            validPositions.Add(position);
        }

        return validPositions;
    }
    #endregion

    #region //Getters
    public override string GetActionName() => "Grenade";
    public int GetRange() => throwDistance;
    #endregion
}
