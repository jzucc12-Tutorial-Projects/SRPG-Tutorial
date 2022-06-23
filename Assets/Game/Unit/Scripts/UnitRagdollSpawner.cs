using System;
using UnityEngine;

public class UnitRagdollSpawner : MonoBehaviour
{
    [SerializeField] private UnitRagdoll ragdollPrefab = null;
    [SerializeField] private Transform originalRootbone = null;
    private HealthSystem healthSystem = null;


    private void Awake()
    {
        healthSystem = GetComponent<HealthSystem>();
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
