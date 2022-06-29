using UnityEngine;

public interface ITargetable
{
    bool CanBeTargeted(Unit attackingUnit);
    
    void Damage(int damage);

    GridPosition GetGridPosition();
    Vector3 GetWorldPosition();
}