using System;
using UnityEngine;

/// <summary>
/// Keeps track of unit health and death
/// </summary>
public class UnitHealth : MonoBehaviour
{
    #region //Variables
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int aiMaxHPBoost = 50;
    [SerializeField] private int currentHealth;
    public event Action OnDeath;
    public event Action<float> OnHPChange;
    public event Action OnDamage;
    #endregion


    #region //Monobehaviour
    private void Awake()
    {
        currentHealth = maxHealth;
    }
    #endregion

    #region //Damage and healing
    public int Damage(int damage)
    {
        int damageAmount = Mathf.Min(damage, currentHealth);
        currentHealth -= damage;
        OnHPChange?.Invoke(GetHealthPercentage());
        if(damage > 0) OnDamage?.Invoke();
        return damageAmount;
    }

    public int Heal(int amount)
    {
        int healAmount = Mathf.Min(amount, maxHealth - currentHealth);
        currentHealth += healAmount;
        OnHPChange?.Invoke(GetHealthPercentage());
        return healAmount;
    }

    public bool DeathCheck()
    {
        if(currentHealth > 0) return false;
        Die();
        return true;
    }

    private void Die()
    {
        OnDeath?.Invoke();
    }

    public void AIMaxHPBoost()
    {
        maxHealth += aiMaxHPBoost;
        currentHealth = maxHealth;
        OnHPChange?.Invoke(GetHealthPercentage());
    }
    #endregion

    #region //Getters
    public float GetHealthPercentage() => (float)currentHealth / maxHealth;
    public int GetHealth() => currentHealth;
    public float GetPercentageFromValue(int value) => (float)value / maxHealth;
    #endregion
}
