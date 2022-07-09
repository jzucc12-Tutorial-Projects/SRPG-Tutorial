using System;
using UnityEngine;

/// <summary>
/// Unit can throw grenades
/// </summary>
public class GrenadeAction : TargetedAction, IAnimatedAction, ISupply
{
    #region //Variables
    [Header("Grenade Action")]
    [SerializeField] private Grenade grenadePrefab = null;
    [SerializeField] private Transform spawnPoint = null;
    [SerializeField] private int maxQuantity = 3;
    private int currentQuantity;
    private Vector3 target;
    private MouseWorld mouseWorld = null;
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
    public override bool CanSelectAction()
    {
        return currentQuantity > 0 && base.CanSelectAction();
    }

    public override void OnSelected()
    {
        mouseWorld.SetAOESize(grenadePrefab.GetExplosionRadius());
    }

    public override void OnUnSelected()
    {
        mouseWorld.SetAOESize(0);
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

        foreach(var targetable in grenadePrefab.GetTargets(targetCell))
        {
            if(targetable is SupplyCrate)
                score += 25;
            else if(!(targetable is Unit))
                score += 10;
            else
            {
                Unit targetUnit = targetable as Unit;
                bool onTarget = targetUnit.GetGridCell() == targetCell;
                int hpDiff = Mathf.RoundToInt(targetUnit.GetHealth() - grenadePrefab.GetDamage(onTarget));
                score += 50 - hpDiff/3;
                if(targetUnit.IsEnemy()) score *= -1;
                if(targetUnit == unit) score -= 50;
            }
        }
        float quantityMod = 0.9f * currentQuantity / maxQuantity + 0.1f;
        score = Mathf.RoundToInt(score * quantityMod);
        return new EnemyAIAction(this, targetCell, score);
    }
    #endregion

    #region //Animated Action
    public void AnimationAct()
    {
        currentQuantity--;
        var grenade = Instantiate(grenadePrefab, spawnPoint.position, Quaternion.identity);
        grenade.SetUp(target);
    }

    public void AnimationEnd()
    {
        unitWeapon.ShowActiveWeapon();
        ActionFinish();
    }
    #endregion

    #region //Tooltip
    protected override void SetUpToolTip()
    {
        base.SetUpToolTip();
        toolTip.effectText = "Throw a grenade that damages \nanything close enough";
        toolTip.damageText = $"{grenadePrefab.GetDamage(true)} on target. {grenadePrefab.GetDamage(false)} in AOE";
    }
    #endregion

    #region //Getters
    public override int GetQuantity() => currentQuantity;
    public override string GetActionName() => "Grenade";
    protected override Vector3 GetTargetPosition() => target;
    #endregion
}