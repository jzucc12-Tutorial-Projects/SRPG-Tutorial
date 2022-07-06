using UnityEngine;

/// <summary>
/// Ragdoll for a given unit. Typically is spawned at unit death.
/// </summary>
public class UnitRagdoll : MonoBehaviour
{
    [SerializeField] private Transform ragdollRootBone = null;


    public void SetUp(Transform rootBone)
    {
        MatchAllChildTransforms(rootBone, ragdollRootBone);
        Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        ragdollRootBone.ApplyExplosionToRBChildren(300f, transform.position + randomDir, 10f);
    }

    private void MatchAllChildTransforms(Transform root, Transform clone)
    {
        foreach(Transform child in root)
        {
            Transform cloneChild = clone.Find(child.name);
            if(cloneChild != null)
            {
                cloneChild.position = child.position;
                cloneChild.rotation = child.rotation;
                MatchAllChildTransforms(child, cloneChild);
            }
        }
    }
}