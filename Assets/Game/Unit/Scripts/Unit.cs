using System;
using JZ.AUDIO;
using UnityEngine;

/// <summary>
/// Units used in combat
/// </summary>
public class Unit : MonoBehaviour, ITargetable
{
    #region //Unit properties
    [SerializeField] private string unitName = "Unit";
    [SerializeField] private bool isTeam1 = true;
    [SerializeField] private int priority = 0;
    [SerializeField] private SoundPlayer sfxPlayer = null;
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
    private bool holdingMove = false;
    #endregion

    #region //Action
    [SerializeField] private int maxActionPoints = 2;
    private BaseAction[] actions = new BaseAction[0];
    public event Action OnActionPointChange;
    [SerializeField] private int currentActionPoints = 0;
    private int accuracyMod = 0; //Should only apply to attacks that rely on accuracy
    private float damageMod = 0; //Should only applies to attacks that rely on accuracy
    #endregion


    #region //Monobehaviour
    private void OnValidate()
    {
        if(!string.IsNullOrEmpty(unitName))
            gameObject.name = unitName;
    }

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
        if(IsAI()) unitHealth.AIMaxHPBoost();
        gridCell = GetGridCell();
    }

    private void Update()
    {
        if(Time.timeScale == 0 && sfxPlayer.IsSoundPlaying("walking"))
        {
            sfxPlayer.Stop("walking");
            holdingMove = true;
        }
        else if(Time.timeScale == 1 && holdingMove)
        {
            sfxPlayer.Play("walking");
            holdingMove = false;
        }

        GridCell newCell = GetGridCell();
        if(newCell == gridCell) return;
        gridCell = newCell;
        OnAnyUnitMove?.Invoke();
    }
    #endregion

    #region //Health
    public int Damage(Unit attacker, int damage, bool crit) 
    {
        int damageAmount = unitHealth.Damage(damage);
        if(damageAmount > 0) 
        {
            string hitText = crit ? "crit" : "hit";
            if(attacker == this)
                ActionLogListener.Publish($"{attacker.GetName()} {hitText} themselves for {damageAmount} damage");
            else
                ActionLogListener.Publish($"{attacker.GetName()} {hitText} {GetName()} for {damageAmount} damage");
        
            if(!unitHealth.DeathCheck())
                sfxPlayer.Play(hitText);
        }
        return damageAmount;
    }
    public int Heal(Unit healer, int amount)
    {
        int healAmount = unitHealth.Heal(amount);
        if(healAmount > 0) 
        {
            if(healer == this)
                ActionLogListener.Publish($"{healer.GetName()} healed themselves for {healAmount} hp");
            else
                ActionLogListener.Publish($"{healer.GetName()} healed {GetName()} for {healAmount} hp");
        }

        return healAmount;
    }

    private void OnDeath()
    {
        ActionLogListener.Publish($"{GetName()} has died");
        sfxPlayer.transform.parent = null;
        sfxPlayer.PlayLastSound("death");
        gameObject.SetActive(false);
        UnitDead?.Invoke(this);
    }
    #endregion

    #region //Positional
    //Returns true if the unit is still rotating
    public bool Rotate(Vector3 targetDirection)
    {
        //The unit can get stuck if the rotation is close to 180. Give a push if that's the case
        var angle = Vector3.SignedAngle(transform.forward, targetDirection, transform.up);
        if(Mathf.Abs(angle) >= 177)
            transform.Rotate(0, Mathf.Sign(angle) * rotateSpeed * Time.deltaTime, 0);
        else
            transform.forward = Vector3.Lerp(transform.forward, targetDirection, rotateSpeed * Time.deltaTime);
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
    private void RestoreUnit(bool team1Turn)
    {
        if(team1Turn ^ IsTeam1()) return;
        currentActionPoints = maxActionPoints;
        accuracyMod = 0;
        damageMod = 0;
        OnActionPointChange?.Invoke();
    }
    #endregion

    #region //Getters
    public int GetPriority() => priority;
    public string GetName() => unitName;
    public bool IsTeam1() => isTeam1;
    public bool IsAI() => GameGlobals.IsAI(isTeam1);
    public float GetHealthPercentage() => unitHealth.GetHealthPercentage();
    public int GetHealth() => unitHealth.GetHealth();
    public float GetTestHPPercent(int value) => unitHealth.GetPercentageFromValue(value);
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
    public T GetAction<T>() where T : BaseAction
    {
        foreach(var action in actions)
            if(action is T) return action as T;
            
        return null;
    }
    public bool CanBeTargeted(Unit attackingUnit, bool isHealing)
    {
        bool differentFromTarget = IsTeam1() ^ attackingUnit.IsTeam1();

        if(isHealing)
            return !differentFromTarget && GetHealthPercentage() < 1;
        else
            return differentFromTarget;
    }
    public int GetAccuracyMod() => accuracyMod;
    public float GetDamageMod() => 1 + damageMod;
    public void PlaySound(string clipName) => sfxPlayer.Play(clipName);
    public void StopSound(string clipName) => sfxPlayer.Stop(clipName);
    #endregion
}