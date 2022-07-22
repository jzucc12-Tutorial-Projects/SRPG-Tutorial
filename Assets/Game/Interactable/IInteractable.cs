using System;
using UnityEngine;

/// <summary>
/// Objects that "Interact Action" effects
/// </summary>
public interface IInteractable : IObjectInGrid
{
    void Interact(Unit actor, Action onComplete);
    Vector3 GetWorldPosition();
    GridCell GetGridCell();
}