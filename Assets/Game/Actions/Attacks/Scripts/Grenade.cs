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
    private List<GridCell> targetCells = new List<GridCell>();
    private LevelGrid levelGrid => FindObjectOfType<LevelGrid>();
    #endregion


    #region //Projectile
    protected override void Collision()
    {
        grenade.enabled = false;
        OnCollisionStatic?.Invoke();
        thrower.PlaySound("grenade explode");
        foreach(var targetable in GetTargets(targetCell))
        {
            Vector3 dir = targetable.GetWorldPosition() - transform.position;
            if(Physics.Raycast(target, dir, dir.magnitude, obstacleLayer)) continue;
            var targetDamage = GetDamage(targetCell == targetable.GetGridCell());
            targetable.Damage(thrower, targetDamage, false);
        }
    }

    public void SetUp(Unit thrower, Vector3 target, Action OnCollision = null)
    {
        grenade.enabled = true;
        targetCells = new List<GridCell>();
        this.thrower = thrower;
        SetUp(target, OnCollision);
    }
    #endregion

    #region //Getters
    public IEnumerable<ITargetable> GetTargets(GridCell targetCell)
    {
        foreach(var cell in levelGrid.CheckGridRange(targetCell, explosionRadius, true, true))
        {
            var targetable = levelGrid.GetTargetable(cell);
            if(targetable == null) continue;
            targetCells.Add(cell);
            yield return targetable;
        } 
    }

    public int GetExplosionRadius() => explosionRadius;

    public int GetDamage(bool onTarget)
    {
        if(onTarget) return damage * onTargetMult;
        else return damage;
    }

    public List<GridCell> GetTargetCells() => targetCells;
    #endregion
}