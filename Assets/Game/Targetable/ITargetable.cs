using UnityEngine;

/// <summary>
/// Objects that can be targeted by unit attacks
/// </summary>
public interface ITargetable
{
    bool CanBeTargeted(Unit attackingUnit, bool isHealing);
    void Damage(Unit attacker, int damage);
    GridCell GetGridCell();
    Vector3 GetWorldPosition();
    string GetName();
}