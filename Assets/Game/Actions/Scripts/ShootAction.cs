using System;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    #region //Weapon variables
    [SerializeField] private int weaponRange = 7;
    [SerializeField] private int damage = 40;
    [SerializeField] private float aimingTimer = 1;
    [SerializeField] private float shootingTimer = 0.1f;
    [SerializeField] private float coolOffTimer = 0.1f;
    [SerializeField] private LayerMask obstacleLayer = 0;
    private float currentStateTime = 1;
    public event Action<Unit, Unit> OnShoot;
    public static event Action OnShootStatic;
    #endregion

    #region //Shooting state Variables
    private bool canShootBullet = false;
    private Unit targetUnit = null;
    private enum State
    {
        Aiming,
        Shooting,
        Cooloff
    }
    private State currentState;
    #endregion


    #region //Monobehaviour
    private void Update()
    {
        if (!isActive) return;
        currentStateTime -= Time.deltaTime;

        switch (currentState)
        {
            case State.Aiming:
                Vector3 aimDir = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                unit.GetAction<MoveAction>().Rotate(aimDir);
                break;

            case State.Shooting:
                if(canShootBullet)
                {
                    Shoot();
                    canShootBullet = false;
                }
                break;

            case State.Cooloff:
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
            case State.Aiming:
                currentState = State.Shooting;
                currentStateTime = shootingTimer;
                break;

            case State.Shooting:
                currentState = State.Cooloff;
                currentStateTime = coolOffTimer;
                break;

            case State.Cooloff:
                ActionFinish();
                break;
        }
    }
    #endregion

    #region //Action performing
    public override void TakeAction(GridPosition gridPosition, Action onFinish)
    {
        targetUnit = LevelGrid.instance.GetUnitAtGridPosition(gridPosition);
        currentState = State.Aiming;
        canShootBullet = true;
        currentStateTime = aimingTimer;
        base.TakeAction(gridPosition, onFinish);
    }
    private void Shoot()
    {
        OnShoot?.Invoke(unit, targetUnit);
        OnShootStatic?.Invoke();
        targetUnit.Damage(damage);
    }

    public override List<GridPosition> GetValidPositions()
    {
        return GetValidPositions(unit.GetGridPosition());
    }

    public List<GridPosition> GetValidPositions(GridPosition unitPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        foreach(var position in LevelGrid.instance.CheckGridRange(unitPosition, weaponRange))
        {
            if(!LevelGrid.instance.HasAnyUnit(position)) continue;
            var target = LevelGrid.instance.GetUnitAtGridPosition(position);
            if(unit.IsEnemy() == target.IsEnemy()) continue;

            Vector3 unitWorldPosition = LevelGrid.instance.GetWorldPosition(unitPosition);
            Vector3 shootDir = (target.GetWorldPosition() - unitWorldPosition).normalized;
            float shoulderHeight = 1.7f;
            var hit = Physics.Raycast(unitWorldPosition + Vector3.up * shoulderHeight, shootDir,
                            Vector3.Distance(unitWorldPosition, target.GetWorldPosition()),
                            obstacleLayer);
            if(hit) continue;
            validGridPositionList.Add(position);
        }

        return validGridPositionList;
    }
    #endregion

    #region //Enemy Action
    public override EnemyAIAction GetEnemyAIAction(GridPosition position)
    {
        Unit unit = LevelGrid.instance.GetUnitAtGridPosition(position);
        return new EnemyAIAction(position, Mathf.RoundToInt(100f - 1f * unit.GetHealthPercentage()));
    }
    #endregion

    #region //Getters
    public int GetRange() => weaponRange;
    public override string GetActionName() => "Shoot";
    public Unit GetUnit() => unit;
    public Unit GetTargetUnit() => targetUnit;
    public int GetTargetsAtPosition(GridPosition position)
    {
        return GetValidPositions(position).Count;
    }
    #endregion
}