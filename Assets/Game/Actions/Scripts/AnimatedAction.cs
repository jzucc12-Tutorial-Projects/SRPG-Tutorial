using System;
public interface IAnimatedAction
{
    event Action<IAnimatedAction> SetAnimatedAction;
    event Action<string> SetTrigger;
    void AnimationAct();
    void AnimationEnd();
}