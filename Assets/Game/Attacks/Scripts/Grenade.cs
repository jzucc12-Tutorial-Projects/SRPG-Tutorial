using System;
using UnityEngine;

public class Grenade : MonoBehaviour
{
    #region //Grenade variables
    [SerializeField] private float moveSpeed = 15f;
    [SerializeField] private float explosionRadius = 2;
    [SerializeField] private int damage = 25;
    private Vector3 target = Vector3.zero;
    private GridPosition targetPosition => LevelGrid.instance.GetGridPosition(target);
    private event Action OnGrenadComplete;
    #endregion

    #region //Effects
    public static event Action OnExplodeStatic;
    [SerializeField] private ParticleSystem explosionParticles = null;
    [SerializeField] private TrailRenderer trail = null;
    [SerializeField] private AnimationCurve projectilePath = null;
    private float totalDistance = 0;
    private Vector3 positionXZ = Vector3.zero;
    #endregion



    #region //Monobehaviour
    private void Update()
    {
        var moveDir = (target - transform.position).normalized;
        positionXZ = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        float distanceNorm = 1 - Vector3.Distance(positionXZ, target) / totalDistance;

        float maxHeight = totalDistance / 4f;
        float yPos = projectilePath.Evaluate(distanceNorm) * maxHeight;
        transform.position = new Vector3(positionXZ.x, yPos, positionXZ.z);
        if((target - transform.position).InRange(0.1f)) 
        {
            transform.position = target;
            Collider[] hits = Physics.OverlapSphere(target, explosionRadius * LevelConstants.cellSize);
            foreach(var collider in hits)
            {
                if(collider.TryGetComponent<Unit>(out Unit targetUnit))
                {
                    var unitDamage = damage * (targetUnit.GetGridPosition() == targetPosition ? 2 : 1);
                    targetUnit.Damage(unitDamage);
                }

                if(collider.TryGetComponent<DestructableObject>(out DestructableObject obj))
                {
                    obj.Damage();
                }
            }
            Instantiate(explosionParticles, target + Vector3.up, Quaternion.identity);
            trail.transform.parent = null;
            OnExplodeStatic?.Invoke();
            Destroy(gameObject);
            OnGrenadComplete?.Invoke();
        }
    }
    #endregion

    #region //Set up
    public void Setup(GridPosition targetGridPosition, Action OnGrenadeComplete)
    {
        target = LevelGrid.instance.GetWorldPosition(targetGridPosition);
        this.OnGrenadComplete = OnGrenadeComplete;
        positionXZ = transform.position;
        positionXZ.y = 0;
        totalDistance = Vector3.Distance(transform.position, target);
    }
    #endregion
}
