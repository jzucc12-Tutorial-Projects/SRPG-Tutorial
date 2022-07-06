using System;
using UnityEngine;

/// <summary>
/// Tracks the mouse location in the game space
/// </summary>
public class MouseWorld : MonoBehaviour
{
    #region //Variables
    [SerializeField] private LayerMask mousePlaneMask = -1;
    private InputManager inputManager = null;
    private LevelGrid levelGrid = null;
    private GridCell currentGridCell = new GridCell();
    private int aoeSize = 0; //Number of highlighted grid cells surrounding the mouse cursor
    public static event Action<GridCell, int> LeaveGridCell;
    public static event Action<GridCell, int> EnterGridCell;
    #endregion


    #region //Monobehaviour
    private void Awake()
    {
        inputManager = FindObjectOfType<InputManager>();
        levelGrid = FindObjectOfType<LevelGrid>();
    }

    private void Update()
    {
        transform.position = GetWorldPosition();
        var newCell = transform.position.GetGridCell();

        if(newCell == currentGridCell) return;

        if(levelGrid.IsValidCell(currentGridCell)) 
            LeaveGridCell?.Invoke(currentGridCell, aoeSize);

        currentGridCell = newCell;

        if(levelGrid.IsValidCell(newCell))
            EnterGridCell?.Invoke(newCell, aoeSize);
    }
    #endregion

    #region //Positions
    public Vector3 GetWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(inputManager.GetMouseScreenPosition());
        Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, mousePlaneMask);
        return hit.point;
    }

    public GridCell GetGridCell()
    {
        return GetWorldPosition().GetGridCell();
    }

    public void SetAOESize(int aoeSize)
    {
        this.aoeSize = aoeSize;
        RefreshMouse();
    }

    //Re-applies mouse positioning. Used after UI refreshes
    public void RefreshMouse()
    {
        if (!levelGrid.IsValidCell(currentGridCell)) return;
        LeaveGridCell?.Invoke(currentGridCell, aoeSize);
        EnterGridCell?.Invoke(currentGridCell, aoeSize);
    }
    #endregion
}