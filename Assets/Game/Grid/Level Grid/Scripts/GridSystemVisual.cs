using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// UI for displaying the entire level's grid
/// </summary>
public class GridSystemVisual : MonoBehaviour
{
    #region //Grid variables
    [SerializeField] private GridSystemVisualSingle visualPrefab = null;

    [SerializeField] private List<GridVisualType> gridMaterials = new List<GridVisualType>();
    private LevelGrid levelGrid = null;
    private GridSystemVisualSingle[,] singles;
    #endregion

    #region //Active State
    private Unit currentUnit = null;
    private BaseAction currentAction = null;
    private TargetedAction targetedAction => currentAction != null ? currentAction as TargetedAction : null;
    private bool isTargetedAction => targetedAction != null;
    private List<GridCell> currentCells = new List<GridCell>();
    #endregion

    #region //Mouse effects
    [SerializeField] private Material defaultMouseMaterial = null;
    private Material activeMouseMaterial = null;
    #endregion


    #region //Monobehaviour
    private void Awake()
    {
        levelGrid = FindObjectOfType<LevelGrid>();

        singles = new GridSystemVisualSingle[levelGrid.GetWidth(), levelGrid.GetHeight()];
        for(int x = 0; x < levelGrid.GetWidth(); x ++)
        {
            for(int z = 0; z < levelGrid.GetHeight(); z++)
            {
                GridCell cell = new GridCell(x, z);
                var single = Instantiate(visualPrefab, cell.GetWorldPosition(), Quaternion.identity);
                single.transform.parent = transform;
                singles[x,z] = single;
            }
        }
    }

    private void OnEnable()
    {
        Unit.OnAnyUnitMove += UpdateGridVisual;
        UnitActionSystem.UpdateUI += UpdateGridVisual;
        MouseWorld.LeaveGridCell += MouseLeave;
        MouseWorld.EnterGridCell += MouseEnter;
        MouseWorld.EnterCellAOE += MouseEnterAOE;
        MouseWorld.EnterCellLine += MouseEnterLine;
    }

    private void OnDisable()
    {
        Unit.OnAnyUnitMove -= UpdateGridVisual;
        UnitActionSystem.UpdateUI -= UpdateGridVisual;
        MouseWorld.LeaveGridCell -= MouseLeave;
        MouseWorld.EnterGridCell -= MouseEnter;
        MouseWorld.EnterCellAOE -= MouseEnterAOE;
        MouseWorld.EnterCellLine -= MouseEnterLine;
    }

    private void Start()
    {
        UpdateGridVisual();
    }
    #endregion
    
    #region //Update visuals
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
        if(currentAction == null) return;
        bool useTargeted = false;

        //Showing range of targeted actions
        if(isTargetedAction)
        {
            var cells = targetedAction.GetRangeCells(currentUnit.GetGridCell());
            ShowCells(cells, GetGridMaterial(currentAction, false)); 
            useTargeted = true;
        }

        //Showing cells that can be selected for the current action
        currentCells = currentAction.GetValidCells();
        ShowCells(currentCells, GetGridMaterial(currentAction, useTargeted));
    }

    private void HideAll()
    {
        currentCells = new List<GridCell>();
        foreach(var single in singles)
            single.Hide();
    }
    #endregion

    #region //Mouse effects
    private void MouseEnter(GridCell gridCell)
    {
        //Show center point
        if(currentAction == null) activeMouseMaterial = defaultMouseMaterial;
        else if(!GetSingle(gridCell).IsShowing()) activeMouseMaterial = defaultMouseMaterial;
        else activeMouseMaterial = GetMouseMaterial(currentAction, isTargetedAction && currentCells.Contains(gridCell));
        ShowCell(gridCell, activeMouseMaterial, false);
    }

    private void MouseEnterAOE(GridCell centerPoint, int aoeSize, bool targetOnly)
    {
        if(targetOnly && activeMouseMaterial == defaultMouseMaterial) return;

        //Only show AOE if the mouse is on a valid cell
        if(levelGrid.HasObstacle(centerPoint)) return;
        foreach(var cell in levelGrid.CheckGridRange(centerPoint, aoeSize))
        {
            Vector3 dir = (cell - centerPoint).GetWorldPosition();
            if(Physics.Raycast(centerPoint.GetWorldPosition(), dir, dir.magnitude, GridGlobals.obstacleMask)) continue;
            ShowCell(cell, activeMouseMaterial, false);
        }
    }

    private void MouseEnterLine(GridCell origin)
    {
        if(activeMouseMaterial == defaultMouseMaterial) return;

        foreach(var cell in levelGrid.GetLine(currentUnit.GetGridCell(), origin))
            ShowCell(cell, activeMouseMaterial, false);
    }

    private void MouseLeave(GridCell centerPoint)
    {
        foreach(var cell in levelGrid.GetAllCells())
            GetSingle(cell).Restore();

    }
    #endregion

    #region //Showing cells
    private void ShowCells(List<GridCell> cells, Material material, bool store = true)
    {
        foreach(var cell in cells)
            ShowCell(cell, material, store);
    }

    private void ShowCell(GridCell cell, Material material, bool store = true)
    {
        GetSingle(cell).Show(material, store);
    }
    #endregion

    #region //Getters
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
    private Material GetMouseMaterial(BaseAction action, bool useTargeted)
    {
        foreach(var gridMaterial in gridMaterials)
        {
            if(!gridMaterial.actionNames.Contains(action.GetType().ToString())) continue;
            if(useTargeted)
            {
                var tar = (TargetedVisualType) gridMaterial;
                return tar.targetedMouseMaterial;
            }
            else return gridMaterial.mouseMaterial;
        }
        return defaultMouseMaterial;
    }
    private GridSystemVisualSingle GetSingle(GridCell cell) => singles[cell.x, cell.z];
    #endregion
}