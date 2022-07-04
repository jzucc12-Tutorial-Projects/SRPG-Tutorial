using System;
using UnityEngine;

public interface IInteractable
{
    void Interact(Unit actor, Action onComplete);
    Vector3 GetWorldPosition();
    GridPosition GetGridPosition();
}