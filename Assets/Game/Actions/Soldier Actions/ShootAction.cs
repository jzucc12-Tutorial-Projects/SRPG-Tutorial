using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unit can use fire arms
/// </summary>
public class ShootAction : TargetedAction, IAnimatedAction, ISupply
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
    public static event Action<Dictionary<ITargetable, (int, int)>> Targeting;
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
        Resupply();
    }
    #endregion

    #region //Action performing
    public override void TakeAction(GridCell gridCell, Action onFinish)
    {
        target = gridCell.GetTargetable();
        SetAnimatedAction?.Invoke(this);
        base.TakeAction(gridCell, onFinish);
    }

    protected override void OnFacing()
    {
        SetTrigger?.Invoke("isShooting");
        OnShootStatic?.Invoke();
    }

    private void Shoot()
    {
        //Spawn bullet
        currentClip--;
        var bullet = Instantiate(bulletPrefab, bulletOrigin.position, Quaternion.identity);
        var bulletTarget = target.GetWorldPosition();
        bulletTarget.y = bullet.transform.position.y;
        bullet.SetUp(bulletTarget);

        //Calculate damage
        int accuracy = accuracySO.CalculateAccuracy(bulletOrigin.position, target, circularRange);
        accuracy += unit.GetAccuracyMod();
        float hitModifier = accuracySO.DamageMult(accuracy);
        int damageDealt = (int)(damage * hitModifier * unit.GetDamageMod());

        //Damage infliction
        if(hitModifier == 0) CallLog($"{unit.GetName()} missed {target.GetName()}");
        else 
        {
            string hitType;
            if(hitModifier == 1) hitType = "hit";
            else hitType = "crit"; 

            CallLog($"{unit.GetName()} {hitType} {target.GetName()} for {damageDealt} damage");
        }
        target.Damage(damageDealt);
    }

    //Reloading
    public override void TakeAltAction(Action onFinish)
    {
        Resupply();
        OnActionFinish = onFinish;
        CallLog($"{unit.GetName()} reloaded their {weaponName}");
        ActionFinish();
    }

    public void Resupply()
    {
        currentClip = maxClip;
    }
    #endregion

    #region //Action selection
    //Shows accuracy UI and sets unit weapon
    public override void OnSelected()
    {
        Dictionary<ITargetable, (int, int)> targets = new Dictionary<ITargetable, (int, int)>();
        unitWeapon.SetActiveWeapon(weaponGO, animController);
        foreach(var gridCell in GetTargetedCells(unit.GetGridCell()))
        {
            ITargetable target = gridCell.GetTargetable();
            int accuracy = accuracySO.CalculateAccuracy(unit.GetWorldPosition(), target, circularRange);
            accuracy += unit.GetAccuracyMod();
            int crit = accuracySO.CalculateCritChance(accuracy);
            targets.Add(target, (accuracy, crit));
        }
        if(targets.Count > 0) Targeting?.Invoke(targets);
    }

    public override void OnUnSelected()
    {
        StopTargeting?.Invoke();
    }

    public override bool CanTakeAction(GridCell gridCell)
    {
        useReloadAPCost = false;
        if(currentClip <= 0) return false;
        return base.CanTakeAction(gridCell);
    }

    public override bool CanTakeAltAction()
    {
        useReloadAPCost = true;
        return currentClip < maxClip;
    }
    #endregion

    #region //Enemy action
    public override EnemyAIAction GetEnemyAIAction(GridCell cell)
    {
        ITargetable target = cell.GetUnit();
        Unit targetUnit = (Unit)target;
        if(targetUnit == null) return new EnemyAIAction(cell, Mathf.RoundToInt(10));
        else return new EnemyAIAction(cell, Mathf.RoundToInt(100f - 1f * targetUnit.GetHealthPercentage()));
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

    #region //Tooltip
    protected override void SetUpToolTip()
    {
        base.SetUpToolTip();
        toolTip.costText += $", {reloadAPCost} to reload";
        toolTip.effectText = "Shoot a target in range";
        toolTip.altText = "Reload this weapon"; 
        toolTip.damageText = $"{damage} on hit, {damage*accuracySO.GetCritMult()} on crit";
        toolTip.accuracyText = $"{accuracySO.GetBaseAccuracy()} to hit, {accuracySO.GetCritChance()} to crit";
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
    protected override Vector3 GetTargetPosition() => target.GetWorldPosition().PlaceOnGrid();
    #endregion
}