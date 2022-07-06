using System;

/// <summary>
/// Interface given to actions that run off of the unit animator
/// </summary>
public interface IAnimatedAction
{
    /// <summary>
    /// Event recevied by the unit animator to set up the publishing animated action
    /// </summary>
    event Action<IAnimatedAction> SetAnimatedAction;

    /// <summary>
    /// Trigger name to set in the unit animator
    /// </summary>
    event Action<string> SetTrigger;

    /// <summary>
    /// Called in the animation when the action logic should occur. ie: Inflicting damage as the sword connects with its target
    /// </summary>
    void AnimationAct();

    /// <summary>
    /// Called at the end of the animation. Should contain logic to transition out of the action logic
    /// </summary>
    void AnimationEnd();
}