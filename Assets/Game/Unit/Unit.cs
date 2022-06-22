using UnityEngine;

public class Unit : MonoBehaviour
{
    #region //Position variables
    private GridPosition gridPosition;
    #endregion

    #region //Action variables
    public MoveAction moveAction {  get; private set; }
    public SpinAction spinAction {  get; private set; }
    private BaseAction[] actions = new BaseAction[0];
    [SerializeField] private int maxActionPoints = 2;
    private int currentActionPoints = 0;
    #endregion


    #region //Monobehaviour
    private void Awake()
    {
        moveAction = GetComponent<MoveAction>();
        spinAction = GetComponent<SpinAction>();
        actions = GetComponents<BaseAction>();
    }

    private void OnEnable()
    {
        TurnSystem.instance.IncrementTurn += RestoreActionPoints;
    }

    private void OnDisable()
    {
        TurnSystem.instance.IncrementTurn -= RestoreActionPoints;
    }

    private void Start()
    {
        gridPosition = LevelGrid.instance.GetGridPosition(transform.position);
        LevelGrid.instance.AddUnitAtGridPosition(gridPosition, this);
    }

    private void Update()
    {
        GridPosition newPosition = LevelGrid.instance.GetGridPosition(transform.position);
        if(newPosition == gridPosition) return;
        LevelGrid.instance.UnitMovedGridPosition(gridPosition, newPosition, this);
        gridPosition = newPosition;
    }
    #endregion

    #region //Action
    public bool TryTakeAction(BaseAction action)
    {
        if(CanTakeAction(action))
        {
            SpendActionPoints(action.GetPointCost());
            return true;
        }
        return false;
    }

    private bool CanTakeAction(BaseAction action)
    {
        return currentActionPoints >= action.GetPointCost();
    }

    private void SpendActionPoints(int amount)
    {
        currentActionPoints -= amount;
    }

    private void RestoreActionPoints()
    {
        currentActionPoints = maxActionPoints;
    }
    #endregion

    #region //Getters
    public int GetActionPoints() => currentActionPoints;
    public GridPosition GetGridPosition() => gridPosition;
    public BaseAction[] GetActions() => actions;
    #endregion
}