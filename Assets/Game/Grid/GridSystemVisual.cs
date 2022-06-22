using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    #region //Variables
    public static GridSystemVisual instance { get; private set; }
    [SerializeField] private GridSystemVisualSingle visualPrefab = null;
    private GridSystemVisualSingle[,] singles;
    #endregion


    #region //Monobehaviour
    private void Awake()
    {
        if(instance == null) instance = this;
        else Destroy(gameObject);
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
                singles[x,z] = single;
            }
        }
    }

    private void Update()
    {
        UpdateGridVisual();
    }
    #endregion
    
    #region //Showing visuals
    private void HideAll()
    {
        foreach(var single in singles)
            single.Hide();
    }

    private void ShowPositions(List<GridPosition> positions)
    {
        foreach(var position in positions)
            singles[position.x, position.z].Show();
    }

    private void UpdateGridVisual()
    {
        HideAll();
        var action = UnitActionSystem.instance.GetSelectedAction();
        ShowPositions(action.GetValidPositions());
    }
    #endregion
}
