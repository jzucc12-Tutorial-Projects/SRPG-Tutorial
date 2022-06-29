using System;
using UnityEngine;

public class MeleeAction : TargetedAction
{
    #region //Weapon variables
    [SerializeField] private float beforeHitTimer = 0.5f;
    [SerializeField] private float afterHitTimer = 0.5f;
    #endregion

    #region //State
    private enum State
    {
        BeforeHit,
        AfterHit
    }
    private State currentState = State.BeforeHit;
    private float currentStateTime = 1f;
    private ITargetable target = null;
    public event Action OnMeleeStarted;
    public static event Action OnMeleeStatic;
    #endregion


    #region //Monobehaviour
    private void Update()
    {
        if (!isActive) return;
        currentStateTime -= Time.deltaTime;

        switch (currentState)
        {
            case State.BeforeHit:
                Vector3 aimDir = (target.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                unit.GetAction<MoveAction>().Rotate(aimDir);
                break;

            case State.AfterHit:
                break;
        }

        if (currentStateTime > 0) return;
        NextState();
    }
    #endregion

    #region //Weapon state
    private void NextState()
    {
        switch (currentState)
        {
            case State.BeforeHit:
                currentState = State.AfterHit;
                currentStateTime = afterHitTimer;
                OnMeleeStatic?.Invoke();
                target.Damage(100);
                break;

            case State.AfterHit:
                ActionFinish();
                break;
        }
    }
    #endregion

    #region //Action performing
    public override void TakeAction(GridPosition gridPosition, Action onFinish)
    {
        target = LevelGrid.instance.GetTargetableAtGridPosition(gridPosition);
        currentState = State.BeforeHit;
        currentStateTime = beforeHitTimer;
        OnMeleeStarted?.Invoke();
        ActionStart(onFinish);
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition position)
    {
        return new EnemyAIAction(position, 150);
    }
    #endregion

    #region //Getters
    public override string GetActionName() => "Melee";
    #endregion
}