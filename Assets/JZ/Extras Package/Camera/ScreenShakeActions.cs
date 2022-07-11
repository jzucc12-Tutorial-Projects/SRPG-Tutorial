using UnityEngine;

/// <summary>
/// Event aggregator for actions that cause screen shaking
/// </summary>
public class ScreenShakeActions : MonoBehaviour
{
    [SerializeField] private ScreenShake shortShake = null;
    [SerializeField] private ScreenShake longShake = null;


    private void OnEnable()
    {
        ShootAction.OnShootStatic += Shake;
        Grenade.OnCollisionStatic += BigShake;
        MeleeAction.OnMeleeStatic += Shake;
        TremorAction.TremorStarted += TremorShake;
    }

    private void OnDisable()
    {
        ShootAction.OnShootStatic -= Shake;
        Grenade.OnCollisionStatic -= BigShake;
        MeleeAction.OnMeleeStatic -= Shake;
        TremorAction.TremorStarted -= TremorShake;
    }

    private void Shake()
    {
        shortShake.ImpulseShake();
    }

    private void BigShake()
    {
        shortShake.ImpulseShake(5);
    }

    private void TremorShake()
    {
        longShake.ImpulseShake(3);
    }
}