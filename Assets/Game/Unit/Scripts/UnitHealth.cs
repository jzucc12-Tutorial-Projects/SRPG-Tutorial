using System;
using UnityEngine;

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
    public void Damage(int damage)
    {
        currentHealth -= damage;
        OnHPChange?.Invoke(GetHealthPercentage());

        if(currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
        else if(damage > 0)
            OnDamage?.Invoke();
    }

    public int Heal(int amount)
    {
        int healAmount = Mathf.Min(maxHealth, currentHealth + amount);
        currentHealth += healAmount;
        OnHPChange?.Invoke(GetHealthPercentage());
        return healAmount;
    }

    private void Die()
    {
        OnDeath?.Invoke();
    }
    #endregion

    #region //Getters
    public float GetHealthPercentage() => (float)currentHealth / maxHealth;
    #endregion
}
