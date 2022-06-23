using UnityEngine;

public class ScreenShakeActions : MonoBehaviour
{
    [SerializeField] private ScreenShake screenShake = null;


    private void OnEnable()
    {
        ShootAction.OnShootStatic += Shake;
    }

    private void OnDisable()
    {
        ShootAction.OnShootStatic += Shake;
    }

    private void Shake()
    {
        screenShake.Shake();
    }
}
