using System;
using UnityEngine;

/// <summary>
/// Grenade that also deals damage and creates explosion effect
/// </summary>
public class Grenade : Projectile
{
    #region //Grenade variables
    [Header("Grenade")]
    [SerializeField] private int explosionRadius = 2;
    [SerializeField] private int damage = 25;
    [SerializeField] private LayerMask obstacleLayer = 0;
    private GridCell targetCell => target.GetGridCell();
    public static Action OnCollisionStatic;
    #endregion


    protected override void Collision()
    {
        var levelGrid = FindObjectOfType<LevelGrid>();
        OnCollisionStatic?.Invoke();
        foreach(var position in levelGrid.CheckGridRange(targetCell, explosionRadius, true, true))
        {
            var targetable = position.GetTargetable();
            if(targetable != null)
            {
                Vector3 dir = targetable.GetWorldPosition() - transform.position;
                if(Physics.Raycast(target, dir, dir.magnitude, obstacleLayer)) continue;
                var targetDamage = damage * (targetable.GetGridCell() == targetCell ? 2 : 1);
                targetable.Damage(targetDamage);
                ActionLogListener.Publish($"{targetable.GetName()} took {targetDamage} damage");
            }
        }
    }

    public int GetExplosionRadius() => explosionRadius;
}