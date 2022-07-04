using System;
using UnityEngine;

public class GrenadeAction : TargetedAction, IAnimatedAction
{
    #region //Variables
    [Header("Grenade Action")]
    [SerializeField] private Grenade grenadePrefab = null;
    [SerializeField] private Transform spawnPoint = null;
    [SerializeField] private int maxQuantity = 3;
    private int currentQuantity;
    private Vector3 target;
    #endregion

    #region //Animated Actions
    public event Action<IAnimatedAction> SetAnimatedAction;
    public event Action<string> SetTrigger;
    #endregion


    #region //Monobehaviour
    protected override void Awake()
    {
        base.Awake();
        currentQuantity = maxQuantity;
    }
    #endregion

    #region //Action performing
    public override void TakeAction(GridPosition gridPosition, Action onFinish)
    {
        target = LevelGrid.instance.GetWorldPosition(gridPosition);
        SetAnimatedAction?.Invoke(this);
        base.TakeAction(gridPosition, onFinish);
    }

    protected override void OnFacing()
    {
        SetTrigger?.Invoke("Grenade Throw");
        CallLog($"{unit.GetName()} threw a grenade");
        unit.HideActiveWeapon();
    }
    #endregion

    #region //Action selection
    public override bool CanSelectAction()
    {
        return currentQuantity > 0 && base.CanSelectAction();
    }
    #endregion

    #region //Enemy action
    public override EnemyAIAction GetEnemyAIAction(GridPosition position)
    {
        return new EnemyAIAction(position, 0);
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
        unit.ShowActiveWeapon();
        ActionFinish();
    }
    #endregion

    #region //Getters
    public override int GetQuantity() => currentQuantity;
    public override string GetActionName() => "Grenade";
    protected override Vector3 GetTargetPosition() => target;
    #endregion
}