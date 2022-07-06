using System;
using UnityEngine;

/// <summary>
/// Resupplies appropriate actions to the acting unit;
/// </summary>
public class SupplyCrate : MonoBehaviour, IInteractable
{
    public GridCell GetGridCell() => transform.position.GetGridCell();

    public Vector3 GetWorldPosition() => transform.position.PlaceOnGrid();

    public void Interact(Unit actor, Action OnComplete)
    {
        foreach(var supply in actor.GetComponents<ISupply>())
            supply.Resupply();

        ActionLogListener.Publish($"{actor.GetName()} resupplied");
        OnComplete();
    }
}
