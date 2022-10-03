using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mantmare : GenericMob
{
    private const float Attack1Power = 5;
    private const float Attack2Power = 2.5f;
    private const float Attack3Power = 7.5f;

    #region Animator Consts

    private const string FlyCycleStateName = "FlyCycle";
    private const string Attack1StartStateName = "Attack1_start";
    private const string Attack1ChargeStateName = "Attack1_loop";
    private const string Attack1EndStateName = "Attack1_end";
    private const string Attack2StartStateName = "Attack2_start";
    private const string Attack2ChargeStateName = "Attack2_charge";
    private const string Attack2BoostStateName = "Attack2_boost";
    private const string Attack2FlyStateName = "Attack2_fly";
    private const string Attack2EndStateName = "Attack2_end";
    private const string Attack3StartStateName = "Attack3_start";
    private const string Attack3ChargeStateName = "Attack3_charge";
    private const string Attack3EndStateName = "Attack3_end";
    private const string WanderStateName = "Wander";
    private const string DieStateName = "Die";
    private const string BlindedStateName = "Blinded";

    private const string HorizontalSpeedParameterName = "HorizontalSpeed";
    private const string VerticalSpeedParameterName = "VerticalSpeed";
    private const string StartAttack1ParameterName = "StartAttack1";
    private const string EndAttack1ParameterName = "EndAttack1";
    private const string StartAttack2ParameterName = "StartAttack2";
    private const string StopChargingAttack2ParameterName = "StopChargingAttack2";
    private const string EndAttack2ParameterName = "EndAttack2";
    private const string StartAttack3ParameterName = "StartAttack3";
    private const string EndAttack3ParameterName = "EndAttack3";
    private const string WanderParameterName = "IsWandering";
    private const string DieParameterName = "Die";
    private const string BlindedParameterName = "Blinded";

    private const float DieScaleLerpingSpeed = 1.5f;

    #endregion

    protected override UIBar HealthBarResource
    {
        get
        {
            return Resources.Load<UIBar>("UI/MantmareHealthbar");
        }
    }

    protected override void SetupBars()
    {
        if (HealthBarResource != null)
        {
            Canvas canvas = GameObject.FindObjectOfType<Canvas>();

            if (canvas != null)
            {
                HealthBar = Instantiate(HealthBarResource, GameObject.FindObjectOfType<Canvas>().transform);
                HealthBar.InitializeStatic(Health.MaxHealth);
            }
        }
    }

    protected override void SetupLayers()
    {
        base.SetupLayers();
        LayersToIgnore.Add(LayerMask.NameToLayer(GameFormulas.TerrainLayerName));
        LayersToIgnore.Add(LayerMask.NameToLayer(GameFormulas.ObstacleLayerName));
        LayersToIgnore.Add(LayerMask.NameToLayer("Default"));
    }

    /// <summary>
    /// Returns the stage of Mantmare, based on its health.
    /// </summary>
    private int MantmareStageByHealth
    {
        get
        {
            int stage = 1;

            int currentHealth = Health.CurrentHealth;
            int maxHealth = Health.MaxHealth;
            
            if (currentHealth > Mathf.RoundToInt(0.6f * maxHealth))
            {
                stage = 1;
            }
            else if (currentHealth > Mathf.RoundToInt(0.4f * maxHealth))
            {
                stage = 2;
            }
            else
            {
                stage = 3;
            }
            
            return stage;
        }
    }

    /// <summary>
    /// Returns a random attack interval, based on the stage of Mantmare.
    /// </summary>
    private float AttackIntervalByStage
    {
        get
        {
            float attackInterval;

            switch (MantmareStageByHealth)
            {
                case 1:
                    attackInterval = Random.Range(3f, 6f);
                    break;
                case 2:
                    attackInterval = Random.Range(2f, 3f);
                    break;
                case 3:
                    attackInterval = Random.Range(1f, 2f);
                    break;
                default:
                    attackInterval = Random.Range(4f, 6f);
                    break;
            }

            return attackInterval;
        }
    }

    /// <summary>
    /// Returns a value that will be multiplied by the time that Mantmare has to wait to do a specific action.
    /// </summary>
    private float TimeMultiplierByStage
    {
        get
        {
            float timeMultiplier;

            switch (MantmareStageByHealth)
            {
                case 1:
                    timeMultiplier = 1;
                    break;
                case 2:
                    timeMultiplier = .75f;
                    break;
                case 3:
                    timeMultiplier = .5f;
                    break;
                default:
                    timeMultiplier = 1;
                    break;
            }

            return timeMultiplier;
        }
    }

    /// <summary>
    /// Returns a value that will multiply the capacities of Mantmare.
    /// </summary>
    private float PowerMultiplierByStage
    {
        get
        {
            float powerMultiplier;

            switch (MantmareStageByHealth)
            {
                case 1:
                    powerMultiplier = 1;
                    break;
                case 2:
                    powerMultiplier = 1.25f;
                    break;
                case 3:
                    powerMultiplier = 1.5f;
                    break;
                default:
                    powerMultiplier = 1;
                    break;
            }

            return powerMultiplier;
        }
    }
    
    /// <summary>
    /// Returns whether Mantmare is on screen or not.
    /// </summary>
    private bool IsOnScreen
    {
        get
        {
            bool isOnScreen = false;

            Camera camera = Camera.main;
            Vector3 position = transform.position;

            if (camera != null)
            {
                if (position.x >= camera.ScreenToWorldPoint(new Vector2(0, 0)).x + (_width / 2) &&
                    position.x <= camera.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x - (_width / 2) &&
                    position.y >= camera.ScreenToWorldPoint(new Vector2(0, 0)).y &&
                    position.y <= camera.ScreenToWorldPoint(new Vector2(0, Screen.height)).y - (_height))
                {
                    isOnScreen = true;
                }
            }

            return isOnScreen;
        }
    }

    /// <summary>
    /// Stores the time that the Mantmare has been visible to the camera.
    /// </summary>
    private float _timeOnScreen;

    /// <summary>
    /// Stores a random position on the screen, useful for Mantmare to wander.
    /// </summary>
    private Vector3 _randomWanderPosition;

    #region Rig

    /// <summary>
    /// The component that is used to cast a collision which involves the left arm of Mantmare.
    /// </summary>
    [SerializeField]
    private TriggerCaster _leftArmTriggerCaster;

    /// <summary>
    /// The component that is used to cast a collision which involves the head of Mantmare.
    /// </summary>
    [SerializeField]
    private TriggerCaster _headTriggerCaster;
    
    #endregion

    #region Graphics

    [Header("Graphics")]
    [SerializeField]
    private SpriteRenderer _leg1;
    [SerializeField]
    private SpriteRenderer _leg2;
    [SerializeField]
    private SpriteRenderer _arm1;
    [SerializeField]
    private SpriteRenderer _arm2;
    [SerializeField]
    private SpriteRenderer _bust;
    [SerializeField]
    private SpriteRenderer _head;
    [SerializeField]
    private Color _normalLegsColor;
    [SerializeField]
    private Color _normalArmsColor;
    [SerializeField]
    private Color _normalBustColor;
    [SerializeField]
    private Color _normalHeadColor;
    [SerializeField]
    private Color _stage2LegsColor;
    [SerializeField]
    private Color _stage2ArmsColor;
    [SerializeField]
    private Color _stage2BustColor;
    [SerializeField]
    private Color _stage2HeadColor;
    [SerializeField]
    private Color _stage3LegsColor;
    [SerializeField]
    private Color _stage3ArmsColor;
    [SerializeField]
    private Color _stage3BustColor;
    [SerializeField]
    private Color _stage3HeadColor;

    private Color LegsColorByStage
    {
        get
        {
            Color color;

            switch (MantmareStageByHealth)
            {
                case 1:
                    color = _normalLegsColor;
                    break;
                case 2:
                    color = _stage2LegsColor;
                    break;
                case 3:
                    color = _stage3LegsColor;
                    break;
                default:
                    color = _normalLegsColor;
                    break;
            }

            return color;
        }
    }

    private Color ArmsColorByStage
    {
        get
        {
            Color color;

            switch (MantmareStageByHealth)
            {
                case 1:
                    color = _normalArmsColor;
                    break;
                case 2:
                    color = _stage2ArmsColor;
                    break;
                case 3:
                    color = _stage3ArmsColor;
                    break;
                default:
                    color = _normalArmsColor;
                    break;
            }

            return color;
        }
    }

    private Color BustColorByStage
    {
        get
        {
            Color color;

            switch (MantmareStageByHealth)
            {
                case 1:
                    color = _normalBustColor;
                    break;
                case 2:
                    color = _stage2BustColor;
                    break;
                case 3:
                    color = _stage3BustColor;
                    break;
                default:
                    color = _normalBustColor;
                    break;
            }

            return color;
        }
    }

    private Color HeadColorByStage
    {
        get
        {
            Color color;

            switch (MantmareStageByHealth)
            {
                case 1:
                    color = _normalHeadColor;
                    break;
                case 2:
                    color = _stage2HeadColor;
                    break;
                case 3:
                    color = _stage3HeadColor;
                    break;
                default:
                    color = _normalHeadColor;
                    break;
            }

            return color;
        }
    }
    
    #endregion

    protected new void Start()
    {
        base.Start();

        AnimController = GetComponentInChildren<Animator>();

        _attackInterval = AttackIntervalByStage;

        if (IsOnScreen)
        {
            _timeOnScreen = 2;
        }

        InvokeRepeating("UpdateRandomWanderPosition", 0, 3);
    }

    #region Random points on screen

    /// <summary>
    /// A procedure used to update the random wander position.
    /// </summary>
    private void UpdateRandomWanderPosition()
    {
        GetRandomPointOnScreen(out _randomWanderPosition);
    }

    /// <summary>
    /// A procedure used to calculate a random position on the screen.
    /// </summary>
    /// <param name="position">A Vector3 that will store the found position</param>
    /// <returns>returns whether the position has been found or not</returns>
    private bool GetRandomPointOnScreen(out Vector3 position)
    {
        if (Camera.main == null)
        {
            position = Vector3.zero;
            return false;
        }

        float randomX = Random.Range
            (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x + (_width / 2), 
             Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x - (_width / 2));
        float randomY = Random.Range
            (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y, 
             Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y - (_height));

        position = new Vector3(randomX, randomY);

        return true;
    }

    #endregion

    private void FixedUpdate()
    {
        UpdateBars();

        Vector2 localSpaceXVelocity = transform.InverseTransformDirection(_attachedRigidbody.velocity);
        float normalizedXSpeed = localSpaceXVelocity.x / (_speed / _attachedRigidbody.drag);

        Vector2 localSpaceYVelocity = transform.InverseTransformDirection(_attachedRigidbody.velocity);
        float normalizedYSpeed = localSpaceYVelocity.y / (_speed / _attachedRigidbody.drag);

        if (normalizedXSpeed < .2f)
        {
            normalizedXSpeed = 0;
        }

        if (normalizedYSpeed < .2f)
        {
            normalizedYSpeed = 0;
        }
        
        AnimController.SetFloat(HorizontalSpeedParameterName, normalizedXSpeed);
        AnimController.SetFloat(VerticalSpeedParameterName, normalizedYSpeed);

        AnimController.SetBool(BlindedParameterName, Status.IsBlinded);

        if (!_isChangingColor)
        {
            if (_leg1 != null && 
                _leg2 != null && 
                _arm1 != null && 
                _arm2 != null && 
                _bust != null && 
                _head != null)
            {
                _leg1.color = LegsColorByStage;
                _leg2.color = LegsColorByStage;
                _arm1.color = ArmsColorByStage;
                _arm2.color = ArmsColorByStage;
                _bust.color = BustColorByStage;
                _head.color = HeadColorByStage;
            }
        }

        if (Camera.main == null)
        {
            return;
        }

        if (IsOnScreen)
        {
            _timeOnScreen = Mathf.Clamp(_timeOnScreen + Time.fixedDeltaTime, 0, 50);
        } else
        {
            _timeOnScreen = 0;
        }

        if (_isAttacking || _isDying || Status.IsBlinded)
        {
            return;
        }

        if (_timeOnScreen < 2)
        {
            ReachPosition(_randomWanderPosition, _speed, true);
        } else
        {
            if (_player != null)
            {
                if (_canAttack)
                {
                    StartCoroutine(HandleAttack(_player));
                } else
                {
                    RandomWander(_player.transform);
                }
            }
        }
    }

    #region Movement

    /// <summary>
    /// A procedure used to change the position of Mantmare.
    /// </summary>
    /// <param name="positionToReach">The position to reach</param>
    /// <param name="speed">The movement speed, if not specified Mantmare will move with its standard speed</param>
    /// <param name="rotate">If this value is true, Mantmare will rotate towards the position to reach</param>
    private void ReachPosition(Vector3 positionToReach, float speed = 0, bool rotate = false)
    {
        if (Vector3.Distance(transform.position, positionToReach) < 2f)
        {
            return;
        }

        if (speed <= 0)
        {
            speed = _speed;
        }

        Vector2 direction = (positionToReach - transform.position).normalized;

        _attachedRigidbody.AddForce(direction * _mass * speed);

        if (rotate)
        {
            Quaternion desiredRotation = transform.rotation;

            if (direction.x > 0)
            {
                desiredRotation = Quaternion.Euler(0, 0, 0);
            }
            else if (direction.x < 0)
            {
                desiredRotation = Quaternion.Euler(0, -180, 0);
            }

            transform.rotation = desiredRotation;
        }
    }

    /// <summary>
    /// A procedure that makes Mantmare wander.
    /// </summary>
    /// <param name="playerTransform"></param>
    private void RandomWander(Transform playerTransform)
    {
        if (playerTransform == null)
        {
            return;
        }

        ReachPosition(_randomWanderPosition, _speed);

        Vector3 lookDirection = (playerTransform.position - transform.position).normalized;

        if (lookDirection.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        } else if (lookDirection.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, -180, 0);
        }
    }

    #endregion

    #region Attack

    protected override IEnumerator Attack(PlayerController target)
    {
        AnimController.SetBool(WanderParameterName, false);

        int decidedPattern = Random.Range(1, 4);

        switch (decidedPattern)
        {
            case 1:
                yield return Pattern1(target);
                break;
            case 2:
                yield return Pattern2(target);
                break;
            case 3:
                yield return Pattern3(target);
                break;
            default:
                yield return Pattern1(target);
                break;
        }

        _attackInterval = AttackIntervalByStage;
    }

    /// <summary>
    /// The 1st attack pattern of Mantmare.
    /// </summary>
    private IEnumerator Pattern1(PlayerController target)
    {
        if (target == null)
        {
            yield break;
        }

        Vector2 targetPosition;
        float distance;

        do
        {
            targetPosition = target.transform.position +
                                 target.transform.right * _attackRange -
                                 Vector3.up * _height / 2;
            distance = Vector2.Distance(transform.position, targetPosition);
            ReachPosition(targetPosition, 80, true);
            yield return null;
        } while (distance > 5);

        _attachedRigidbody.velocity = Vector3.zero;

        Vector2 lookDirection = (target.transform.position - transform.position).normalized;
        Quaternion desiredRotation = transform.rotation;

        if (lookDirection.x > 0)
        {
            desiredRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (lookDirection.x < 0)
        {
            desiredRotation = Quaternion.Euler(0, -180, 0);
        }

        transform.rotation = desiredRotation;

        AnimController.SetTrigger(StartAttack1ParameterName);

        yield return new WaitForSeconds(.5f);

        _leftArmTriggerCaster.TriggerFunction = collider => InflictDamage(collider, Attack1Power);
        
        AnimController.SetTrigger(EndAttack1ParameterName);

        yield return new WaitUntil(() => AnimController.GetCurrentAnimatorStateInfo(0).IsName(Attack1EndStateName));

        yield return new WaitUntil(() => !AnimController.GetCurrentAnimatorStateInfo(0).IsName(Attack1EndStateName));

        _leftArmTriggerCaster.TriggerFunction = null;
    }

    /// <summary>
    /// The 2nd attack pattern of Mantmare.
    /// </summary>
    private IEnumerator Pattern2(PlayerController target)
    {
        if (target == null)
        {
            yield break;
        }

        Vector3 leftScreenPosition = 
            Camera.main.ScreenToWorldPoint(new Vector3(Mathf.RoundToInt((1f / 8f) * Screen.width), 
                                           0, 
                                           0));

        Vector3 rightScreenPosition =
            Camera.main.ScreenToWorldPoint(new Vector3(Mathf.RoundToInt((7f / 8f) * Screen.width),
                                           0,
                                           0));

        for (int i = 0; i < 4; i++)
        {
            Vector3 startScreenPosition;
            Vector3 endScreenPosition;

            if (Vector3.Distance(transform.position, leftScreenPosition) >
                Vector3.Distance(transform.position, rightScreenPosition))
            {
                startScreenPosition = rightScreenPosition;
                endScreenPosition = leftScreenPosition;
            }
            else
            {
                startScreenPosition = leftScreenPosition;
                endScreenPosition = rightScreenPosition;
            }

            Vector3 patternStartPosition = (target != null) ? 
                new Vector3(startScreenPosition.x, target.transform.position.y) : 
                new Vector3(startScreenPosition.x, transform.position.y);
            
            Vector3 patternEndPosition = (target != null) ? 
                new Vector3(endScreenPosition.x, target.transform.position.y) : 
                new Vector3(endScreenPosition.x, transform.position.y);

            float distance;

            do
            {
                distance = Vector3.Distance(transform.position, patternStartPosition);
                ReachPosition(patternStartPosition);
                yield return null;
            } while (distance > 2);

            Vector3 lookDirection = (patternEndPosition - transform.position).normalized;
            Quaternion desiredRotation = transform.rotation;

            if (lookDirection.x > 0)
            {
                desiredRotation = Quaternion.Euler(0, 0, 0);
            }
            else if (lookDirection.x < 0)
            {
                desiredRotation = Quaternion.Euler(0, -180, 0);
            }

            transform.rotation = desiredRotation;

            AnimController.SetTrigger(StartAttack2ParameterName);

            yield return new WaitForSeconds(TimeMultiplierByStage / 2);

            AnimController.SetTrigger(StopChargingAttack2ParameterName);

            yield return new WaitUntil(() => AnimController.GetCurrentAnimatorStateInfo(0).IsName(Attack2BoostStateName));

            _headTriggerCaster.TriggerFunction = collider => InflictDamage(collider, Attack2Power);
            
            do
            {
                distance = Vector3.Distance(transform.position, patternEndPosition);
                ReachPosition(patternEndPosition, 150);
                yield return null;
            } while (distance > 2);

            _attachedRigidbody.velocity = Vector3.zero;
            AnimController.SetTrigger(EndAttack2ParameterName);

            yield return new WaitUntil(() => AnimController.GetCurrentAnimatorStateInfo(0).IsName(Attack2EndStateName));
            
            yield return new WaitUntil(() => !AnimController.GetCurrentAnimatorStateInfo(0).IsName(Attack2EndStateName));

            _headTriggerCaster.TriggerFunction = null;
        }
    }

    /// <summary>
    /// The 3rd attack pattern of Mantmare.
    /// </summary>
    private IEnumerator Pattern3(PlayerController target)
    {
        if (target == null)
        {
            yield break;
        }

        Vector2 leftScreenPosition =
            Camera.main.ScreenToWorldPoint(new Vector3(Mathf.RoundToInt((2f / 8f) * Screen.width),
                                           Mathf.RoundToInt((7f / 8f) * Screen.height),
                                           0)) - (Vector3.up * _height);

        Vector2 rightScreenPosition =
            Camera.main.ScreenToWorldPoint(new Vector3(Mathf.RoundToInt((6f / 8f) * Screen.width),
                                           Mathf.RoundToInt((7f / 8f) * Screen.height),
                                           0)) - (Vector3.up * _height);

        Vector3 patternStartPosition;
        Vector3 oppositePosition;

        if (Vector2.Distance(transform.position, leftScreenPosition) >
            Vector2.Distance(transform.position, rightScreenPosition))
        {
            patternStartPosition = rightScreenPosition;
            oppositePosition = leftScreenPosition;
        }
        else
        {
            patternStartPosition = leftScreenPosition;
            oppositePosition = rightScreenPosition;
        }

        float distance;
        
        do
        {
            distance = Vector2.Distance(transform.position, patternStartPosition);
            ReachPosition(patternStartPosition);
            yield return null;
        } while (distance > 2);

        _attachedRigidbody.velocity = Vector3.zero;

        Vector2 lookDirection = (oppositePosition - transform.position).normalized;
        Quaternion desiredRotation = transform.rotation;

        if (lookDirection.x > 0)
        {
            desiredRotation = Quaternion.Euler(0, 0, 0);
        }
        else if (lookDirection.x < 0)
        {
            desiredRotation = Quaternion.Euler(0, -180, 0);
        }

        transform.rotation = desiredRotation;

        AnimController.SetTrigger(StartAttack3ParameterName);

        yield return new WaitForSeconds(2 * TimeMultiplierByStage);

        AnimController.SetTrigger(EndAttack3ParameterName);

        yield return new WaitUntil(() => AnimController.GetCurrentAnimatorStateInfo(0).IsName(Attack3EndStateName));

        yield return new WaitUntil(() => !AnimController.GetCurrentAnimatorStateInfo(0).IsName(Attack3EndStateName));
    }
    
    /// <summary>
    /// The procedure used to inflict damage to the player.
    /// </summary>
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

    #endregion

    protected override IEnumerator Die()
    {
        StopCoroutine(_attackCoroutine);

        AnimController.SetTrigger(DieParameterName);

        yield return new WaitUntil(() => AnimController.GetCurrentAnimatorStateInfo(0).IsName(DieStateName));

        AnimatorStateInfo info = AnimController.GetCurrentAnimatorStateInfo(0);

        float animationDuration = info.length / info.speed;

        yield return new WaitForSeconds(animationDuration * 1.5f);

        Vector3 startScale = transform.localScale;
        float lerpFactor = 0;

        while (transform.localScale != Vector3.zero)
        {
            lerpFactor = Mathf.Clamp01(lerpFactor + (Time.fixedDeltaTime * DieScaleLerpingSpeed));
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, lerpFactor);

            yield return new WaitForFixedUpdate();
        }

        Destroy(gameObject);

        yield break;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayerController player = collision.gameObject.GetComponent<PlayerController>();

        if (player == null)
        {
            player = collision.gameObject.GetComponentInChildren<PlayerController>();
        }

        if (player == null)
        {
            player = collision.gameObject.GetComponentInParent<PlayerController>();
        }

        if (player == null)
        {
            return;
        }

        Vector3 offsettedPosition = new Vector3(transform.position.x, player.transform.position.y, player.transform.position.z);
        Vector3 conjunctionLine = (player.transform.position - offsettedPosition);
        conjunctionLine.z = 0;
        conjunctionLine.Normalize();

        Rigidbody2D playerRigidbody = player.GetComponent<Rigidbody2D>();

        if (playerRigidbody != null)
        {
            playerRigidbody.AddForce(conjunctionLine * _repulsiveForce);
        }

        player.Health.Decrease(_contactDamage);
    }
}
