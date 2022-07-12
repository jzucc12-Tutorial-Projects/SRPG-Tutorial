using System;
using UnityEngine;

/// <summary>
/// Unit can use fire arms
/// </summary>
public class ShootAction : TargetedAction, IAnimatedAction, ISupply, IOnSelectAction
{
    #region //Weapon info
    [Header("Weapon Info")]
    [SerializeField] private string weaponName = "Rifle";
    [SerializeField] private Transform bulletOrigin = null;
    [SerializeField] private GameObject weaponGO = null;
    [SerializeField] private AnimatorOverrideController animController = null;
    private EffectsManager effectsManager = null;
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
        Resupply();
        effectsManager = FindObjectOfType<EffectsManager>();
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

    public void Resupply()
    {
        currentClip = maxClip;
    }
    #endregion

    #region //Action selection
    public override bool CanSelectAction(int currentAP)
    {
        return currentClip > 0 && base.CanSelectAction(currentAP);
    }

    //Shows accuracy UI and sets unit weapon
    public void OnSelected()
    {
        unitWeapon.SetActiveWeapon(weaponGO, animController);

        if(unit.IsEnemy()) return;
        AccuracyHub.ShowAccuracyUI(unit, GetTargetedCells(unit.GetGridCell()), accuracySO);
    }

    public void OnUnSelected()
    {
        if(unit.IsEnemy()) return;
        AccuracyHub.HideAccuracyUI();
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
        int accuracy = accuracySO.CalculateAccuracy(unit, unitCell.GetWorldPosition(), target);
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
        //Spawn bullet
        currentClip--;
        var bullet = effectsManager.GetBullet();
        bullet.transform.position = bulletOrigin.position;
        bullet.transform.rotation = Quaternion.identity;
        var bulletTarget = target.GetWorldPosition();
        bulletTarget.y = bullet.transform.position.y;
        bullet.SetUp(bulletTarget);

        //Calculate damage
        float hitModifier = accuracySO.DamageMult(unit, bulletOrigin.position, target);
        int damageDealt = (int)(damage * hitModifier * unit.GetDamageMod());

        //Damage infliction
        if(hitModifier == 0) CallLog($"{unit.GetName()} missed {target.GetName()}");
        target.Damage(unit, damageDealt);
    }

    public void AnimationEnd()
    {
        ActionFinish();
    }
    #endregion

    #region //Tooltip
    protected override void SpecificTooltipSetup()
    {
        base.SpecificTooltipSetup();
        tooltip.effectText = "Shoot a target in range";
        tooltip.altText = "Switch to reload action"; 
        tooltip.damageText = $"{damage} on hit, {damage*accuracySO.GetCritMult()} on crit";
        tooltip.accuracyText = $"{accuracySO.GetBaseAccuracy()} to hit, {accuracySO.GetCritChance()} to crit";
    }
    #endregion

    #region //Getters
    public override int GetQuantity() => currentClip;
    public float GetClipPercent() => (float)currentClip / maxClip;
    public override string GetActionName() => weaponName;
    public ITargetable GetTarget() => target;
    public override Vector3 GetTargetPosition() => target.GetWorldPosition().PlaceOnGrid();
    #endregion
}