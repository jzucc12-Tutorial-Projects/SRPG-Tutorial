using System;
using System.Collections.Generic;
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
    [SerializeField] private int reloadAPCost = 2;
    [SerializeField] private int damage = 40;
    [SerializeField] private AccuracySO accuracySO = null;
    private bool useReloadAPCost = false;
    public static event Action OnShootStatic;
    public static event Action<List<(ITargetable, int, int)>> Targeting;
    public static event Action StopTargeting;
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
        float hitModifier = accuracySO.ShotHits(accuracy);
        int damageDealt = (int)(damage * hitModifier * unit.GetDamageMod());

        if(hitModifier == 0) CallLog($"{unit.GetName()} missed {target.GetTargetName()}");
        else 
        {
            string hitType;
            if(hitModifier == 1) hitType = "hit";
            else hitType = "crit"; 

            CallLog($"{unit.GetName()} {hitType} {target.GetTargetName()} for {damageDealt} damage");
        }
        target.Damage(damageDealt);
    }

    public override void TakeAltAction(Action onFinish)
    {
        currentClip = maxClip;
        OnActionFinish = onFinish;
        CallLog($"{unit.GetName()} reloaded their {weaponName}");
        ActionFinish();
    }
    #endregion

    #region //Action selection
    public override void OnSelected()
    {
        List<(ITargetable, int, int)> targets = new List<(ITargetable, int, int)>();
        unit.SetActiveWeapon(weaponGO, animController);
        foreach(var position in GetTargetedPositions(unit.GetGridPosition()))
        {
            ITargetable target = LevelGrid.instance.GetTargetableAtGridPosition(position);
            int accuracy = accuracySO.CalculateAccuracy(unit.GetTargetPosition(), target, circularRange);
            accuracy += unit.GetAccuracyMod();
            int crit = accuracySO.CaclulateCritChance(accuracy);
            targets.Add((target, accuracy, crit));
        }
        if(targets.Count > 0) Targeting?.Invoke(targets);
    }

    public override void OnUnSelected()
    {
        StopTargeting?.Invoke();
    }

    public override bool IsValidAction(GridPosition gridPosition)
    {
        useReloadAPCost = false;
        if(currentClip <= 0) return false;
        return base.IsValidAction(gridPosition);
    }

    public override bool CanTakeAltAction()
    {
        useReloadAPCost = true;
        return currentClip < maxClip;
    }
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
    public override int GetPointCost()
    {
        if(useReloadAPCost || currentClip <= 0) return reloadAPCost;
        else return base.GetPointCost();
    }
    public Unit GetUnit() => unit;
    public ITargetable GetTarget() => target;
    protected override Vector3 GetTargetPosition() => target.GetWorldPosition();
    #endregion
}