using System;
using UnityEngine;

public interface ITargetable
{
    bool CanBeTargeted(Unit attackingUnit, bool isHealing);
    void Damage(int damage);
    GridPosition GetGridPosition();
    Vector3 GetWorldPosition();
    Vector3 GetTargetPosition();
    string GetTargetName();
}