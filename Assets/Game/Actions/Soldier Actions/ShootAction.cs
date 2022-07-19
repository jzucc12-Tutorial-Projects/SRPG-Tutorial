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
    [SerializeField, ScriptableObjectDropdown(typeof(AccuracySO))] private AccuracySO accuracySO = null;
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

        if(unit.IsAI()) return;
        AccuracyHub.ShowAccuracyUI(unit, GetTargetedCells(unit.GetGridCell()), accuracySO);
    }

    public void OnUnSelected()
    {
        if(unit.IsAI()) return;
        AccuracyHub.HideAccuracyUI();
    }
    #endregion

    #region //Enemy action
    protected override int GetScore(EnemyAIActionList actionList, GridCell unitCell, GridCell targetCell)
    {
        ITargetable target = targetCell.GetTargetable();
        AIDamageVars vars = new AIDamageVars(damage, 105, 20, 10);
        if(actionList.GetAggression() > 5) vars.SetNonUnitValues(25, 15);
        int score = unit.AccuracyDamageScoring(actionList.HasAction<SpinAction>(), target, vars, accuracySO, unitCell.GetWorldPosition());
        return score;
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