using System;
using UnityEngine;

public class ShootAction : TargetedAction
{
    #region //Weapon variables
    [Header("Shoot Action")]
    [SerializeField] private int damage = 40;
    [SerializeField] private int maxClip = 6;
    [SerializeField] private float aimingTimer = 1;
    [SerializeField] private float shootingTimer = 0.1f;
    [SerializeField] private float coolOffTimer = 0.1f;
    private float currentStateTime = 1;
    public event Action<Unit, ITargetable> OnShoot;
    public static event Action OnShootStatic;
    private int currentClip;
    #endregion

    #region //Shooting state Variables
    private bool canShootBullet = false;
    private ITargetable target = null;
    private enum State
    {
        Aiming,
        Shooting,
        Cooloff
    }
    private State currentState;
    #endregion


    #region //Monobehaviour
    protected override void Awake()
    {
        base.Awake();
        currentClip = maxClip;
    }

    private void Update()
    {
        if (!isActive) return;
        currentStateTime -= Time.deltaTime;

        switch (currentState)
        {
            case State.Aiming:
                Vector3 aimDir = (target.GetWorldPosition() - unit.GetWorldPosition()).normalized;
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
                currentClip--;
                ActionFinish();
                break;
        }
    }
    #endregion

    #region //Action performing
    public override void TakeAction(GridPosition gridPosition, Action onFinish)
    {
        target = LevelGrid.instance.GetTargetableAtGridPosition(gridPosition);
        currentState = State.Aiming;
        canShootBullet = true;
        currentStateTime = aimingTimer;
        ActionStart(onFinish);
    }

    public override void TakeAltAction(Action onFinish)
    {
        currentClip = maxClip;
        OnActionFinish = onFinish;
        ActionFinish();
        Debug.Log("alt");
    }

    private void Shoot()
    {
        OnShoot?.Invoke(unit, target);
        OnShootStatic?.Invoke();
        target.Damage(damage);
    }

    public override bool IsValidAction(GridPosition gridPosition)
    {
        if(currentClip <= 0) return false;
        return base.IsValidAction(gridPosition);
    }

    public override bool CanTakeAltAction() => currentClip < maxClip;
    #endregion

    #region //Enemy Action
    public override EnemyAIAction GetEnemyAIAction(GridPosition position)
    {
        ITargetable target = LevelGrid.instance.GetUnitAtGridPosition(position);
        Unit targetUnit = (Unit)target;
        if(targetUnit == null) return new EnemyAIAction(position, Mathf.RoundToInt(10));
        else return new EnemyAIAction(position, Mathf.RoundToInt(100f - 1f * targetUnit.GetHealthPercentage()));
    }
    #endregion

    #region //Getters
    public override int GetQuantity() => currentClip;
    public override string GetActionName() => currentClip > 0 ? "Shoot" : "Reload";
    public Unit GetUnit() => unit;
    public ITargetable GetTarget() => target;
    #endregion
}