using System;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAction : BaseAction
{
    #region //Weapon variables
    [SerializeField] private int range = 1;
    [SerializeField] private float beforeHitTimer = 0.5f;
    [SerializeField] private float afterHitTimer = 0.5f;
    #endregion

    #region //State
    private enum State
    {
        BeforeHit,
        AfterHit
    }
    private State currentState = State.BeforeHit;
    private float currentStateTime = 1f;
    private Unit targetUnit = null;
    public event Action OnMeleeStarted;
    public static event Action OnMeleeStatic;
    #endregion


    #region //Monobehaviour
    private void Update()
    {
        if (!isActive) return;
        currentStateTime -= Time.deltaTime;

        switch (currentState)
        {
            case State.BeforeHit:
                Vector3 aimDir = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                unit.GetAction<MoveAction>().Rotate(aimDir);
                break;

            case State.AfterHit:
                break;
        }

        if (currentStateTime > 0) return;
        NextState();
    }
    #endregion

    #region //Weapon state
    private void NextState()
    {
        switch (currentState)
        {
            case State.BeforeHit:
                currentState = State.AfterHit;
                currentStateTime = afterHitTimer;
                OnMeleeStatic?.Invoke();
                targetUnit.Damage(100);
                break;

            case State.AfterHit:
                ActionFinish();
                break;
        }
    }
    #endregion

    #region //Action performing
    public override void TakeAction(GridPosition gridPosition, Action onFinish)
    {
        targetUnit = LevelGrid.instance.GetUnitAtGridPosition(gridPosition);
        currentState = State.BeforeHit;
        currentStateTime = beforeHitTimer;
        OnMeleeStarted?.Invoke();
        base.TakeAction(gridPosition, onFinish);
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition position)
    {
        return new EnemyAIAction(position, 150);
    }

    public override List<GridPosition> GetValidPositions()
    {
        List<GridPosition> validPositions = new List<GridPosition>();
        var unitPosition = unit.GetGridPosition();

        foreach(var position in LevelGrid.instance.CheckGridRange(unitPosition, range, false))
        {
            if(position == unitPosition) continue;
            if(!LevelGrid.instance.HasAnyUnit(position)) continue;
            var target = LevelGrid.instance.GetUnitAtGridPosition(position);
            if(unit.IsEnemy() == target.IsEnemy()) continue;
            validPositions.Add(position);
        }

        return validPositions;
    }
    #endregion

    #region //Getters
    public override string GetActionName() => "Melee";
    public int GetRange() => range;
    #endregion
}