using System;
using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    #region //Variables
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    public event Action OnDeath;
    public event Action<float> OnDamage;
    #endregion


    #region //Monobehaviour
    private void Awake()
    {
        currentHealth = maxHealth;
    }
    #endregion

    #region //Damage and death
    public void Damage(int damage)
    {
        currentHealth -= damage;
        OnDamage?.Invoke(GetHealthPercentage());

        if(currentHealth <= 0)
        {
            currentHealth = 0;
            Die();
        }
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
