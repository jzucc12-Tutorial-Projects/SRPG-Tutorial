using System;
using UnityEngine;

/// <summary>
/// Keeps track of unit health and death
/// </summary>
public class UnitHealth : MonoBehaviour
{
    #region //Variables
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
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

    public void DeathCheck()
    {
        if(currentHealth > 0) return;
        Die();
    }

    private void Die()
    {
        OnDeath?.Invoke();
    }
    #endregion

    #region //Getters
    public float GetHealthPercentage() => (float)currentHealth / maxHealth;
    public int GetHealth() => currentHealth;
    #endregion
}
