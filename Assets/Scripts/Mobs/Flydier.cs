using System.Collections;
using UnityEngine;
using Pathfinding;

public class Flydier : GenericMob
{
    /// <summary>
    /// Specifies if the flydier should strictly follow the patrol points.
    /// </summary>
    [SerializeField]
    private bool _remainsOnPattern;

    /// <summary>
    /// Specifies where does the projectiles come from.
    /// </summary>
    [SerializeField]
    private Transform _projectileSpawnPoint;
    
    /// <summary>
    /// The custom Flydier target, that specifies where to go.
    /// </summary>
    private Transform _target;

    #region Start and Setup

    protected new void Start()
    {
        base.Start();
        _target = new GameObject("FlydierTarget").transform;
    }

    protected override void SetupLayers()
    {
        base.SetupLayers();
        LayersToIgnore.Add(LayerMask.NameToLayer(GameFormulas.ObstacleLayerName));
    }

    #endregion

    private void FixedUpdate()
    {
        if (_isAttacking)
        {
            return;
        }

        if (_remainsOnPattern)
        {
            FollowPattern(_player);
        } else
        {
            if (_player == null)
            {
                if (_mobAI.Target != null)
                {
                    _mobAI.Target = null;
                }

                Patrol();
            }
            else
            {
                HandlePlayer(_player);
            }
        }
    }

    #region Behaviour

    private void FollowPattern(PlayerController player)
    {
        Transform lookTarget = (player != null) ? player.transform : null;

        Patrol(lookTarget);

        if (player != null)
        {
            float heightDistance = Mathf.Abs(transform.position.y - player.transform.position.y);

            if (heightDistance <= .5f)
            {
                StartCoroutine(HandleAttack(player));
            }
        }
    }

    private void Patrol(Transform lookTarget = null)
    {
        bool canPatrol = false;

        if (PPGroup != null)
        {
            canPatrol = (PPGroup.FirstPatrolPoint != null);
        }

        if (!canPatrol)
        {
            if (!IsInvoking("SearchForPatrolPoints"))
            {
                InvokeRepeating("SearchForPatrolPoints", 0, 2f);
            }

            return;
        }

        Vector3 moveDirection = (CurrentPatrolPoint.position - transform.position).normalized;

        Vector3 lookDirection;

        if (lookTarget == null)
        {
            lookDirection = moveDirection;
        }
        else
        {
            lookDirection = (lookTarget.position - transform.position).normalized;
        }

        float distance = Vector3.Distance(transform.position, CurrentPatrolPoint.position);

        #region Moving

        if (distance > 1.5f)
        {
            _attachedRigidbody.AddForce(moveDirection * _mass * _speed * Time.deltaTime);
        } else
        {
            IncreasePatrolPoint();
        }

        #endregion

        #region Rotating

        if (lookDirection.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (lookDirection.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, -180, 0);
        }

        #endregion
    }

    private void HandlePlayer(PlayerController player)
    {
        if (player == null)
        {
            return;
        }

        #region Assigning the desired position to the target

        Vector3 leftPosition = player.transform.position + Vector3.left * _attackRange;
        Vector3 rightPosition = player.transform.position + Vector3.right * _attackRange;

        float leftDistance = Vector3.Distance(transform.position, leftPosition);
        float rightDistance = Vector3.Distance(transform.position, rightPosition);

        Vector3 targetPosition = (leftDistance < rightDistance) ? leftPosition : rightPosition;

        _target.position = _mobAI.GetNearestReachablePosition(targetPosition);

        #endregion
        
        float distance = Vector3.Distance(transform.position, _target.position);

        if (distance >= 1f)
        {
            Chase(_target, player.transform);
        }
        else
        {
            if (_canAttack)
            {
                StartCoroutine(HandleAttack(player));
            }
        }
    }

    private void Chase(Transform positionTarget, Transform lookTarget = null)
    {
        if (positionTarget == null)
        {
            return;
        }

        _mobAI.Target = positionTarget;

        //Moving
        _attachedRigidbody.AddForce(_mobAI.DesiredDirection * _mass * _speed * Time.deltaTime);

        #region Rotating

        if (lookTarget == null)
        {
            lookTarget = positionTarget;
        }

        Vector3 lookDirection = (lookTarget.position - transform.position).normalized;

        if (lookDirection.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (lookDirection.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, -180, 0);
        }

        #endregion
    }

    protected override IEnumerator Attack(PlayerController target)
    {
        if (target == null)
        {
            yield break;
        }

        Transform playerTransform = target.transform;

        Vector3 lookDirection = (playerTransform.position - transform.position).normalized;
        lookDirection.y = 0;
        Vector3 shootDirection = lookDirection;

        #region Rotating according to lookDirection

        if (lookDirection.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (lookDirection.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, -180, 0);
        }

        #endregion

        #region Calculating the spawn position of the projectile

        Vector3 spawnPosition;

        if (_projectileSpawnPoint == null)
        {
            spawnPosition = transform.position + transform.right * 2;
        } else
        {
            spawnPosition = _projectileSpawnPoint.position;
        }

        #endregion

        #region Calculating the desired rotation of the projectile

        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        Quaternion desiredRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        #endregion

        Projectile resource =
            Resources.Load<Projectile>(Projectile.ProjectileResourcesPath + Projectile.NormalProjectileName);

        Projectile projectile = Instantiate(resource, spawnPosition, desiredRotation);

        projectile.AttackerAttack = Stats.Attack.CurrentValue;

        projectile.Layer = LayerMask.NameToLayer(MobProjectileLayerName);

        projectile.LayersToIgnore.Add(LayerMask.NameToLayer(MobLayerName));
        projectile.LayersToIgnore.Add(LayerMask.NameToLayer(MobProjectileLayerName));

        yield break;
    }

    protected override void Die()
    {
        Destroy(_target.gameObject);
        Destroy(gameObject);
    }

    #endregion
}
