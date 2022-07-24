using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unit can throw grenades
/// </summary>
public class GrenadeAction : TargetedAction, IAnimatedAction, ISupply, IOnSelectAction
{
    #region //Variables
    [Header("Grenade Action")]
    [SerializeField] private Transform spawnPoint = null;
    [SerializeField] private int maxQuantity = 3;
    private Grenade grenadePrefab = null;
    private Grenade activeGrenade = null;
    private int currentQuantity;
    private Vector3 target;
    private MouseWorld mouseWorld = null;
    private EffectsManager effectsManager = null;
    #endregion

    #region //Animated Action
    public event Action<IAnimatedAction> SetAnimatedAction;
    public event Action<string> SetTrigger;
    #endregion


    #region //Monobehaviour
    protected override void Awake()
    {
        base.Awake();
        Resupply();
        mouseWorld = FindObjectOfType<MouseWorld>();
        effectsManager = FindObjectOfType<EffectsManager>();
    }

    protected override void Start()
    {
        grenadePrefab = effectsManager.GetGrenadeReference();
        base.Start();
    }
    #endregion

    #region //Action performing
    public override void TakeAction(GridCell gridCell, Action onFinish)
    {
        target = gridCell.GetWorldPosition();
        SetAnimatedAction?.Invoke(this);
        base.TakeAction(gridCell, onFinish);
    }

    protected override void OnFacing()
    {
        SetTrigger?.Invoke("Grenade Throw");
        CallLog($"{unit.GetName()} threw a grenade");
        unitWeapon.HideActiveWeapon();
    }

    public void Resupply()
    {
        currentQuantity = maxQuantity;
    }
    #endregion

    #region //Action selection
    public override bool CanSelectAction(int currentAP)
    {
        return currentQuantity > 0 && base.CanSelectAction(currentAP);
    }

    public void OnSelected()
    {
        if(unit.IsAI()) return;
        mouseWorld.SetAOESize(grenadePrefab.GetExplosionRadius(), false);
    }

    public void OnUnSelected()
    {
        if(unit.IsAI()) return;
        mouseWorld.ResetAOE();
    }
    #endregion

    #region //Enemy action
    protected override int GetScore(EnemyAIActionList actionList, GridCell unitCell, GridCell targetCell)
    {
        float quantityMod = currentQuantity / maxQuantity;
        AIDamageVars vars = new AIDamageVars(grenadePrefab.GetDamage(false), 40, 25, 25);

        var targets = new List<ITargetable>();
        int score = unit.AOEScoring(grenadePrefab.GetTargets(targetCell), vars, -0.5f, -30, 0.9f);

        Unit targetUnit = levelGrid.GetUnit(targetCell);
        if(targetUnit != null)
        {
            score += 30 * (targetUnit.IsAI() ? -1 : 2);
        }
        return Mathf.RoundToInt(score * quantityMod);
    }
    #endregion

    #region //Animated Action
    public void AnimationAct()
    {
        currentQuantity--;
        activeGrenade = effectsManager.GetGrenade();
        activeGrenade.transform.position = spawnPoint.position;
        activeGrenade.transform.rotation = Quaternion.identity;
        activeGrenade.SetUp(unit, target);
        unit.PlaySound("grenade throw");
    }

    public void AnimationEnd()
    {
        unitWeapon.ShowActiveWeapon();
        activeGrenade.GetTargets(target.GetGridCell());
        ActionFinish(activeGrenade.GetTargetCells());
        activeGrenade = null;
    }
    #endregion

    #region //Tooltip
    protected override void SpecificTooltipSetup()
    {
        base.SpecificTooltipSetup();
        tooltip.effectText = "Throw a grenade that damages \nanything close enough";
        tooltip.damageText = $"{grenadePrefab.GetDamage(true)} on target. {grenadePrefab.GetDamage(false)} in AOE";
    }
    #endregion

    #region //Getters
    public override int GetQuantity() => currentQuantity;
    public override string GetActionName() => "Grenade";
    public override Vector3 GetTargetPosition() => target;
    #endregion
}