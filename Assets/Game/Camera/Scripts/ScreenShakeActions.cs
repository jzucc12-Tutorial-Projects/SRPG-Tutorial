using UnityEngine;

public class ScreenShakeActions : MonoBehaviour
{
    [SerializeField] private ScreenShake screenShake = null;


    private void OnEnable()
    {
        ShootAction.OnShootStatic += Shake;
        Grenade.OnExplodeStatic += BigShake;
        MeleeAction.OnMeleeStatic += Shake;
    }

    private void OnDisable()
    {
        ShootAction.OnShootStatic -= Shake;
        Grenade.OnExplodeStatic -= BigShake;
        MeleeAction.OnMeleeStatic -= Shake;
    }

    private void Shake()
    {
        screenShake.Shake();
    }

    private void BigShake()
    {
        screenShake.Shake(5);
    }
}
