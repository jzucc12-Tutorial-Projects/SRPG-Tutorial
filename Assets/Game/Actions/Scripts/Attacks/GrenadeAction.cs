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
    public event Action<IAnimatedAction> StartRotation;
    #endregion


    #region //Monobehaviour
    protected override void Awake()
    {
        base.Awake();
        currentQuantity = maxQuantity;
    }
    #endregion

    #region //Action taking
    public override void TakeAction(GridPosition gridPosition, Action onFinish)
    {
        target = LevelGrid.instance.GetWorldPosition(gridPosition);
        StartRotation?.Invoke(this);
        ActionStart(onFinish);
    }

    public override bool CanSelectAction() => currentQuantity > 0;

    public override EnemyAIAction GetEnemyAIAction(GridPosition position)
    {
        return new EnemyAIAction(position, 0);
    }
    #endregion

    #region //Animated Action
    public void OnFacing()
    {
        unit.HideActiveWeapon();
    }

    public void AnimationAct()
    {
        if(!isActive) return;
        currentQuantity--;
        var grenade = Instantiate(grenadePrefab, spawnPoint.position, Quaternion.identity);
        grenade.SetUp(target);
    }

    public void AnimationEnd()
    {
        if(!isActive) return;
        unit.ShowActiveWeapon();
        ActionFinish();
    }

    public AnimData GetAnimData() => new AnimData(target, "Grenade Throw", 0.25f, 0.993f);
    #endregion

    #region //Getters
    public override int GetQuantity() => currentQuantity;
    public override string GetActionName() => "Grenade";
    #endregion
}