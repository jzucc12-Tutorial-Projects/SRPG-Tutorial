using TMPro;
using UnityEngine;

/// <summary>
/// Shows debug info for a given grid cell
/// </summary>
public class PathfindingDebugObject : GridDebugObject
{
    private PathNode pathNode = null;

    #region //UI Components
    [SerializeField] private TextMeshPro fCostText = null;
    [SerializeField] private TextMeshPro gCostText = null;
    [SerializeField] private TextMeshPro hCostText = null;
    [SerializeField] private SpriteRenderer isWalkable = null;
    #endregion


    #region //Monobehaviour
    protected override void Update()
    {
        base.Update();
        fCostText.text = pathNode.GetFCost().ToString();
        gCostText.text = pathNode.GetGCost().ToString();
        hCostText.text = pathNode.GetHCost().ToString();
    }
    #endregion

    #region //Update UI
    public override void SetGridObject(object gridObject)
    {
        base.SetGridObject(gridObject);
        pathNode = (PathNode)gridObject;
        isWalkable.color = pathNode.IsWalkable() ? Color.green : Color.red;
    }
    #endregion
}