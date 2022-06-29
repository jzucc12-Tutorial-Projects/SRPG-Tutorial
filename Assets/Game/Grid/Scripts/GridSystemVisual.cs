using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual instance { get; private set; }

    #region //Grid variables
    [SerializeField] private GridSystemVisualSingle visualPrefab = null;
    private GridSystemVisualSingle[,] singles;
    [SerializeField] private List<GridVisualType> gridMaterials = new List<GridVisualType>();
    #endregion


    #region //Monobehaviour
    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void OnDisable()
    {
        UnitActionSystem.instance.OnSelectedActionChanged -= UpdateGridVisual;
        LevelGrid.instance.OnAnyUnitMove -= UpdateGridVisual;
    }

    private void Start()
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

        UnitActionSystem.instance.OnSelectedActionChanged += UpdateGridVisual;
        LevelGrid.instance.OnAnyUnitMove += UpdateGridVisual;
        UpdateGridVisual();
    }
    #endregion
    
    #region //Showing visuals
    private void HideAll()
    {
        foreach(var single in singles)
            single.Hide();
    }

    private void UpdateGridVisual()
    {
        HideAll();
        var unit = UnitActionSystem.instance.GetSelectedUnit();
        var action = UnitActionSystem.instance.GetSelectedAction();
        var targetedAction = action as TargetedAction;
        
        if(targetedAction != null)
        {
            var positions = targetedAction.GetRangePositions(unit.GetGridPosition());
            ShowPositions(positions, action, GetGridMaterial(action, false)); 
        }

        ShowPositions(action.GetValidPositions(), action, GetGridMaterial(action, targetedAction != null));
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
