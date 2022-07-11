using System;
using UnityEngine;

/// <summary>
/// Tracks the mouse location in the game space
/// </summary>
public class MouseWorld : MonoBehaviour
{
    private bool isAcive = true;

    #region //Grid
    [SerializeField] private LayerMask mousePlaneMask = -1;
    private InputManager inputManager = null;
    private LevelGrid levelGrid = null;
    private GridCell currentGridCell = new GridCell();
    #endregion

    #region //Mouse effects
    private int aoeSize = 0; //Number of highlighted grid cells surrounding the mouse cursor
    private bool targetOnly = false; //Mouse aoe works on target cells only
    private bool lineMode = false; //Create line off of mouse point
    public static event Action<GridCell> LeaveGridCell;
    public static event Action<GridCell> EnterGridCell;
    public static event Action<GridCell, int, bool> EnterCellAOE;
    public static event Action<GridCell> EnterCellLine;
    #endregion


    #region //Monobehaviour
    private void Awake()
    {
        inputManager = FindObjectOfType<InputManager>();
        levelGrid = FindObjectOfType<LevelGrid>();
    }

    private void OnEnable()
    {
        Pause.OnPause += OnPause;
        UnitManager.GameOver += GameOver;
    }

    private void OnDisable()
    {
        Pause.OnPause -= OnPause;
        UnitManager.GameOver -= GameOver;
    }

    private void Update()
    {
        if(!isAcive) return;
        transform.position = GetWorldPosition();
        var newCell = transform.position.GetGridCell();

        if(newCell == currentGridCell) return;

        if(levelGrid.IsValidCell(currentGridCell)) 
            LeaveGridCell?.Invoke(currentGridCell);

        currentGridCell = newCell;

        if(levelGrid.IsValidCell(newCell))
            MouseEnterCell();
    }
    #endregion

    #region //Active
    private void GameOver() => OnPause(true);
    private void OnPause(bool pause) => isAcive = !pause;
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
    #endregion

    #region //Mouse effects
    public void SetLineMode(bool set) => lineMode = set;
    public void SetAOESize(int aoeSize, bool targetOnly)
    {
        this.aoeSize = aoeSize;
        this.targetOnly = targetOnly;
        RefreshMouse();
    }

    public void ResetAOE()
    {
        aoeSize = 0;
        targetOnly = false;
    }

    //Re-applies mouse positioning. Used after UI refreshes
    public void RefreshMouse()
    {
        if (!levelGrid.IsValidCell(currentGridCell)) return;
        LeaveGridCell?.Invoke(currentGridCell);
        MouseEnterCell();
    }

    public void MouseEnterCell()
    {
        EnterGridCell?.Invoke(currentGridCell);
        if(aoeSize > 0) EnterCellAOE?.Invoke(currentGridCell, aoeSize, targetOnly);
        if(lineMode) EnterCellLine?.Invoke(currentGridCell);
    }
    #endregion
}