using UnityEngine;

public class Unit : MonoBehaviour
{
    #region //Cached components
    [SerializeField] private Animator animator = null;
    private GridPosition gridPosition;
    #endregion

    #region //Movement variables
    [SerializeField] private float moveSpeed = 4;
    [SerializeField] private float rotateSpeed = 4;
    [SerializeField] private float threshold = 0.025f;
    private Vector3 targetPosition;
    #endregion


    #region //Monobehaviour
    private void Awake()
    {
        targetPosition = transform.position;
    }

    private void Start()
    {
        gridPosition = LevelGrid.instance.GetGridPosition(transform.position);
        LevelGrid.instance.AddUnitAtGridPosition(gridPosition, this);
    }

    private void Update()
    {
        animator.SetBool("isWalking", Move());
        GridPosition newPosition = LevelGrid.instance.GetGridPosition(transform.position);
        if(newPosition == gridPosition) return;
        LevelGrid.instance.UnitMovedGridPosition(gridPosition, newPosition, this);
        gridPosition = newPosition;
    }
    #endregion

    #region //Movement variables
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
    #endregion
}