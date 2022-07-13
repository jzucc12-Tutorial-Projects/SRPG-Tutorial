using System;
using UnityEngine;

/// <summary>
/// Sets up lightning bolt colliders
/// </summary>
public class LightningBolt : MonoBehaviour
{
    [SerializeField] private ParticleSystem particles = null;
    ParticleSystem.MainModule main => particles.main;
    [SerializeField] private LightningBoltCollider[] colliders = new LightningBoltCollider[0];


    public void SetUp(Action<GameObject> damageAction, Vector3 direction)
    {
        foreach(var collider in colliders)
            collider.Setup(transform.position, main.startSpeed.constant * direction, damageAction);
    }
}
