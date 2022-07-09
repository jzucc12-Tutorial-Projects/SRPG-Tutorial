using System;
using UnityEngine;

/// <summary>
/// Parent class for projectile effects
/// </summary>
public abstract class Projectile : MonoBehaviour
{
    [Header("Base Projectile")]
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float collisionDistance = 0.1f;
    protected Vector3 target = Vector3.zero;

    [Header("Effects")]
    [SerializeField] private bool hasCollisionParticles = false;
    [SerializeField, ShowIf("hasCollisionParticles")] private ParticleSystem collisionParticles = null;
    [SerializeField] private bool hasTrail = false;
    [SerializeField, ShowIf("hasTrail")] private TrailRenderer trail = null;

    [Header("Projectile Path")]
    [SerializeField] private bool hasCurvedPath = false;
    [SerializeField, ShowIf("hasCurvedPath")] private AnimationCurve projectilePath = null;
    private float totalDistance = 0;


    private void Update()
    {
        MoveProjectile();
        if ((target - transform.position).InRange(collisionDistance))
        {
            Collision();
            transform.position = target;
            if (hasCollisionParticles) Instantiate(collisionParticles, target + Vector3.up, Quaternion.identity);
            if (hasTrail) trail.transform.parent = null;
            Destroy(gameObject);
        }
    }

    private void MoveProjectile()
    {
        Vector3 moveDir = (target - transform.position).normalized;
        Vector3 newPosition = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);

        if (hasCurvedPath)
        {
            float distanceNorm = 1 - Vector3.Distance(newPosition, target) / totalDistance;
            float maxHeight = totalDistance / 4f;
            newPosition.y = projectilePath.Evaluate(distanceNorm) * maxHeight;
        }
        transform.position = newPosition;
    }

    protected abstract void Collision();

    public virtual void SetUp(Vector3 target)
    {
        this.target = target;
        totalDistance = Vector3.Distance(transform.position, target);
    }
}
