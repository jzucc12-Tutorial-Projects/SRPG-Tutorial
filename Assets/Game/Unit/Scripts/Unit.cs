using System;
using UnityEngine;

/// <summary>
/// Units used in combat
/// </summary>
public class Unit : MonoBehaviour, ITargetable
{
    #region //Unit properties
    [SerializeField] private string unitName = "Unit";
    [SerializeField] private bool isEnemy = false;
    [SerializeField] private int priority = 0;
    #endregion

    #region //Health
    private UnitHealth unitHealth = null;
    public static event Action<Unit> UnitSpawned;
    public static event Action<Unit> UnitDead;
    public static event Action OnAnyUnitMove;
    #endregion

    #region //Positional
    [SerializeField] private float rotateSpeed = 5;
    [SerializeField] private float shoulderHeight = 1.7f;
    private GridCell gridCell;
    #endregion

    #region //Action
    [SerializeField] private int maxActionPoints = 2;
    [Tooltip("HP% loss increment that drops accuracy")] [SerializeField, MinMax(0, 1)] private float hpLossToDropAccuracy = 0.1f;
    [Tooltip("Accuracy drop with above HP loss.")] [SerializeField, MinMax(0, 100)] private int accuracyDropWithHP = 10;
    private BaseAction[] actions = new BaseAction[0];
    public event Action OnActionPointChange;
    private int currentActionPoints = 0;
    private int accuracyMod = 0; //Should only apply to attacks that rely on accuracy
    private float damageMod = 0; //Should only applies to attacks that rely on accuracy
    #endregion


    #region //Monobehaviour
    private void Awake()
    {
        currentActionPoints = maxActionPoints;
        actions = GetComponents<BaseAction>();
        unitHealth = GetComponent<UnitHealth>();
    }

    private void OnEnable()
    {
        UnitSpawned?.Invoke(this);
        TurnSystem.IncrementTurn += RestoreUnit;
        unitHealth.OnDeath += OnDeath;
    }

    private void OnDisable()
    {
        TurnSystem.IncrementTurn -= RestoreUnit;
        unitHealth.OnDeath -= OnDeath;
    }

    private void Start()
    {
        gridCell = GetGridCell();
    }

    private void Update()
    {
        GridCell newCell = GetGridCell();
        if(newCell == gridCell) return;
        gridCell = newCell;
        OnAnyUnitMove?.Invoke();
    }
    #endregion

    #region //Health
    public void Damage(int damage) => unitHealth.Damage(damage);
    public int Heal(int amount) => unitHealth.Heal(amount); //Returns amount of health healed
    private void OnDeath()
    {
        ActionLogListener.Publish($"{GetName()} has died");
        Destroy(gameObject);
        UnitDead?.Invoke(this);
    }
    #endregion

    #region //Positional
    //Returns true if the unit is still rotating
    public bool Rotate(Vector3 targetDirection)
    {
        transform.forward = Vector3.RotateTowards(transform.forward, targetDirection, rotateSpeed * Time.deltaTime, 0);
        return transform.forward != targetDirection;
    }
    #endregion

    #region //Action
    public bool TryTakeAction(BaseAction action, GridCell targetCell)
    {
        if(action.CanTakeAction(targetCell))
        {
            SpendActionPoints(action.GetAPCost());
            return true;
        }
        return false;
    }

    private void SpendActionPoints(int amount)
    {
        currentActionPoints -= amount;
        OnActionPointChange?.Invoke();
    }

    public void AddAccuracyMod(int amount) => accuracyMod += amount;
    public void AddDamageMod(float amount) => damageMod += amount;

    //Resets action points and unit modifiers at the end of a turn
    private void RestoreUnit(bool isPlayerTurn)
    {
        if(isPlayerTurn ^ !isEnemy) return;
        currentActionPoints = maxActionPoints;
        accuracyMod = 0;
        damageMod = 0;
        OnActionPointChange?.Invoke();
    }
    #endregion

    #region //Getters
    public int GetPriority() => priority;
    public string GetName() => unitName;
    public bool IsEnemy() => isEnemy;
    public float GetHealthPercentage() => unitHealth.GetHealthPercentage();
    public int GetHealth() => unitHealth.GetHealth();
    public bool IsAlive() => unitHealth.GetHealthPercentage() > 0;
    public GridCell GetGridCell() => transform.position.GetGridCell();
    public Vector3 GetWorldPosition() => ConvertToShoulderHeight(transform.position);
    public Vector3 ConvertToShoulderHeight(GridCell cell) => ConvertToShoulderHeight(cell.GetWorldPosition());
    public Vector3 ConvertToShoulderHeight(Vector3 position) => position + Vector3.up * shoulderHeight;
    public BaseAction GetRootAction()
    {
        foreach(var action in actions)
        {
            if(!action.CanSelectAction()) continue;
            return action;
        }
        return null;
    }
    public int GetAP() => currentActionPoints;
    public BaseAction[] GetActions() => actions;
    public bool CanBeTargeted(Unit attackingUnit, bool isHealing)
    {
        bool differentFromTarget = isEnemy ^ attackingUnit.isEnemy;

        if(isHealing)
            return !differentFromTarget && GetHealthPercentage() < 1;
        else
            return differentFromTarget;
    }
    public int GetAccuracyMod() //Combines native accuracy modifier with drop from hp loss
    {
        float hpMissing = 1 - unitHealth.GetHealthPercentage();
        int ticks = Mathf.RoundToInt(hpMissing / hpLossToDropAccuracy);
        return -ticks * accuracyDropWithHP + accuracyMod;
    }
    public float GetDamageMod() => 1 + damageMod;
    #endregion
}