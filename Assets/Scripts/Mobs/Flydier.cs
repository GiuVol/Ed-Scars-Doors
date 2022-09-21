using System.Collections;
using UnityEngine;

public class Flydier : GenericMob
{
    /// <summary>
    /// Const which represent how variable the x position offset is.
    /// </summary>
    private const float XOffsetVariance = 2;

    /// <summary>
    /// The minimum difference of positions y component that the flydier needs to attack the target.
    /// </summary>
    private const float HeightDistanceToAttack = 2;

    /// <summary>
    /// Consts useful for the Animator's handling.
    /// </summary>
    #region Animator's consts

    private const string FlyCycleStateName = "FlyCycle";
    private const string AttackStateName = "Attack";
    private const string DieStateName = "Die";

    private const string SpeedParameterName = "Speed";
    private const string AttackParameterName = "Attack";
    private const string DieParameterName = "Die";

    private const float AttackProjectileSpawnPercentage = .25f;
    private const float DieWaitPercentage = .5f;
    private const float DieScaleLerpingSpeed = 1.5f;

    #endregion

    /// <summary>
    /// Stores the path to the prefab of the flydier.
    /// </summary>
    public const string PrefabPath = "Mobs/Flydier";

    protected override Vector3 HealthBarPositionOffset
    {
        get
        {
            return new Vector3(0, 3, 0);
        }
    }

    /// <summary>
    /// Specifies if the flydier should strictly follow the patrol points.
    /// </summary>
    [SerializeField]
    private bool _remainsOnPattern;

    /// <summary>
    /// This property is used to set _remainsOnPattern field from outside.
    /// </summary>
    public bool RemainsOnPattern
    {
        set
        {
            _remainsOnPattern = value;
        }
    }

    /// <summary>
    /// Specifies where does the projectiles come from.
    /// </summary>
    [SerializeField]
    private Transform _projectileSpawnPoint;
    
    /// <summary>
    /// The custom Flydier target, that specifies where to go.
    /// </summary>
    private Transform _target;

    /// <summary>
    /// The eventual Spawnest that spawned this mob.
    /// </summary>
    private Spawnest _spawner;

    /// <summary>
    /// A property to set the value of the spawner.
    /// </summary>
    public Spawnest Spawner
    {
        set
        {
            _spawner = value;
        }
    }

    /// <summary>
    /// Stores a randomly generated value that gives variability to the target position of the flydier.
    /// </summary>
    private float _xPositionOffset;
    
    #region Start and Setup

    protected new void Start()
    {
        base.Start();

        AnimController = GetComponentInChildren<Animator>();

        _target = new GameObject("FlydierTarget").transform;

        InvokeRepeating("UpdateXOffset", 0, 5);
    }

    protected override void SetupLayers()
    {
        base.SetupLayers();
        LayersToIgnore.Add(LayerMask.NameToLayer(GameFormulas.ObstacleLayerName));
    }

    #endregion

    private void FixedUpdate()
    {
        if (HealthBar != null)
        {
            HealthBar.UpdateValue(Health.CurrentHealth);
        }

        if (_isAttacking || _isDying)
        {
            return;
        }

        if (_remainsOnPattern && CanPatrol)
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

        Vector2 localSpaceVelocity = transform.InverseTransformDirection(_attachedRigidbody.velocity);
        float normalizedSpeed = localSpaceVelocity.x / (_speed / _attachedRigidbody.drag);

        AnimController.SetFloat(SpeedParameterName, normalizedSpeed);
    }

    /// <summary>
    /// Method used to change the x position offset that the flydier should follow when chasing the player.
    /// </summary>
    private void UpdateXOffset()
    {
        _xPositionOffset = Random.Range(-XOffsetVariance, XOffsetVariance);
    }

    #region Behaviour

    private void FollowPattern(PlayerController player)
    {
        Transform lookTarget = (player != null) ? player.transform : null;

        Patrol(lookTarget);

        if (player != null)
        {
            float heightDistance = Mathf.Abs(transform.position.y - player.transform.position.y);

            if (heightDistance <= HeightDistanceToAttack)
            {
                StartCoroutine(HandleAttack(player));
            }
        }
    }

