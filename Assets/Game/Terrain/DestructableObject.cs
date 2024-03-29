using JZ.AUDIO;
using UnityEngine;

/// <summary>
/// Objects that can be destroyed in combat
/// </summary>
public class DestructableObject : MonoBehaviour, ITargetable
{
    #region //Variables
    [SerializeField] private GameObject rootObject = null;
    [SerializeField] private string objectName = "";
    [SerializeField] private Transform destroyedObject = null;
    [SerializeField] private Transform aimPoint = null;
    [SerializeField] private int maxHealth = 1;
    [SerializeField] private SoundPlayer sfxPlayer = null;
    private int currentHealth = 0;
    bool destroyed = false;
    #endregion


    #region //Monobehaviour
    private void Awake()
    {
        currentHealth = maxHealth;
    }
    #endregion

    #region //Damaging and destruction
    public int Damage(Unit attacker, int damage, bool crit)
    {
        if(destroyed) return 0;
        int damageDealt = Mathf.Min(damage, currentHealth);
        currentHealth -= damageDealt;
        if(currentHealth <= 0) 
        {
            ActionLogListener.Publish($"{attacker.GetName()} destroyed {GetName()}");
            ObjectDestroyed();
        }
        return damageDealt;
    }

    private void ObjectDestroyed()
    {
        destroyed = true;
        sfxPlayer.PlayLastSound("destroyed");
        destroyedObject.gameObject.SetActive(true);
        Vector3 randomDir = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f));
        destroyedObject.ApplyExplosionToRBChildren(150f, destroyedObject.position + randomDir, 10f);
        destroyedObject.transform.parent = null;
        rootObject.SetActive(false);
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