using UnityEngine;

public class Unit : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4;
    [SerializeField] private float rotateSpeed = 4;
    [SerializeField] private float threshold = 0.025f;
    [SerializeField] private Animator animator = null;
    private Vector3 targetPosition;


    private void Awake()
    {
        targetPosition = transform.position;
    }

    private void Update()
    {
        animator.SetBool("isWalking", Move());
    }

    private bool Move()
    {
        if(Mathf.Abs((targetPosition - transform.position).sqrMagnitude) <= threshold * threshold) return false;
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        transform.forward = Vector3.Lerp(transform.forward, moveDirection, rotateSpeed * Time.deltaTime);
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
        return true;
    }

    public void ChangePosition(Vector3 targetPosition)
    {
        this.targetPosition = targetPosition;
    }
}