    private void Patrol(Transform lookTarget = null)
    {
        if (!CanPatrol)
        {
            if (!IsInvoking("SearchForPatrolPoints"))
            {
                InvokeRepeating("SearchForPatrolPoints", 0, 2f);
            }

            return;
        }

        Vector2 moveDirection = (CurrentPatrolPoint.position - transform.position).normalized;

        Vector2 lookDirection;

        if (lookTarget == null)
        {
            lookDirection = moveDirection;
        }
        else
        {
            lookDirection = (lookTarget.position - transform.position).normalized;
        }

        float distance = Vector2.Distance(transform.position, CurrentPatrolPoint.position);

        #region Moving

        if (distance > 1.5f)
        {
            _attachedRigidbody.AddForce(moveDirection * _mass * _speed);
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

        Vector2 leftPosition = player.transform.position + Vector3.left * (_attackRange + _xPositionOffset);
        Vector2 rightPosition = player.transform.position + Vector3.right * (_attackRange + _xPositionOffset);

        float leftDistance = Vector2.Distance(transform.position, leftPosition);
        float rightDistance = Vector2.Distance(transform.position, rightPosition);

        Vector2 targetPosition = (leftDistance < rightDistance) ? leftPosition : rightPosition;

        _target.position = _mobAI.GetNearestReachablePosition(targetPosition);

        #endregion
        
        Chase(_target, player.transform);

        float heightDistance = Mathf.Abs(transform.position.y - _target.position.y);
        
        if (_canAttack && heightDistance <= HeightDistanceToAttack)
        {
            StartCoroutine(HandleAttack(player));
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
        _attachedRigidbody.AddForce(_mobAI.DesiredDirection * _mass * _speed);

        #region Rotating

        if (lookTarget == null)
        {
            lookTarget = positionTarget;
        }

        Vector2 lookDirection = (lookTarget.position - transform.position).normalized;

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

        Vector2 lookDirection = (playerTransform.position - transform.position).normalized;
        lookDirection.y = 0;
        Vector2 shootDirection = lookDirection;

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

        AnimController.SetTrigger(AttackParameterName);

        yield return new WaitUntil(() => AnimController.GetCurrentAnimatorStateInfo(0).IsName(AttackStateName));

        AnimatorStateInfo info = AnimController.GetCurrentAnimatorStateInfo(0);
        
        float animationDuration = info.length / info.speed;

        yield return new WaitForSeconds(animationDuration * AttackProjectileSpawnPercentage);

        #region Calculating the spawn position of the projectile

        Vector2 spawnPosition;

        if (_projectileSpawnPoint == null)
        {
            spawnPosition = transform.position + transform.right * 2;
        }
        else
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
        projectile.LayersToIgnore.Add(LayerMask.NameToLayer(GameFormulas.ObstacleLayerName));
        projectile.LayersToIgnore.Add(LayerMask.NameToLayer(GameFormulas.HidingPlaceLayerName));

        yield return new WaitUntil(() => !AnimController.GetCurrentAnimatorStateInfo(0).IsName(AttackStateName));

        yield break;
    }

    protected override IEnumerator Die()
    {
        AnimController.SetTrigger(DieParameterName);

        yield return new WaitUntil(() => AnimController.GetCurrentAnimatorStateInfo(0).IsName(DieStateName));

        AnimatorStateInfo info = AnimController.GetCurrentAnimatorStateInfo(0);

        float animationDuration = info.length / info.speed;

        yield return new WaitForSeconds(animationDuration * DieWaitPercentage);

        Vector3 startScale = transform.localScale;
        float lerpFactor = 0;

        while (transform.localScale != Vector3.zero)
        {
            lerpFactor = Mathf.Clamp01(lerpFactor + (Time.fixedDeltaTime * DieScaleLerpingSpeed));
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, lerpFactor);

            yield return new WaitForFixedUpdate();
        }

        if (_spawner != null)
        {
            _spawner.SpawnedFlydiers.Remove(this);
        }

        Destroy(_target.gameObject);
        Destroy(gameObject);
    }

    #endregion
}
