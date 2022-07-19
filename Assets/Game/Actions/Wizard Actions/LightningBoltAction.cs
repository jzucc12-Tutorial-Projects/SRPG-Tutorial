using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Fires a line of lightning
/// </summary>
public class LightningBoltAction : CooldownAction, IAnimatedAction, IOnSelectAction
{
    #region //Variables
    [Header("Lightning Bolt Action")]
    [SerializeField] private int damage = 30;
    [SerializeField] private Transform lightningOrigin = null;
    private GridCell target;
    private MouseWorld mouseWorld = null;
    private EffectsManager effectsManager = null;
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
        effectsManager = FindObjectOfType<EffectsManager>();
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
        SetTrigger?.Invoke("Lightning Bolt");
    }

    private void Damage(GameObject target)
    {
        ITargetable targetable = target.GetComponent<ITargetable>();
        if(targetable == null) return;
        if(targetable == GetComponent<ITargetable>()) return;
        Vector3 dir = targetable.GetWorldPosition() - GetTargetPosition();
        targetable.Damage(unit, damage);
    }
    #endregion

    #region //Action selection
    public void OnSelected()
    {
        if(unit.IsAI()) return;
        mouseWorld.SetLineMode(true);
    }

    public void OnUnSelected()
    {
        if(unit.IsAI()) return;
        mouseWorld.SetLineMode(false);
    }
    #endregion

    #region //Enemy AI
    protected override int GetScore(EnemyAIActionList actionList, GridCell unitCell, GridCell targetCell)
    {
        AIDamageVars vars = new AIDamageVars(damage, 35, 30, 25);
        vars.SetNonUnitValues(10, 5);
        int score = unit.AOEScoring(GetTargets(unitCell, targetCell), vars, -1, 0, 2/maxCooldown);
        return score + base.GetScore(actionList, unitCell, targetCell);
    }
    #endregion

    #region //Animated action
    public void AnimationAct()
    {
        var lightning = effectsManager.GetLightningBolt();
        lightning.transform.position = lightningOrigin.position;
        lightning.transform.rotation = unit.transform.rotation;
        CallLog($"{unit.GetName()} shot a lightning bolt");
        Vector3 aimDir = target.GetWorldPosition() - unit.GetWorldPosition().PlaceOnGrid();
        lightning.SetUp(Damage, aimDir.normalized);
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
        tooltip.effectText = "Shoot a lightning bolt, damaging\nanything it its path";
        tooltip.altText = "Switch to focus action";
        tooltip.rangeText = "Straight line until it hits a wall";
        tooltip.damageText = damage.ToString();
    }
    #endregion

    #region //Getters
    public override string GetActionName() => "Lightning Bolt";

    public override Vector3 GetTargetPosition() => target.GetWorldPosition();
    private IEnumerable<ITargetable> GetTargets(GridCell unitCell, GridCell targetCell)
    {
        foreach(var cell in levelGrid.GetLine(unitCell, targetCell))
        {
            var targetable = cell.GetTargetable();
            if(targetable == null) continue;
            yield return targetable;
        }
    }
    #endregion
}