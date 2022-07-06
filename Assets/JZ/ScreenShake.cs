using UnityEngine;
using Cinemachine;

/// <summary>
/// Shakes the game camera
/// </summary>
public class ScreenShake : MonoBehaviour
{
    private CinemachineImpulseSource impulseSource = null;

    private void Awake()
    {
        impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void Shake(float intensity = 1f)
    {
        impulseSource.GenerateImpulse(intensity);
    }
}
