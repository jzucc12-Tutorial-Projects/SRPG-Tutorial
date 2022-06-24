using System;
using UnityEngine;

public class DestructableObject : MonoBehaviour
{
    public static event Action<GridPosition> DestroyedStatic;
    [SerializeField] private Transform destroyedCratePrefab = null;


    public void Damage()
    {
        DestroyedStatic?.Invoke(LevelGrid.instance.GetGridPosition(transform.position));
        var destroyTransform = Instantiate(destroyedCratePrefab, transform.position, transform.rotation);
        Vector3 randomDir = new Vector3(UnityEngine.Random.Range(-1f, 1f), 0, UnityEngine.Random.Range(-1f, 1f));
        destroyTransform.ApplyExplosionToRBChildren(150f, transform.position + randomDir, 10f);
        Destroy(gameObject);
    }
}
