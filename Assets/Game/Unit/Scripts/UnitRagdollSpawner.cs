using UnityEngine;

/// <summary>
/// Creates a ragdoll for a given unit. Typically at unit death.
/// </summary>
public class UnitRagdollSpawner : MonoBehaviour
{
    [SerializeField] private UnitRagdoll ragdollPrefab = null;
    [SerializeField] private Transform originalRootbone = null;
    private UnitHealth healthSystem = null;


    private void Awake()
    {
        healthSystem = GetComponent<UnitHealth>();
    }

    private void OnEnable()
    {
        healthSystem.OnDeath += SpawnRagdoll;
    }

    private void Disable()
    {
        healthSystem.OnDeath -= SpawnRagdoll;
    }

    private void SpawnRagdoll()
    {
        var ragdoll = Instantiate(ragdollPrefab, transform.position, transform.rotation);
        ragdoll.SetUp(originalRootbone);
    }
}
