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
    public override EnemyAIAction GetEnemyAIAction(GridCell cell)
    {
        return new EnemyAIAction(cell, 0);
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