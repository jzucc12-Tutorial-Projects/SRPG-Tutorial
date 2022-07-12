using UnityEngine;

/// <summary>
/// Objects that can be destroyed in combat
/// </summary>
public class DestructableObject : MonoBehaviour, ITargetable
{
    #region //Variables
    [SerializeField] private string objectName = "";
    [SerializeField] private Transform destroyedPrefab = null;
    [SerializeField] private Transform aimPoint = null;
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
    public void Damage(Unit attacker, int damage)
    {
        currentHealth -= damage;
        if(currentHealth <= 0) 
        {
            ActionLogListener.Publish($"{attacker.GetName()} destroyed {GetName()}");
            ObjectDestroyed();
        }
    }

    private void ObjectDestroyed()
    {
        var destroyTransform = Instantiate(destroyedPrefab, transform.position, transform.rotation);
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
    public Vector3 GetWorldPosition() => aimPoint.position;
    public bool CanBeTargeted(Unit attackingUnit, bool isHealing) => !isHealing;
    public string GetName() => objectName;
    #endregion
}