using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual instance { get; private set; }

    #region //Grid variables
    [SerializeField] private GridSystemVisualSingle visualPrefab = null;
    private GridSystemVisualSingle[,] singles;
    [System.Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualType visualType;
        public Material material;
    }
    public enum GridVisualType
    {
        White,
        BlueSoft,
        Blue,
        RedSoft,
        Red,
        Yellow
    }
    [SerializeField] private List<GridVisualTypeMaterial> gridMaterials = new List<GridVisualTypeMaterial>();
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
        GridVisualType visualType;

        switch(action)
        {
            default:
            case MoveAction moveAction:
                visualType = GridVisualType.White;
                break;

            case SpinAction spinAction:
                visualType = GridVisualType.Blue;
                break;

            case ShootAction shootAction:
                visualType = GridVisualType.Red;
                var positions = new List<GridPosition>(LevelGrid.instance.CheckGridRange(unit.GetGridPosition(), shootAction.GetRange()));
                ShowPositions(positions, GridVisualType.RedSoft);
                break;

            case MeleeAction meleeAction:
                visualType = GridVisualType.Red;
                var meleePosition = new List<GridPosition>(LevelGrid.instance.CheckGridRange(unit.GetGridPosition(), meleeAction.GetRange(), false));
                ShowPositions(meleePosition, GridVisualType.RedSoft);
                break;

            case InteractAction interactAction:
                visualType = GridVisualType.Blue;
                var interactPosition = new List<GridPosition>(LevelGrid.instance.CheckGridRange(unit.GetGridPosition(), interactAction.GetRange(), false));
                ShowPositions(interactPosition, GridVisualType.BlueSoft);
                break;  
        }

        ShowPositions(action.GetValidPositions(), visualType);
    }

    private void ShowPositions(List<GridPosition> positions, GridVisualType visualType)
    {
        foreach(var position in positions)
            singles[position.x, position.z].Show(GetGridMaterial(visualType));
    }

    private Material GetGridMaterial(GridVisualType visualType)
    {
        foreach(var gridMaterial in gridMaterials)
        {
            if(gridMaterial.visualType != visualType) continue;
            return gridMaterial.material;
        }
        return null;
    }
    #endregion
}
