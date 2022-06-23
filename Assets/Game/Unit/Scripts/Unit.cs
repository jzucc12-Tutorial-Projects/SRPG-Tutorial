using System;
using UnityEngine;

public class Unit : MonoBehaviour
{
    #region //Unit base variables
    [SerializeField] private bool isEnemy = false;
    private GridPosition gridPosition;
    private HealthSystem healthSystem = null;
    public static event Action<Unit> UnitSpawned;
    public static event Action<Unit> UnitDead;
    #endregion

    #region //Action variables
    private BaseAction[] actions = new BaseAction[0];
    [SerializeField] private int maxActionPoints = 2;
    public event Action OnActionPointChange;
    private int currentActionPoints = 0;
    #endregion


    #region //Monobehaviour
    private void Awake()
    {
        actions = GetComponents<BaseAction>();
        healthSystem = GetComponent<HealthSystem>();
    }

    private void OnEnable()
    {
        TurnSystem.instance.IncrementTurn += RestoreActionPoints;
        healthSystem.OnDeath += OnDeath;
    }

    private void OnDisable()
    {
        TurnSystem.instance.IncrementTurn -= RestoreActionPoints;
        healthSystem.OnDeath -= OnDeath;
    }

    private void Start()
    {
        UnitSpawned?.Invoke(this);
        gridPosition = LevelGrid.instance.GetGridPosition(transform.position);
        LevelGrid.instance.AddUnitAtGridPosition(gridPosition, this);
    }

    private void Update()
    {
        GridPosition newPosition = LevelGrid.instance.GetGridPosition(transform.position);
        if(newPosition == gridPosition) return;
        var oldPosition = gridPosition;
        gridPosition = newPosition;
        LevelGrid.instance.UnitMovedGridPosition(oldPosition, newPosition, this);
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

    public bool CanTakeAction(BaseAction action)
    {
        return currentActionPoints >= action.GetPointCost();
    }

    private void SpendActionPoints(int amount)
    {
        currentActionPoints -= amount;
        OnActionPointChange?.Invoke();
    }

    private void RestoreActionPoints()
    {
        if(TurnSystem.instance.IsPlayerTurn() ^ !isEnemy) return;
        currentActionPoints = maxActionPoints;
        OnActionPointChange?.Invoke();
    }
    #endregion

    #region //Health and damage
    public void Damage(int damage)
    {
        healthSystem.Damage(damage);
    }

    private void OnDeath()
    {
        UnitDead?.Invoke(this);
        LevelGrid.instance.RemoveUnitAtGridPosition(gridPosition, this);
        Destroy(gameObject);
    }
    #endregion

    #region //Getters
    public T GetAction<T>() where T : BaseAction
    {
        foreach(var action in actions)
            if(action is T) return (T)action;

        return null;
    }
    public float GetHealthPercentage() => healthSystem.GetHealthPercentage();
    public int GetActionPoints() => currentActionPoints;
    public GridPosition GetGridPosition() => gridPosition;
    public Vector3 GetWorldPosition() => transform.position;
    public BaseAction[] GetActions() => actions;
    public bool IsEnemy() => isEnemy;
    #endregion
}