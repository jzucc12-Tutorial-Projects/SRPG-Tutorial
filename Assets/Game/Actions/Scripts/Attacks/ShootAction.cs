using System;
using UnityEngine;

public class ShootAction : TargetedAction, IAnimatedAction
{
    #region //Weapon variables
    [Header("Shoot Action")]
    [SerializeField] private int damage = 40;
    [SerializeField] private int maxClip = 6;
    [SerializeField] private Bullet bulletPrefab = null;
    [SerializeField] private Transform bulletOrigin = null;
    public static event Action OnShootStatic;
    private ITargetable target = null;
    private int currentClip;
    #endregion

    #region //Animated Actions
    public event Action<IAnimatedAction> StartRotation;
    #endregion


    #region //Monobehaviour
    protected override void Awake()
    {
        base.Awake();
        currentClip = maxClip;
    }
    #endregion

    #region //Action performing
    public override void TakeAction(GridPosition gridPosition, Action onFinish)
    {
        target = LevelGrid.instance.GetTargetableAtGridPosition(gridPosition);
        StartRotation?.Invoke(this);
        ActionStart(onFinish);
    }

    public override void TakeAltAction(Action onFinish)
    {
        currentClip = maxClip;
        OnActionFinish = onFinish;
        ActionFinish();
    }

    public override bool IsValidAction(GridPosition gridPosition)
    {
        if(currentClip <= 0) return false;
        return base.IsValidAction(gridPosition);
    }

    public override bool CanTakeAltAction() => currentClip < maxClip;

    public override EnemyAIAction GetEnemyAIAction(GridPosition position)
    {
        ITargetable target = LevelGrid.instance.GetUnitAtGridPosition(position);
        Unit targetUnit = (Unit)target;
        if(targetUnit == null) return new EnemyAIAction(position, Mathf.RoundToInt(10));
        else return new EnemyAIAction(position, Mathf.RoundToInt(100f - 1f * targetUnit.GetHealthPercentage()));
    }
    #endregion

    #region //Animated Action
    public void OnFacing()
    {
        OnShootStatic?.Invoke();
    }

    public void AnimationAct()
    {
        if(!isActive) return;
        currentClip--;
        var bullet = Instantiate(bulletPrefab, bulletOrigin.position, Quaternion.identity);
        var bulletTarget = target.GetWorldPosition();
        bulletTarget.y = bullet.transform.position.y;
        bullet.SetUp(bulletTarget);
        target.Damage(damage);
    }

    public void AnimationEnd()
    {
        if(!isActive) return;
        ActionFinish();
    }

    public AnimData GetAnimData() => new AnimData(target.GetWorldPosition(), "isShooting", 0.5f, 0.993f);
    #endregion

    #region //Getters
    public override int GetQuantity() => currentClip;
    public override string GetActionName() => currentClip > 0 ? "Shoot" : "Reload";
    public Unit GetUnit() => unit;
    public ITargetable GetTarget() => target;
    #endregion
}