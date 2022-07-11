using System;
using UnityEngine;

/// <summary>
/// AOE that can be cast through walls. Serves similarly to grenades
/// </summary>
public class FireballAction : CooldownAction, IAnimatedAction, IOnSelectAction
{
    #region //Variables
    [Header("Fireball Action")]
    [SerializeField] private int aoeSize = 2;
    [SerializeField] private int damage = 30;
    [SerializeField] private GameObject fireBallFX = null;
    private GridCell target;
    private MouseWorld mouseWorld = null;
    #endregion

    #region //Animated action
    public event Action<IAnimatedAction> SetAnimatedAction;
    public event Action<string> SetTrigger;
    #endregion


    #region //Monobehaviour
    protected override void Awake()
    {
        base.Awake();
        mouseWorld = FindObjectOfType<MouseWorld>();
    }
    #endregion

    #region //Action performing
    public override void TakeAction(GridCell gridCell, Action onFinish)
    {
        target = gridCell;
        SetAnimatedAction?.Invoke(this);
        base.TakeAction(gridCell, onFinish);
    }

    protected override void OnFacing()
    {
        SetTrigger?.Invoke("Fireball");
    }
    #endregion

    #region //Action selection
    public void OnSelected()
    {
        if(unit.IsEnemy()) return;
        mouseWorld.SetAOESize(aoeSize, false);
    }

    public void OnUnSelected()
    {
        if(unit.IsEnemy()) return;
        mouseWorld.ResetAOE();
    }
    #endregion

    #region //Enemy AI
    public override EnemyAIAction GetEnemyAIAction(GridCell unitCell, GridCell cell)
    {
        throw new System.NotImplementedException();
    }
    #endregion

    #region //Animated action
    public void AnimationAct()
    {
        Instantiate(fireBallFX, GetTargetPosition(), Quaternion.identity);
        CallLog($"{unit.GetName()} summoned a fireball");
        foreach(var cell in levelGrid.CheckGridRange(target, aoeSize, circularRange, true))
        {
            ITargetable targetable = cell.GetTargetable();
            if(targetable == null) continue;
            Vector3 dir = targetable.GetWorldPosition() - GetTargetPosition();
            if(Physics.Raycast(GetTargetPosition(), dir, dir.magnitude, GridGlobals.obstacleMask)) continue;
            targetable.Damage(damage);
            CallLog($"{targetable.GetName()} took {damage} damage");
        }
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
        tooltip.effectText = "Summon a fireball that hurts\nanything within";
        tooltip.altText = "Switch to focus action";
        tooltip.damageText = damage.ToString();
    }
    #endregion

    #region //Getters
    public override string GetActionName() => "Fireball";

    public override Vector3 GetTargetPosition() => target.GetWorldPosition();
    #endregion
}
