using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Resupplies appropriate actions to the acting unit;
/// </summary>
public class SupplyCrate : MonoBehaviour, IInteractable
{
    #region //Variables
    [SerializeField] private int maxUses = 2;
    [SerializeField] private Image tooltip = null;
    [SerializeField] private ItemsToRemove[] removeAfterUse = new ItemsToRemove[0];
    private int timesUsed = 0;
    public GridCell GetGridCell() => transform.position.GetGridCell();
    public Vector3 GetWorldPosition() => transform.position.PlaceOnGrid();
    private MouseWorld mouseWorld = null;
    #endregion


    #region //Monobehaviuor
    private void Awake()
    {
        mouseWorld = FindObjectOfType<MouseWorld>();
        UpdateText();
    }
    #endregion

    #region //Interaction
    public void Interact(Unit actor, Action OnComplete)
    {
        timesUsed++;
        foreach(var go in removeAfterUse[timesUsed - 1].itemsToRemove)
            go.SetActive(false);

        Resupply(actor);
        if(timesUsed >= maxUses) 
        {
            Destroy(tooltip.gameObject);
            Destroy(this);
        }
        else
        {
            UpdateText();
        }
        OnComplete();
    }

    private static void Resupply(Unit actor)
    {
        foreach (var supply in actor.GetComponents<ISupply>())
            supply.Resupply();

        actor.PlaySound("resupply");
        ActionLogListener.Publish($"{actor.GetName()} resupplied");
    }
    #endregion

    #region //Tooltip
    private void UpdateText()
    {
        var tmp = tooltip.GetComponentInChildren<TextMeshProUGUI>();
        tmp.text = tmp.text.Remove(tmp.text.Length - 5);
        tmp.text += $"{maxUses - timesUsed} / {maxUses}";
    }

    private void OnMouseEnter() 
    {
        tooltip.gameObject.SetActive(true);
    }

    private void OnMouseExit()
    {
        tooltip.gameObject.SetActive(false);
    }
    #endregion

    [System.Serializable]
    private struct ItemsToRemove
    {
        public GameObject[] itemsToRemove;
    }
}

