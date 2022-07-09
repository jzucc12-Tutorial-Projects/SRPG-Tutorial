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
    [SerializeField] private int damage = 40;
    [SerializeField] private AccuracySO accuracySO = null;
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

        float hitModifier = accuracySO.DamageMult(CalculateAccuracy(bulletOrigin.position, target));
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

    private int CalculateAccuracy(Vector3 origin, ITargetable target)
    {
        int accuracy = accuracySO.CalculateAccuracy(origin, target, circularRange);
        accuracy += unit.GetAccuracyMod();
        return accuracy;
    }

    public void Resupply()
    {
        currentClip = maxClip;
    }
    #endregion

    #region //Action selection
    public override bool CanSelectAction()
    {
        return currentClip > 0 && base.CanSelectAction();
    }

    //Shows accuracy UI and sets unit weapon
    public override void OnSelected()
    {
        unitWeapon.SetActiveWeapon(weaponGO, animController);

        if(unit.IsEnemy()) return;
        Dictionary<ITargetable, (int, int)> targets = new Dictionary<ITargetable, (int, int)>();
        foreach(var gridCell in GetTargetedCells(unit.GetGridCell()))
        {
            ITargetable target = gridCell.GetTargetable();
            int accuracy = CalculateAccuracy(unit.GetWorldPosition(), target);
            int crit = accuracySO.CalculateCritChance(accuracy);
            targets.Add(target, (accuracy, crit));
        }
        if(targets.Count > 0) Targeting?.Invoke(targets);
    }

    public override void OnUnSelected()
    {
        if(unit.IsEnemy()) return;
        StopTargeting?.Invoke();
    }

    public override bool CanTakeAction(GridCell gridCell)
    {
        if(currentClip <= 0) return false;
        return base.CanTakeAction(gridCell);
    }
    #endregion

    #region //Enemy action
    /// <summary>
    /// Prioritizes units over other targets. Highly prioritizes if it kills.
    /// Drops priority with accuracy and increases with lower target hp.
    /// Aggressively destroys supply crates
    /// </summary>
    /// <param name="unitCell"></param>
    /// <param name="targetCell"></param>
    /// <returns></returns>
    public override EnemyAIAction GetEnemyAIAction(GridCell unitCell, GridCell targetCell)
    {
        //Set up
        int score = 0;
        ITargetable target = targetCell.GetTargetable();

        //Accuracy score
        int accuracy = CalculateAccuracy(unitCell.GetWorldPosition(), target);
        score += accuracy - 100;

        //Target score
        Unit targetUnit = target as Unit;
        SupplyCrate crate = target as SupplyCrate;
        if(crate != null) score += 100;
        else if(targetUnit == null) score += 25;
        else
        {
            int hpDiff = Mathf.RoundToInt(targetUnit.GetHealth() - damage * unit.GetDamageMod());
            score += 100 - hpDiff/2;
            if(hpDiff <= 0)
                score += 120;
        }
        return new EnemyAIAction(this, targetCell, score);
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
        toolTip.effectText = "Shoot a target in range";
        toolTip.altText = "Switch to reload action"; 
        toolTip.damageText = $"{damage} on hit, {damage*accuracySO.GetCritMult()} on crit";
        toolTip.accuracyText = $"{accuracySO.GetBaseAccuracy()} to hit, {accuracySO.GetCritChance()} to crit";
    }
    #endregion

    #region //Getters
    public override int GetQuantity() => currentClip;
    public float GetClipPercent() => (float)currentClip / maxClip;
    public override string GetActionName() => weaponName;
    public Unit GetUnit() => unit;
    public ITargetable GetTarget() => target;
    protected override Vector3 GetTargetPosition() => target.GetWorldPosition().PlaceOnGrid();
    #endregion
}