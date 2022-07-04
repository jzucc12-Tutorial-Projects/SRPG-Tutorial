using System;
using UnityEngine;

public class DestructableObject : MonoBehaviour, ITargetable
{
    #region //Variables
    public static event Action<GridPosition> DestroyedStatic;
    [SerializeField] private string objectName = "";
    [SerializeField] private Transform destroyedCratePrefab = null;
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private Collider myCollider = null;
    private int currentHealth = 0;
    #endregion


    #region //Monobehaviour
    private void Awake()
    {
        currentHealth = maxHealth;
    }

    private void Start()
    {
        LevelGrid.instance.SetTargetableAtGridPosition(GetGridPosition(), this);
    }
    #endregion

    #region //Damaging and destruction
    public void Damage(int damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0) ObjectDestroyed();
    }

    private void ObjectDestroyed()
    {
        LevelGrid.instance.SetTargetableAtGridPosition(GetGridPosition(), null);
        DestroyedStatic?.Invoke(LevelGrid.instance.GetGridPosition(transform.position));
        var destroyTransform = Instantiate(destroyedCratePrefab, transform.position, transform.rotation);
        Vector3 randomDir = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f));
        destroyTransform.ApplyExplosionToRBChildren(150f, transform.position + randomDir, 10f);
        Destroy(gameObject);
    }
    #endregion

    #region //Getters
    public GridPosition GetGridPosition()
    {
        return LevelGrid.instance.GetGridPosition(transform.position);
    }

    public Vector3 GetWorldPosition() => transform.position;

    public bool CanBeTargeted(Unit attackingUnit, bool isHealing) => !isHealing;
    public Vector3 GetTargetPosition()
    {
        return transform.position + Vector3.up * myCollider.bounds.size.y/2;
    }
    public string GetTargetName() => objectName;
    #endregion
}