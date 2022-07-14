using System;
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
    /// <summary>
    /// Priotizes hitting more targets. Extra points for it being an enemy unit.
    /// Less points if friendly. Even less if it is itself
    /// Likes hitting supply crates
    /// </summary>
    /// <param name="unitCell"></param>
    /// <param name="targetCell"></param>
    /// <returns></returns>
    public override EnemyAIAction GetEnemyAIAction(GridCell unitCell, GridCell targetCell)
    {
        int score = 0;
        int numTargets = 0;

        foreach(var targetable in grenadePrefab.GetTargets(targetCell))
        {
            numTargets++;
            if(targetable is SupplyCrate)
                score += 25;
            else if(!(targetable is Unit))
                score += 10;
            else
            {
                Unit targetUnit = targetable as Unit;
                bool onTarget = targetUnit.GetGridCell() == targetCell;
                int hpDiff = Mathf.RoundToInt(targetUnit.GetHealth() - grenadePrefab.GetDamage(onTarget));
                score += 60 - hpDiff/3;
                if(targetUnit.IsAI()) score *= -1;
                if(targetUnit == unit) score -= 50;
            }
        }

        if(numTargets == 1) score = Mathf.RoundToInt(score * 0.5f);
        float quantityMod = currentQuantity / maxQuantity;
        score = Mathf.RoundToInt(score * quantityMod);
        return new EnemyAIAction(this, targetCell, score);
    }
    #endregion

    #region //Animated Action
    public void AnimationAct()
    {
        currentQuantity--;
        var grenade = effectsManager.GetGrenade();
        grenade.transform.position = spawnPoint.position;
        grenade.transform.rotation = Quaternion.identity;
        grenade.SetUp(unit, target);
    }

    public void AnimationEnd()
    {
        unitWeapon.ShowActiveWeapon();
        ActionFinish();
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