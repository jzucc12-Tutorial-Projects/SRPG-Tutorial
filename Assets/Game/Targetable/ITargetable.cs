using UnityEngine;

/// <summary>
/// Objects that can be targeted by unit attacks
/// </summary>
public interface ITargetable : IObjectInGrid
{
    bool CanBeTargeted(Unit attackingUnit, bool isHealing);
    int Damage(Unit attacker, int damage, bool crit);
    GridCell GetGridCell();
    Vector3 GetWorldPosition();
    string GetName();
}