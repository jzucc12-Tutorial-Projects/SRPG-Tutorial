using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Resupplies appropriate actions to the acting unit;
/// </summary>
public class SupplyCrate : MonoBehaviour, IInteractable
{
    public GridCell GetGridCell() => transform.position.GetGridCell();
    public Vector3 GetWorldPosition() => transform.position.PlaceOnGrid();
    public Image tooltip = null;
    private MouseWorld mouseWorld = null;


    private void Awake()
    {
        mouseWorld = FindObjectOfType<MouseWorld>();
    }

    public void Interact(Unit actor, Action OnComplete)
    {
        foreach(var supply in actor.GetComponents<ISupply>())
            supply.Resupply();

        ActionLogListener.Publish($"{actor.GetName()} resupplied");
        OnComplete();
    }

    private void OnMouseEnter() 
    {
        tooltip.gameObject.SetActive(true);
    }

    private void OnMouseExit()
    {
        tooltip.gameObject.SetActive(false);
    }
}
