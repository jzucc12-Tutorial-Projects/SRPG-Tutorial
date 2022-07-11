using System;
using UnityEngine;

/// <summary>
/// Detects collisions and deals damage to targets in a lightning bolt
/// </summary>
public class LightningBoltCollider : MonoBehaviour
{
    [SerializeField] Rigidbody rb = null;
    [SerializeField] private LayerMask damageLayer = 0;
    private Action<GameObject> damageAction = null;
    private Vector3 startPoint = Vector3.zero;
    private float wallDistance = float.MaxValue;


    private void OnTriggerEnter(Collider other) 
    {
        if(GridGlobals.obstacleMask.HasLayer(other.gameObject.layer))
        {
            rb.velocity = Vector3.zero;
            wallDistance = (other.transform.position - startPoint).magnitude;
        }  

        if(damageLayer.HasLayer(other.gameObject.layer))
        {
            if(wallDistance < (other.transform.position - startPoint).magnitude) return;
            damageAction?.Invoke(other.gameObject);
        }
    }

    public void Setup(Vector3 startPoint, Vector3 velocity, Action<GameObject> damageAction)
    {
        wallDistance = float.MaxValue;
        transform.position = new Vector3(startPoint.x, transform.position.y, startPoint.z);
        this.startPoint = transform.position;
        rb.velocity = velocity;
        this.damageAction = damageAction;
    }
}
