using System;
using UnityEngine;

/// <summary>
/// Controls the animator for a given unit
/// </summary>
public class UnitAnimator : MonoBehaviour
{
    #region //Variables
    [SerializeField] private UnitWeapon unitWeapon = null;
    [SerializeField] private Animator animator = null;
    private Action Act;
    private Action End;
    #endregion


    #region //Monobehaviour
    private void OnEnable()
    {
        unitWeapon.OnWeaponSwap += ChangeController;

        foreach(var animatedAction in unitWeapon.GetComponents<IAnimatedAction>())
        {
            animatedAction.SetAnimatedAction += SetAnimatedAction;
            animatedAction.SetTrigger += SetTrigger;
        }

        if(unitWeapon.TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.StartMoving += StartMoving;
            moveAction.StopMoving += StopMoving;
        }

        if(unitWeapon.TryGetComponent<UnitHealth>(out UnitHealth unitHealth))
        {
            unitHealth.OnDamage += TakeDamage;
        }
    }

    private void OnDisable()
    {
        unitWeapon.OnWeaponSwap -= ChangeController;

        foreach(var animatedAction in unitWeapon.GetComponents<IAnimatedAction>())
        {
            animatedAction.SetAnimatedAction -= SetAnimatedAction;
            animatedAction.SetTrigger -= SetTrigger;
        }

        if(unitWeapon.TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.StartMoving -= StartMoving;
            moveAction.StopMoving -= StopMoving;
        }

        if(unitWeapon.TryGetComponent<UnitHealth>(out UnitHealth unitHealth))
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
    //Called from animation events. Call when you want the action's effect to take place, like as a sword hits its target
    public void CurrentAct() => Act?.Invoke();

    //Called from  the animation events. Call at the end of the animation.
    public void CurrentEnd() 
    {
        End?.Invoke();
        ResetAnimatedAction();
    }

    private void SetAnimatedAction(IAnimatedAction animatedAction)
    {
        Act = animatedAction.AnimationAct;
        End = animatedAction.AnimationEnd;
    }

    private void ResetAnimatedAction()
    {
        Act = null;
        End = null;
    }
    #endregion
}