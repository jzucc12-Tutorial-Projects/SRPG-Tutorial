using System;
using UnityEngine;

public class ShootAction : TargetedAction, IAnimatedAction
{
    #region //Weapon info
    [Header("Weapon Info")]
    [SerializeField] private string weaponName = "Rifle";
    [SerializeField] private GameObject weaponGO = null;
    [SerializeField] private Bullet bulletPrefab = null;
    [SerializeField] private Transform bulletOrigin = null;
    [SerializeField] private AnimatorOverrideController animController = null;
    #endregion

    #region //Firing info
    [Header("Firing Info")]
    [Tooltip("Shots before a reload is needed")] [SerializeField] private int maxClip = 6;
    [SerializeField] private int damage = 40;
    [SerializeField] private AccuracySO accuracySO = null;
    public static event Action OnShootStatic;
    #endregion

    #region //Weapon state
    private ITargetable target = null;
    private int currentClip;
    public event Action<IAnimatedAction> SetAnimatedAction;
    public event Action<string> SetTrigger;
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
        SetAnimatedAction?.Invoke(this);
        base.TakeAction(gridPosition, onFinish);
    }

    protected override void OnFacing()
    {
        SetTrigger?.Invoke("isShooting");
        OnShootStatic?.Invoke();
    }

    private void Shoot()
    {
        currentClip--;
        var bullet = Instantiate(bulletPrefab, bulletOrigin.position, Quaternion.identity);
        var bulletTarget = target.GetWorldPosition();
        bulletTarget.y = bullet.transform.position.y;
        bullet.SetUp(bulletTarget);

        int accuracy = accuracySO.CalculateAccuracy(bulletOrigin.position, target, circularRange);
        accuracy += unit.GetAccuracyMod();
        float damageDealt = damage * accuracySO.ShotHits(accuracy) * unit.GetDamageMod();
        target.Damage((int)damageDealt);
    }

    public override void TakeAltAction(Action onFinish)
    {
        currentClip = maxClip;
        OnActionFinish = onFinish;
        ActionFinish();
    }
    #endregion

    #region //Action selection
    public override void OnSelected()
    {
        unit.SetActiveWeapon(weaponGO, animController);
    }

    public override bool IsValidAction(GridPosition gridPosition)
    {
        if(currentClip <= 0) return false;
        return base.IsValidAction(gridPosition);
    }

    public override bool CanTakeAltAction() => currentClip < maxClip;
    #endregion

    #region //Enemy action
    public override EnemyAIAction GetEnemyAIAction(GridPosition position)
    {
        ITargetable target = LevelGrid.instance.GetUnitAtGridPosition(position);
        Unit targetUnit = (Unit)target;
        if(targetUnit == null) return new EnemyAIAction(position, Mathf.RoundToInt(10));
        else return new EnemyAIAction(position, Mathf.RoundToInt(100f - 1f * targetUnit.GetHealthPercentage()));
    }
    #endregion

    #region //Animated Action
    public void AnimationAct()
    {
        Shoot();
    }

    public void AnimationEnd()
    {
        ActionFinish();
    }
    #endregion

    #region //Getters
    public override int GetQuantity() => currentClip;
    public override string GetActionName() => currentClip > 0 ? weaponName : $"Reload {weaponName}";
    public Unit GetUnit() => unit;
    public ITargetable GetTarget() => target;
    protected override Vector3 GetTargetPosition() => target.GetWorldPosition();
    #endregion
}