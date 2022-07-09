using UnityEngine;

public static class JZTransformExtensions
{
    public static void ApplyExplosionToRBChildren(this Transform root, float explosionForce, Vector3 explosionPosition, float explosionRange)
    {
        foreach(Transform child in root)
        {
            if(child.TryGetComponent<Rigidbody>(out Rigidbody childRigidbody))
            {
                childRigidbody.AddExplosionForce(explosionForce, explosionPosition, explosionRange);
            }
            ApplyExplosionToRBChildren(child, explosionForce, explosionPosition, explosionRange);
        }
    }

    public static Transform FindDeepChild(this Transform refTransform, string childName)
    {
        Transform result = refTransform.Find(childName);
        if(result != null) return result;

        foreach(Transform child in refTransform)
        {
            result = child.FindDeepChild(childName);
            if(result != null) return result;
        }

        return null;
    }
}

