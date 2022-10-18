using System.Collections;
using UnityEngine;

public class Crawler : GenericMob
{
    private const float MaxSpeed = 7;
    
    /// <summary>
    /// Consts useful for the Animator's handling.
    /// </summary>
    #region Animator's consts

    private const string LocomotionCycleName = "LocomotionCycle";
    private const string AttackStateName = "Attack";
    private const string DieStateName = "Die";
    private const string BlindedStateName = "Blinded";

    private const string SpeedParameterName = "Speed";
    private const string AttackParameterName = "Attack";
    private const string DieParameterName = "Die";
    private const string BlindedParameterName = "Blinded";

    private const float AttackDamagingPhasePercentage = .2f;
    private const float DieWaitPercentage = .5f;
    private const float DieScaleLerpingSpeed = 1.5f;

    #endregion
    
    protected override Vector3 HealthBarPositionOffset => new Vector3(0, _height, 0);

    protected override Vector3 BlindnessBarPositionOffset => new Vector3(0, _height + 1, 0);
    
    protected override Vector3 CorrosionBarPositionOffset => new Vector3(0, _height + 2, 0);

    protected override Vector3 BlindnessEffectPositionOffset => new Vector3(0, _height / 2, 0);

    protected override Vector3 CorrosionEffectPositionOffset => new Vector3(0, _height / 2, 0);

    [SerializeField]
    private TriggerCaster _headCaster;

    private AudioClipHandler _crawlAudioClip;

    private float _desiredCrawlVolume;
    private float _crawlVolume;
    private float _crawlVolumeRefVelocity;
    
    protected new void Start()
    {
        base.Start();

        AnimController = GetComponentInChildren<Animator>();

        _crawlAudioClip = AudioClipHandler.PlayAudio("Audio/Crawl", 1, transform.position, true, 1);
        _crawlAudioClip.transform.parent = transform;
    }

    private void FixedUpdate()
    {
        UpdateBars();

        Vector2 localSpaceVelocity = transform.InverseTransformDirection(_attachedRigidbody.velocity);
        float normalizedSpeed = localSpaceVelocity.x / MaxSpeed;

        if (normalizedSpeed < .2f)
        {
            normalizedSpeed = 0;
        }

        AnimController.SetFloat(SpeedParameterName, normalizedSpeed);

        AnimController.SetBool(BlindedParameterName, Status.IsBlinded);

        _desiredCrawlVolume = (normalizedSpeed < .2f) ? 0 : 1;
        _crawlVolume = Mathf.SmoothDamp(_crawlVolume, _desiredCrawlVolume, ref _crawlVolumeRefVelocity, .1f);

        if (_crawlAudioClip != null)
        {
            if (_crawlAudioClip.GetComponent<AudioSource>() != null)
            {
                _crawlAudioClip.GetComponent<AudioSource>().volume = _crawlVolume;
            }
        }

        if (_isAttacking || _isDying || Status.IsBlinded)
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

        Vector2 moveDirection = (CurrentPatrolPoint.position - transform.position).normalized;
        moveDirection.y = 0;
        Vector2 lookDirection = moveDirection;

        float distance = Vector2.Distance(transform.position, CurrentPatrolPoint.position);

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
                Vector2 moveDirection = (player.transform.position - transform.position).normalized;
                moveDirection.y = 0;
                Vector2 lookDirection = moveDirection;

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

                _attachedRigidbody.AddForce(moveDirection * _mass * _speed);
            }
        }
    }

    protected override IEnumerator Attack(PlayerController target)
    {
        Vector2 lookDirection = (target.transform.position - transform.position).normalized;
        lookDirection.y = 0;

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

        AnimController.SetTrigger(AttackParameterName);

        yield return new WaitUntil(() => AnimController.GetCurrentAnimatorStateInfo(0).IsName(AttackStateName));

        AnimatorStateInfo info = AnimController.GetCurrentAnimatorStateInfo(0);

        float animationDuration = info.length / info.speed;

        foreach (Collider2D collider in _headCaster.GetComponents<Collider2D>())
        {
            collider.enabled = false;
        }

        yield return new WaitForSeconds(animationDuration * AttackDamagingPhasePercentage);

        AudioClipHandler.PlayAudio("Audio/Slash", 0, transform.position, false, .2f);

        if (_headCaster != null)
        {
            _headCaster.TriggerFunction = collider => {
                AudioClipHandler.PlayAudio("Audio/Damage", 0, transform.position, false, .1f);
                InflictDamage(collider, 20); 
            };
        }

        foreach (Collider2D collider in _headCaster.GetComponents<Collider2D>())
        {
            collider.enabled = true;
        }
        
        yield return new WaitUntil(() => !AnimController.GetCurrentAnimatorStateInfo(0).IsName(AttackStateName));

        if (_headCaster != null)
        {
            _headCaster.TriggerFunction = null;
        }
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

        if (_crawlAudioClip != null)
        {
            _crawlAudioClip.StopClip();
        }

        AudioClipHandler.PlayAudio("Audio/DyingCrawler", 1, transform.position, false, .1f);
        
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

        Destroy(gameObject);
    }

    #endregion
}
