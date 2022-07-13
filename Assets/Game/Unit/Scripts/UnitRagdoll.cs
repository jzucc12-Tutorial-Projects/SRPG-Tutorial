using UnityEngine;

/// <summary>
/// Ragdoll for a given unit. Typically is spawned at unit death.
/// </summary>
public class UnitRagdoll : MonoBehaviour
{
    [SerializeField] private Transform ragdollRootBone = null;


    public void SetUp(Transform rootBone, GameObject unitWeapon)
    {
        MatchAllChildTransforms(rootBone, ragdollRootBone);
        Vector3 randomDir = new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
        ragdollRootBone.ApplyExplosionToRBChildren(300f, transform.position + randomDir, 10f);
        if(unitWeapon != null) SetUpWeapon(unitWeapon);
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

    private void SetUpWeapon(GameObject unitWeapon)
    {
        var parent = transform.FindDeepChild(unitWeapon.transform.parent.name);
        var weapon = Instantiate(unitWeapon, parent);
        weapon.transform.localPosition = unitWeapon.transform.localPosition;
        weapon.transform.rotation = unitWeapon.transform.rotation;
        weapon.transform.localScale = unitWeapon.transform.localScale;
    }
}