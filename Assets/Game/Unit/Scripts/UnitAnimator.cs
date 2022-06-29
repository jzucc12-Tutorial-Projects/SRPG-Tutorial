using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    #region //Variables
    [SerializeField] private Animator animator = null;
    [SerializeField] private Bullet bulletPrefab = null;
    [SerializeField] private Transform bulletOrigin = null;
    #endregion


    #region //Monobehaviour
    private void OnEnable()
    {
        if(TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.StartMoving += StartMoving;
            moveAction.StopMoving += StopMoving;
        }

        if(TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.OnShoot += Shoot;
        }

        if(TryGetComponent<MeleeAction>(out MeleeAction meleeAction))
        {
            meleeAction.OnMeleeStarted += MeleeAttack;
        }
    }

    private void OnDisable()
    {
        if(TryGetComponent<MoveAction>(out MoveAction moveAction))
        {
            moveAction.StartMoving -= StartMoving;
            moveAction.StopMoving -= StopMoving;
        }

        if(TryGetComponent<ShootAction>(out ShootAction shootAction))
        {
            shootAction.OnShoot -= Shoot;
        }

        if(TryGetComponent<MeleeAction>(out MeleeAction meleeAction))
        {
            meleeAction.OnMeleeStarted -= MeleeAttack;
        } 
    }
    #endregion

    #region //Animation setting
    private void Shoot(Unit shooter, ITargetable target)
    {
        animator.SetTrigger("isShooting");
        var bullet = Instantiate(bulletPrefab, bulletOrigin.position, Quaternion.identity);
        var bulletTarget = target.GetWorldPosition();
        bulletTarget.y = bullet.transform.position.y;
        bullet.SetUp(bulletTarget);
    }

    private void StartMoving()
    {
        animator.SetBool("isWalking", true);
    }

    private void StopMoving()
    {
        animator.SetBool("isWalking", false);
    }

    private void MeleeAttack()
    {
        animator.SetTrigger("Melee");
    }
    #endregion
}