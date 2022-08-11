using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mantmare : GenericMob
{
    protected override UIBar HealthBarResource
    {
        get
        {
            return Resources.Load<UIBar>("UI/MantmareHealthbar");
        }
    }

    protected override void SetupHealthBar()
    {
        if (HealthBarResource != null)
        {
            HealthBar = Instantiate(HealthBarResource, GameObject.FindObjectOfType<Canvas>().transform);
            HealthBar.InitializeStatic(Health.MaxHealth);
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
                    attackInterval = Random.Range(4, 6);
                    break;
                case 2:
                    attackInterval = Random.Range(2, 4);
                    break;
                case 3:
                    attackInterval = Random.Range(1.5f, 3.5f);
                    break;
                default:
                    attackInterval = Random.Range(4, 6);
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

            Vector3 offsettedPosition = transform.position + Vector3.up * 5;

            if (offsettedPosition.x > Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x &&
                offsettedPosition.x < Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x &&
                offsettedPosition.y > Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y &&
                offsettedPosition.y < Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y)
            {
                isOnScreen = true;
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
            (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).x, Camera.main.ScreenToWorldPoint(new Vector2(Screen.width, 0)).x);
        float randomY = Random.Range
            (Camera.main.ScreenToWorldPoint(new Vector2(0, 0)).y, Camera.main.ScreenToWorldPoint(new Vector2(0, Screen.height)).y);

        position = new Vector3(randomX, randomY);

        return true;
    }

    #endregion

    private void FixedUpdate()
    {
        if (HealthBar != null)
        {
            HealthBar.UpdateCurrentValue(Health.CurrentHealth);
        }

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

        if (_isAttacking || _isDying)
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
                    AnimController.SetBool("IsWandering", false);
                    StartCoroutine(HandleAttack(_player));
                } else
                {
                    AnimController.SetBool("IsWandering", true);
                    RandomWander(_player.transform);
                }
            }
        }

        Vector3 localSpaceXVelocity = transform.InverseTransformDirection(_attachedRigidbody.velocity);
        float normalizedXSpeed = localSpaceXVelocity.x / (_speed / _attachedRigidbody.drag);

        Vector3 localSpaceYVelocity = transform.InverseTransformDirection(_attachedRigidbody.velocity);
        float normalizedYSpeed = localSpaceYVelocity.y / (_speed / _attachedRigidbody.drag);

        AnimController.SetFloat("HorizontalSpeed", normalizedXSpeed);
        AnimController.SetFloat("VerticalSpeed", normalizedYSpeed);
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
        if (speed <= 0)
        {
            speed = _speed;
        }

        Vector3 direction = (positionToReach - transform.position).normalized;

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

        ReachPosition(_randomWanderPosition, 10);

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
        Vector3 targetPosition = target.transform.position + target.transform.right * _attackRange - Vector3.up * 5;

        float distance;

        do
        {
            distance = Vector3.Distance(transform.position, targetPosition);
            ReachPosition(targetPosition, _speed, true);
            yield return null;
        } while (distance > 2);

        _attachedRigidbody.velocity = Vector3.zero;

        Vector3 lookDirection = (target.transform.position - transform.position).normalized;
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

        AnimController.SetTrigger("StartAttack1");

        _leftArmTriggerCaster.TriggerFunction = collider => InflictDamage(collider, 70);

        yield return new WaitForSeconds(TimeMultiplierByStage);

        AnimController.SetTrigger("EndAttack1");

        yield return new WaitUntil(() => AnimController.GetCurrentAnimatorStateInfo(0).IsName("Attack1_end"));

        yield return new WaitUntil(() => !AnimController.GetCurrentAnimatorStateInfo(0).IsName("Attack1_end"));

        _leftArmTriggerCaster.TriggerFunction = null;
    }

    /// <summary>
    /// The 2nd attack pattern of Mantmare.
    /// </summary>
    private IEnumerator Pattern2(PlayerController target)
    {
        Vector3 leftScreenPosition = 
            Camera.main.ScreenToWorldPoint(new Vector3(Mathf.RoundToInt((2f / 8f) * Screen.width), 
                                           0, 
                                           0));

        Vector3 rightScreenPosition =
            Camera.main.ScreenToWorldPoint(new Vector3(Mathf.RoundToInt((6f / 8f) * Screen.width),
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

            Vector3 patternStartPosition =
                new Vector3(startScreenPosition.x, target.transform.position.y - 5);
            
            Vector3 patternEndPosition =
                new Vector3(endScreenPosition.x, target.transform.position.y - 5);

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

            AnimController.SetTrigger("StartAttack2");

            yield return new WaitForSeconds(TimeMultiplierByStage / 2);

            AnimController.SetTrigger("StopChargingAttack2");

            yield return new WaitUntil(() => AnimController.GetCurrentAnimatorStateInfo(0).IsName("Attack2_boost"));

            _headTriggerCaster.TriggerFunction = collider => InflictDamage(collider, 50);
            
            do
            {
                distance = Vector3.Distance(transform.position, patternEndPosition);
                ReachPosition(patternEndPosition, 50);
                yield return null;
            } while (distance > 2);

            _attachedRigidbody.velocity = Vector3.zero;
            AnimController.SetTrigger("EndAttack2");

            yield return new WaitUntil(() => AnimController.GetCurrentAnimatorStateInfo(0).IsName("Attack2_end"));
            
            yield return new WaitUntil(() => !AnimController.GetCurrentAnimatorStateInfo(0).IsName("Attack2_end"));

            _headTriggerCaster.TriggerFunction = null;
        }
    }

    /// <summary>
    /// The 3rd attack pattern of Mantmare.
    /// </summary>
    private IEnumerator Pattern3(PlayerController target)
    {
        Vector3 leftScreenPosition =
            Camera.main.ScreenToWorldPoint(new Vector3(Mathf.RoundToInt((2f / 8f) * Screen.width),
                                           Mathf.RoundToInt((3f / 8f) * Screen.height),
                                           0));

        Vector3 rightScreenPosition =
            Camera.main.ScreenToWorldPoint(new Vector3(Mathf.RoundToInt((6f / 8f) * Screen.width),
                                           Mathf.RoundToInt((3f / 8f) * Screen.height),
                                           0));

        Vector3 patternStartPosition;
        Vector3 oppositePosition;

        if (Vector3.Distance(transform.position, leftScreenPosition) >
            Vector3.Distance(transform.position, rightScreenPosition))
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
            ReachPosition(patternStartPosition, _speed);
            yield return null;
        } while (distance > 2);

        _attachedRigidbody.velocity = Vector3.zero;

        Vector3 lookDirection = (oppositePosition - transform.position).normalized;
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

        AnimController.SetTrigger("StartAttack3");

        yield return new WaitForSeconds(2 * TimeMultiplierByStage);

        AnimController.SetTrigger("EndAttack3");

        yield return new WaitUntil(() => AnimController.GetCurrentAnimatorStateInfo(0).IsName("Attack3_end"));

        yield return new WaitUntil(() => !AnimController.GetCurrentAnimatorStateInfo(0).IsName("Attack3_end"));
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
            healthComponent.Decrease(Mathf.RoundToInt(Stats.Attack.CurrentValue * power / (statsComponent.Defence.CurrentValue * 2)));
        }
    }

    #endregion

    protected override IEnumerator Die()
    {
        AnimController.SetTrigger("Die");

        yield return new WaitUntil(() => AnimController.GetCurrentAnimatorStateInfo(0).IsName("Die"));

        AnimatorStateInfo info = AnimController.GetCurrentAnimatorStateInfo(0);

        float animationDuration = info.length / info.speed;

        yield return new WaitForSeconds(animationDuration * 1.5f);

        Vector3 startScale = transform.localScale;
        float lerpFactor = 0;

        while (transform.localScale != Vector3.zero)
        {
            lerpFactor = Mathf.Clamp01(lerpFactor + (Time.fixedDeltaTime));
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, lerpFactor);

            yield return null;
        }

        Destroy(gameObject);

        yield break;
    }

    protected new void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isDying)
        {
            if (collision.gameObject.layer != LayerMask.NameToLayer(GameFormulas.TerrainLayerName))
            {
                Physics2D.IgnoreCollision(collision.collider, collision.otherCollider);
                return;
            }

            return;
        }

        if (LayersToIgnore.Contains(collision.gameObject.layer))
        {
            Physics2D.IgnoreCollision(collision.collider, collision.otherCollider);
            return;
        }

        Rigidbody2D rigidbody = collision.rigidbody;

        if (rigidbody == null)
        {
            return;
        }

        PlayerController player = rigidbody.GetComponent<PlayerController>();

        if (player == null)
        {
            return;
        }

        Vector3 offsettedPosition = new Vector3(transform.position.x, player.transform.position.y);
        Vector2 conjunctionLine = (player.transform.position - offsettedPosition).normalized;

        rigidbody.AddForce(conjunctionLine * _repulsiveForce);
        player.ChangeColorTemporarily(Color.red, .5f);
        player.Health.Decrease(_contactDamage);
    }
}
