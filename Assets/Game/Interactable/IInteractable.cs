using System;
using UnityEngine;

public interface IInteractable
{
    void Interact(Action onComplete);

    public Vector3 GetWorldPosition();
    public GridPosition GetGridPosition();
}