using System.Collections;
using UnityEngine;
using Pathfinding;

public class Flydier : GenericMob
{
    /// <summary>
    /// Specifies if the flydier should strictly follow the patrol points.
    /// </summary>
    [SerializeField]
    private bool _stayOnPattern;

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
        
        PlayerController player = _mobAI.FindPlayerInRadius(transform.position, _rangeToCheck);
        
        if (_stayOnPattern)
        {
            if (PPGroup == null || PPGroup.FirstPatrolPoint == null)
            {
                if (!IsInvoking("SearchForPatrolPoints"))
                {
                    InvokeRepeating("SearchForPatrolPoints", 0, 2f);
                }
            } else
            {
                CancelInvoke("SearchForPatrolPoints");
                FollowPattern(player);
            }
        } else
        {
            if (player == null)
            {
                if (_mobAI.Target != null)
                {
                    _mobAI.Target = null;
                }

                Patrol();
            }
            else
            {
                HandlePlayer(player);
            }
        }
    }

    #region Behaviour

    private void FollowPattern(PlayerController player)
    {
        Patrol();

        Vector3 lookDirection;

        if (player == null)
        {
            lookDirection = (CurrentPatrolPoint.position - transform.position).normalized;
        }
        else
        {
            lookDirection = (player.transform.position - transform.position).normalized;
        }

        if (lookDirection.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (lookDirection.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, -180, 0);
        }

        if (player != null)
        {
            float heightDistance = Mathf.Abs(transform.position.y - player.transform.position.y);

            if (heightDistance < 3)
            {
                StartCoroutine(HandleAttack(player));
            }
        }
    }

    private void Patrol()
    {
        if (PPGroup == null || PPGroup.FirstPatrolPoint == null)
        {
            if (!IsInvoking("SearchForPatrolPoints"))
            {
                InvokeRepeating("SearchForPatrolPoints", 0, 2f);
            }

            return;
        }
        else
        {
            CancelInvoke("SearchForPatrolPoints");
        }

        Vector3 direction = (CurrentPatrolPoint.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, CurrentPatrolPoint.position);

        if (distance > 3)
        {
            _attachedRigidbody.AddForce(direction * _speed * Time.deltaTime);
        } else
        {
            IncreasePatrolPoint();
        }
    }

    private void HandlePlayer(PlayerController player)
    {
        Vector3 leftPosition = player.transform.position + Vector3.left * _attackRange;
        Vector3 rightPosition = player.transform.position + Vector3.right * _attackRange;

        float leftDistance = Vector3.Distance(transform.position, leftPosition);
        float rightDistance = Vector3.Distance(transform.position, rightPosition);

        Vector3 targetPosition = (leftDistance < rightDistance) ? leftPosition : rightPosition;

        _target.position = _mobAI.GetNearestReachablePosition(targetPosition);

        float distance = Vector3.Distance(transform.position, _target.position);

        if (distance > 2)
        {
            Chase(_target);

            Vector3 lookDirection = (player.transform.position - transform.position).normalized;

            if (lookDirection.x > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if (lookDirection.x < 0)
            {
                transform.rotation = Quaternion.Euler(0, -180, 0);
            }
        }
        else
        {
            if (_canAttack)
            {
                StartCoroutine(HandleAttack(player));
            }
        }
    }

    private void Chase(Transform targetTransform)
    {
        _mobAI.Target = targetTransform;

        _attachedRigidbody.AddForce(_mobAI.DesiredDirection * _speed * Time.deltaTime);
    }

    protected override IEnumerator Attack(PlayerController target)
    {
        Transform playerTransform = target.transform;

        Vector3 lookDirection = 
            ((playerTransform.position + playerTransform.up * 2) - transform.position).normalized;

        if (lookDirection.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (lookDirection.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, -180, 0);
        }

        Vector3 shootDirection = target.transform.position - transform.position;
        shootDirection.y = 0;

        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;
        Quaternion desiredRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Vector3 spawnPoint = transform.position + transform.right * 2;

        Projectile resource =
            Resources.Load<Projectile>(Projectile.ProjectileResourcesPath + Projectile.NormalProjectileName);

        Projectile projectile = Instantiate(resource, spawnPoint, desiredRotation);

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
