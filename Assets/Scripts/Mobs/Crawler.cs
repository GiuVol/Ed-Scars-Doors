using System.Collections;
using UnityEngine;

public class Crawler : GenericMob
{
    /// <summary>
    /// Consts useful for the Animator's handling.
    /// </summary>
    #region Animator's consts

    private const string LocomotionCycleName = "LocomotionCycle";
    private const string AttackStateName = "Attack";
    private const string DieStateName = "Die";

    private const string SpeedParameterName = "Speed";
    private const string AttackParameterName = "Attack";
    private const string DieParameterName = "Die";

    private const float AttackDamagingPhasePercentage = .5f;
    private const float DieWaitPercentage = .5f;
    private const float DieScaleLerpingSpeed = .5f;

    #endregion
    
    protected override Vector3 HealthBarPositionOffset => new Vector3(0, _height, 0);

    [SerializeField]
    private TriggerCaster _headCaster;

    protected new void Start()
    {
        base.Start();

        AnimController = GetComponentInChildren<Animator>();
    }

    private void FixedUpdate()
    {
        if (HealthBar != null)
        {
            HealthBar.UpdateCurrentValue(Health.CurrentHealth);
        }
        
        if (_isAttacking || _isDying)
        {
            return;
        }

        if (_player == null)
        {
            Patrol();
        } else
        {
            HandlePlayer(_player);
        }

        Vector3 localSpaceVelocity = transform.InverseTransformDirection(_attachedRigidbody.velocity);
        float normalizedSpeed = localSpaceVelocity.x / (_speed / _attachedRigidbody.drag);

        if (normalizedSpeed < .2f)
        {
            normalizedSpeed = 0;
        }

        AnimController.SetFloat(SpeedParameterName, normalizedSpeed);
    }

    #region Behaviour

    private void Patrol()
    {
        if (!CanPatrol)
        {
            if (!IsInvoking("SearchForPatrolPoints"))
            {
                InvokeRepeating("SearchForPatrolPoints", 0, 2f);
            }

            return;
        }

        Vector3 moveDirection = (CurrentPatrolPoint.position - transform.position).normalized;
        moveDirection.y = 0;
        Vector3 lookDirection = moveDirection;

        float distance = Vector3.Distance(transform.position, CurrentPatrolPoint.position);

        #region Moving

        if (distance > 1.5f)
        {
            _attachedRigidbody.AddForce(moveDirection * _mass * _speed);
        }
        else
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

        if (_canAttack)
        {
            float distance = Vector2.Distance(player.transform.position, transform.position);

            if (distance <= _attackRange)
            {
                StartCoroutine(HandleAttack(player));
            }
            else
            {
                Vector3 moveDirection = (player.transform.position - transform.position).normalized;
                _attachedRigidbody.AddForce(moveDirection * _mass * _speed);
            }
        }
    }

    protected override IEnumerator Attack(PlayerController target)
    {
        AnimController.SetTrigger(AttackParameterName);

        yield return new WaitUntil(() => AnimController.GetCurrentAnimatorStateInfo(0).IsName(AttackStateName));

        AnimatorStateInfo info = AnimController.GetCurrentAnimatorStateInfo(0);

        float animationDuration = info.length / info.speed;

        yield return new WaitForSeconds(animationDuration * AttackDamagingPhasePercentage);

        if (_headCaster != null)
        {
            _headCaster.TriggerFunction = collider => { InflictDamage(collider, 20); };
        }
        
        yield return new WaitUntil(() => !AnimController.GetCurrentAnimatorStateInfo(0).IsName(AttackStateName));
    }

    private void InflictDamage(Collider2D collision, float power)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer(PlayerController.PlayerLayerName))
        {
            return;
        }

        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        if (player == null)
        {
            player = collision.gameObject.GetComponentInChildren<PlayerController>();
        }

        if (player == null)
        {
            player = collision.gameObject.GetComponentInParent<PlayerController>();
        }

        if (player != null)
        {
            HealthComponent healthComponent = player.Health;
            StatsComponent statsComponent = player.Stats;
            int damage = GameFormulas.Damage(power, Stats.Attack.CurrentValue, statsComponent.Defence.CurrentValue);
            healthComponent.Decrease(damage);
        }
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

            yield return null;
        }

        Destroy(gameObject);
    }

    #endregion
}
