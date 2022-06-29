using UnityEngine;

public class Grenade : Projectile
{
    #region //Grenade variables
    [Header("Grenade")]
    [SerializeField] private float explosionRadius = 2;
    [SerializeField] private int damage = 50;
    private GridPosition targetPosition => LevelGrid.instance.GetGridPosition(target);
    #endregion


    protected override void Collision()
    {
        Collider[] hits = Physics.OverlapSphere(target, explosionRadius * LevelConstants.cellSize);
        foreach(var collider in hits)
        {
            if(collider.TryGetComponent<ITargetable>(out ITargetable target))
            {
                var unitDamage = damage * (target.GetGridPosition() == targetPosition ? 2 : 1);
                target.Damage(unitDamage);
            }
        }
    }
}
