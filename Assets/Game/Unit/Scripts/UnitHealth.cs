using System;
using UnityEngine;

public class UnitHealth : MonoBehaviour
{
    #region //Variables
    [SerializeField] private int maxHealth = 100;
    private int currentHealth;
    public event Action OnDeath;
    public event Action<float> OnDamage;
    private Unit unit = null;
    #endregion


    #region //Monobehaviour
    private void Awake()
    {
        currentHealth = maxHealth;
        unit = GetComponent<Unit>();
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
