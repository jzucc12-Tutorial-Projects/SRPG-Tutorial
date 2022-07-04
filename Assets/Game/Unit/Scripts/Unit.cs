using System;
using UnityEngine;

public class Unit : MonoBehaviour, ITargetable
{
    #region //Unit base variables
    [Header("Base variables")]
    [SerializeField] private string unitName = "Unit";
    [SerializeField] private bool isEnemy = false;
    [SerializeField] private float rotateSpeed = 5;
    [SerializeField] private int priority = 0;
    private GridPosition gridPosition;
    private UnitHealth unitHealth = null;
    public static event Action<Unit> UnitSpawned;
    public static event Action<Unit> UnitDead;
    #endregion

    #region //Action variables
    private BaseAction[] actions = new BaseAction[0];
    [SerializeField] private int maxActionPoints = 2;
    public event Action OnActionPointChange;
    private int currentActionPoints = 0;
    #endregion

    #region //Weapons and attacks
    [Header("Weapons and attacks")]
    [SerializeField] private GameObject defaultWeapon = null;
    private GameObject activeWeapon = null;
    public event Action<AnimatorOverrideController> OnWeaponSwap;
    [Tooltip("HP% loss increment that drops accuracy")] [SerializeField, MinMax(0, 1)] private float hpLossToDropAccuracy = 0.1f;
    [Tooltip("Accuracy drop with above HP loss.")] [SerializeField, MinMax(0, 100)] private int accuracyDropWithHP = 10;
    private int accuracyMod = 0;
    private float damageMod = 0;
    #endregion


    #region //Monobehaviour
    private void Awake()
    {
        currentActionPoints = maxActionPoints;
        actions = GetComponents<BaseAction>();
        unitHealth = GetComponent<UnitHealth>();
        activeWeapon = defaultWeapon;
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
        gridPosition = GetGridPosition();
        LevelGrid.instance.AddUnitAtGridPosition(gridPosition, this);
        LevelGrid.instance.SetTargetableAtGridPosition(GetGridPosition(), this);
    }

    private void Update()
    {
        GridPosition newPosition = GetGridPosition();
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

    public void AddAccuracyMod(int amount) => accuracyMod += amount;
    public void AddDamageMod(float amount) => damageMod += amount;

    private void RestoreUnit(bool isPlayerTurn)
    {
        if(isPlayerTurn ^ !isEnemy) return;
        currentActionPoints = maxActionPoints;
        accuracyMod = 0;
        damageMod = 0;
        OnActionPointChange?.Invoke();
    }
    #endregion

    #region //Health and damage
    public void Damage(int damage)
    {
        unitHealth.Damage(damage);
    }

    public int Heal(int amount)
    {
        return unitHealth.Heal(amount);
    }

    private void OnDeath()
    {
        ActionLogListener.Publish($"{GetName()} has died");
        UnitDead?.Invoke(this);
        LevelGrid.instance.SetTargetableAtGridPosition(GetGridPosition(), null);
        LevelGrid.instance.RemoveUnitAtGridPosition(gridPosition, this);
        Destroy(gameObject);
    }
    #endregion

    #region //Active weapons
    public void SetActiveWeapon(GameObject newWeapon, AnimatorOverrideController controller)
    {
        HideActiveWeapon();
        activeWeapon = newWeapon;
        if(controller != null) OnWeaponSwap?.Invoke(controller);
        ShowActiveWeapon();
    }

    public void ShowActiveWeapon()
    {
        activeWeapon.SetActive(true);
    }

    public void HideActiveWeapon()
    {
        activeWeapon.SetActive(false);
    }
    #endregion

    #region //Rotation
    //Returns true if the unit is still rotating
    public bool Rotate(Vector3 targetDirection)
    {
        transform.forward = Vector3.Lerp(transform.forward, targetDirection, rotateSpeed * Time.deltaTime);
        return transform.forward != targetDirection;
    }
    #endregion

    #region //Getters
    public BaseAction GetRootAction() => actions[0];
    public T GetAction<T>() where T : BaseAction
    {
        foreach(var action in actions)
            if(action is T) return (T)action;

        return null;
    }
    public float GetHealthPercentage() => unitHealth.GetHealthPercentage();
    public int GetActionPoints() => currentActionPoints;
    public GridPosition GetGridPosition() => LevelGrid.instance.GetGridPosition(transform.position);
    public Vector3 GetWorldPosition() => transform.position;
    public BaseAction[] GetActions() => actions;
    public bool IsEnemy() => isEnemy;
    public bool CanBeTargeted(Unit attackingUnit, bool isHealing)
    {
        bool differentFromTarget = isEnemy ^ attackingUnit.isEnemy;

        if(isHealing)
            return !differentFromTarget && GetHealthPercentage() < 1;
        else
            return differentFromTarget;
    }
    public Vector3 GetTargetPosition()
    {
        return transform.position + Vector3.up * 1.7f;
    }
    public int GetAccuracyMod()
    {
        float hpMissing = 1 - unitHealth.GetHealthPercentage();
        int ticks = Mathf.RoundToInt(hpMissing / hpLossToDropAccuracy);
        return -ticks * accuracyDropWithHP + accuracyMod;
    }
    public float GetDamageMod() => 1 + damageMod;
    public int GetPriority() => priority;
    public string GetName() => unitName;
    public string GetTargetName() => GetName();
    #endregion
}