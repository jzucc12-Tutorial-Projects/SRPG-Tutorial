using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual instance { get; private set; }

    #region //Grid variables
    [SerializeField] private GridSystemVisualSingle visualPrefab = null;
    private GridSystemVisualSingle[,] singles;
    [SerializeField] private List<GridVisualType> gridMaterials = new List<GridVisualType>();
    private Unit currentUnit = null;
    private BaseAction currentAction = null;
    #endregion


    #region //Monobehaviour
    private void Awake()
    {
        singles = new GridSystemVisualSingle[LevelGrid.instance.GetWidth(), LevelGrid.instance.GetHeight()];
        for(int x = 0; x < LevelGrid.instance.GetWidth(); x ++)
        {
            for(int z = 0; z < LevelGrid.instance.GetHeight(); z++)
            {
                GridPosition position = new GridPosition(x, z);
                var single = Instantiate(visualPrefab, LevelGrid.instance.GetWorldPosition(position), Quaternion.identity);
                single.transform.parent = transform;
                singles[x,z] = single;
            }
        }

        if(instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void OnEnable()
    {
        LevelGrid.instance.OnAnyUnitMove += UpdateGridVisual;
        UnitActionSystem.UpdateUI += UpdateGridVisual;
    }

    private void OnDisable()
    {
        LevelGrid.instance.OnAnyUnitMove -= UpdateGridVisual;
        UnitActionSystem.UpdateUI -= UpdateGridVisual;
    }
    #endregion
    
    #region //Showing visuals
    private void UpdateGridVisual(Unit newUnit, BaseAction newAction)
    {
        currentUnit = newUnit;
        currentAction = newAction;
        UpdateGridVisual();
    }

    public void UpdateGridVisual()
    {
        HideAll();
        if(currentUnit == null) return;
        var targetedAction = currentAction as TargetedAction;
        
        if(targetedAction != null)
        {
            var positions = targetedAction.GetRangePositions(currentUnit.GetGridPosition());
            ShowPositions(positions, currentAction, GetGridMaterial(currentAction, false)); 
        }

        ShowPositions(currentAction.GetValidPositions(), currentAction, GetGridMaterial(currentAction, targetedAction != null));
    }

    private void HideAll()
    {
        foreach(var single in singles)
            single.Hide();
    }

    private void ShowPositions(List<GridPosition> positions, BaseAction action, Material material)
    {
        foreach(var position in positions)
            singles[position.x, position.z].Show(material);
    }

    private Material GetGridMaterial(BaseAction action, bool useTargeted)
    {
        foreach(var gridMaterial in gridMaterials)
        {
            if(!gridMaterial.actionNames.Contains(action.GetType().ToString())) continue;
            if(useTargeted)
            {
                var tar = (TargetedVisualType) gridMaterial;
                return tar.targetedMaterial;
            }
            else return gridMaterial.baseMaterial;
        }
        return null;
    }
    #endregion
}
