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
        foreach(var animatedAction in unit.GetComponents<IAnimatedAction>())
        {
            animatedAction.StartRotation += SetAnimatedAction;
        }

        if(unit.TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.StartMoving += StartMoving;
            moveAction.StopMoving += StopMoving;
        }
    }

    private void OnDisable()
    {
        foreach(var animatedAction in unit.GetComponents<IAnimatedAction>())
        {
            animatedAction.StartRotation -= SetAnimatedAction;
        }

        if(unit.TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.StartMoving -= StartMoving;
            moveAction.StopMoving -= StopMoving;
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
    #endregion

    #region //Animated Actions
    public IEnumerator ActionRotation(IAnimatedAction animAction)
    {
        var animData = animAction.GetAnimData();
        Vector3 aimDir = (animData.target - unit.GetWorldPosition()).normalized;

        float dT = 0;
        while(!aimDir.AlmostFacing(transform.forward, animData.facingLimit))
        {
            yield return null;
            unit.Rotate(aimDir);
            dT += Time.deltaTime;
        }

        if(dT < animData.waitTime) yield return new WaitForSeconds(animData.waitTime - dT);
        animAction.OnFacing();
        animator.SetTrigger(animData.triggerName);

        while(unit.Rotate(aimDir))
            yield return null;
    }

    public void CurrentAct() => currentAnimAction.AnimationAct();
    public void CurrentEnd() => currentAnimAction.AnimationEnd();

    private void SetAnimatedAction(IAnimatedAction animatedAction)
    {
        currentAnimAction = animatedAction;
        StartCoroutine(ActionRotation(currentAnimAction));
    }

    private void ResetAnimatedAction()
    {
        currentAnimAction = null;
    }
    #endregion
}