using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Grenade that also deals damage and creates explosion effect
/// </summary>
public class Grenade : Projectile
{
    #region //Grenade variables
    [Header("Grenade")]
    [SerializeField] private MeshRenderer grenade = null;
    [SerializeField] private int explosionRadius = 2;
    [SerializeField] private int damage = 25;
    [Tooltip("Damage multiplier to the target cell")] [SerializeField] private int onTargetMult = 2;
    [SerializeField] private LayerMask obstacleLayer = 0;
    private GridCell targetCell => target.GetGridCell();
    public static Action OnCollisionStatic;
    private Unit thrower = null;
    #endregion


    protected override void Collision()
    {
        grenade.enabled = false;
        var levelGrid = FindObjectOfType<LevelGrid>();
        OnCollisionStatic?.Invoke();
        foreach(var targetable in GetTargets(targetCell))
        {
            Vector3 dir = targetable.GetWorldPosition() - transform.position;
            if(Physics.Raycast(target, dir, dir.magnitude, obstacleLayer)) continue;
            var targetDamage = GetDamage(targetCell == targetable.GetGridCell());
            targetable.Damage(thrower, targetDamage);
        }
    }

    public IEnumerable<ITargetable> GetTargets(GridCell targetCell)
    {
        var levelGrid = FindObjectOfType<LevelGrid>();
        foreach(var position in levelGrid.CheckGridRange(targetCell, explosionRadius, true, true))
        {
            var targetable = position.GetTargetable();
            if(targetable == null) continue;
            yield return targetable;
        } 
    }

    public int GetExplosionRadius() => explosionRadius;

    public int GetDamage(bool onTarget)
    {
        if(onTarget) return damage * onTargetMult;
        else return damage;
    }

    public void SetUp(Unit thrower, Vector3 target, Action OnCollision = null)
    {
        grenade.enabled = true;
        this.thrower = thrower;
        SetUp(target, OnCollision);
    }
}