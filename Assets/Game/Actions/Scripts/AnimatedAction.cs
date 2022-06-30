using System;
using UnityEngine;

public interface IAnimatedAction
{
    event Action<IAnimatedAction> StartRotation;
    void OnFacing();
    void AnimationAct();
    void AnimationEnd();
    AnimData GetAnimData();
}

public struct AnimData
{
    public Vector3 target;
    public string triggerName;
    public float waitTime;
    public float facingLimit;

    public AnimData(Vector3 target, string triggerName, float waitTime, float facingLimit = 0.9f)
    {
        this.target = target;
        this.triggerName = triggerName;
        this.waitTime = waitTime;
        this.facingLimit = facingLimit;
    }

    public AnimData(Vector3 target, string triggerName)
    {
        this.target = target;
        this.triggerName = triggerName;
        this.waitTime = 0;
        this.facingLimit = 0.9f;
    }
}