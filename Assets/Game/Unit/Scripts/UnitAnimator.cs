using System.Collections;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    #region //Variables
    [SerializeField] private Unit unit = null;
    [SerializeField] private Animator animator = null;
    private IAnimatedAction currentAnimAction = null;
    #endregion


    #region //Monobehaviour
    private void OnEnable()
    {
        unit.OnWeaponSwap += ChangeController;

        foreach(var animatedAction in unit.GetComponents<IAnimatedAction>())
        {
            animatedAction.SetAnimatedAction += SetAnimatedAction;
            animatedAction.SetTrigger += SetTrigger;
        }

        if(unit.TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.StartMoving += StartMoving;
            moveAction.StopMoving += StopMoving;
        }

        if(unit.TryGetComponent<UnitHealth>(out UnitHealth unitHealth))
        {
            unitHealth.OnDamage += TakeDamage;
        }
    }

    private void OnDisable()
    {
        unit.OnWeaponSwap -= ChangeController;

        foreach(var animatedAction in unit.GetComponents<IAnimatedAction>())
        {
            animatedAction.SetAnimatedAction -= SetAnimatedAction;
            animatedAction.SetTrigger -= SetTrigger;
        }

        if(unit.TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.StartMoving -= StartMoving;
            moveAction.StopMoving -= StopMoving;
        }

        if(unit.TryGetComponent<UnitHealth>(out UnitHealth unitHealth))
        {
            unitHealth.OnDamage -= TakeDamage;
        }
    }
    #endregion

    #region //Animation setting
    private void StartMoving()
    {
        animator.SetBool("isWalking", true);
    }

    private void StopMoving()
    {
        animator.SetBool("isWalking", false);
    }

    private void TakeDamage() => SetTrigger("Injured");

    private void SetTrigger(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }

    private void ChangeController(AnimatorOverrideController controller)
    {
        animator.runtimeAnimatorController = controller;
    }
    #endregion

    #region //Animated Actions
    public void CurrentAct() => currentAnimAction.AnimationAct();
    public void CurrentEnd() => currentAnimAction.AnimationEnd();

    private void SetAnimatedAction(IAnimatedAction animatedAction)
    {
        currentAnimAction = animatedAction;
    }

    private void ResetAnimatedAction()
    {
        currentAnimAction = null;
    }
    #endregion
}