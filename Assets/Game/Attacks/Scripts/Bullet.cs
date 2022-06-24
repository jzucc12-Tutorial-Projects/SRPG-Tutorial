using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector3 target = Vector3.zero;
    [SerializeField] private float moveSpeed = 200;
    [SerializeField] private TrailRenderer trail = null;
    [SerializeField] private ParticleSystem particles = null;


    private void Update()
    {
        Vector3 moveDir = (target - transform.position).normalized;
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        if((target - transform.position).InRange(0.1f)) 
        {
            transform.position = target;
            trail.transform.parent = null;
            Instantiate(particles, target, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    public void SetUp(Vector3 target)
    {
        this.target = target;
    }
}
