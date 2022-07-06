using UnityEngine;

/// <summary>
/// Objects that can be destroyed in combat
/// </summary>
public class DestructableObject : MonoBehaviour, ITargetable
{
    #region //Variables
    [SerializeField] private string objectName = "";
    [SerializeField] private Transform destroyedCratePrefab = null;
    [SerializeField] private int maxHealth = 1;
    private int currentHealth = 0;
    #endregion


    #region //Monobehaviour
    private void Awake()
    {
        currentHealth = maxHealth;
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
        var destroyTransform = Instantiate(destroyedCratePrefab, transform.position, transform.rotation);
        Vector3 randomDir = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f));
        destroyTransform.ApplyExplosionToRBChildren(150f, transform.position + randomDir, 10f);
        Destroy(gameObject);
    }
    #endregion

    #region //Getters
    public GridCell GetGridCell()
    {
        return transform.position.GetGridCell();
    }
    public Vector3 GetWorldPosition() => transform.position;
    public bool CanBeTargeted(Unit attackingUnit, bool isHealing) => !isHealing;
    public string GetName() => objectName;
    #endregion
}