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
    /// Returns a random attack interval, based on the stage of Mantmare.
    /// </summary>
    private float AttackIntervalByStage
    {
        get
        {
            float attackInterval;

            int currentHealth = Health.CurrentHealth;
            int maxHealth = Health.MaxHealth;

            if (currentHealth > Mathf.RoundToInt(0.6f * maxHealth))
            {
                attackInterval = Random.Range(4, 6);
            } else if (currentHealth > Mathf.RoundToInt(0.4f * maxHealth))
            {
                attackInterval = Random.Range(2, 4);
            } else
            {
                attackInterval = Random.Range(1.5f, 3.5f);
            }

            return attackInterval;
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

    private Vector3 _randomWanderPosition;

    #region Rig

    [SerializeField]
    private TriggerCaster _leftArmTriggerCaster;

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

            int currentHealth = Health.CurrentHealth;
            int maxHealth = Health.MaxHealth;

            if (currentHealth > Mathf.RoundToInt(0.6f * maxHealth))
            {
                color = _normalLegsColor;
            }
            else if (currentHealth > Mathf.RoundToInt(0.4f * maxHealth))
            {
                color = _stage2LegsColor;
            }
            else
            {
                color = _stage3LegsColor;
            }

            return color;
        }
    }

    private Color ArmsColorByStage
    {
        get
        {
            Color color;

            int currentHealth = Health.CurrentHealth;
            int maxHealth = Health.MaxHealth;

            if (currentHealth > Mathf.RoundToInt(0.6f * maxHealth))
            {
                color = _normalArmsColor;
            }
            else if (currentHealth > Mathf.RoundToInt(0.4f * maxHealth))
            {
                color = _stage2ArmsColor;
            }
            else
            {
                color = _stage3ArmsColor;
            }

            return color;
        }
    }

    private Color BustColorByStage
    {
        get
        {
            Color color;

            int currentHealth = Health.CurrentHealth;
            int maxHealth = Health.MaxHealth;

            if (currentHealth > Mathf.RoundToInt(0.6f * maxHealth))
            {
                color = _normalBustColor;
            }
            else if (currentHealth > Mathf.RoundToInt(0.4f * maxHealth))
            {
                color = _stage2BustColor;
            }
            else
            {
                color = _stage3BustColor;
            }

            return color;
        }
    }

    private Color HeadColorByStage
    {
        get
        {
            Color color;

            int currentHealth = Health.CurrentHealth;
            int maxHealth = Health.MaxHealth;

            if (currentHealth > Mathf.RoundToInt(0.6f * maxHealth))
            {
                color = _normalHeadColor;
            }
            else if (currentHealth > Mathf.RoundToInt(0.4f * maxHealth))
            {
                color = _stage2HeadColor;
            }
            else
            {
                color = _stage3HeadColor;
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

        InvokeRepeating("UpdateRandomWanderPosition", 0, 1);
    }

    private void UpdateRandomWanderPosition()
    {
        GetRandomPointOnScreen(out _randomWanderPosition);
    }

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

        if (_isAttacking)
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

    #region Attack

    protected override IEnumerator Attack(PlayerController target)
    {
        int decidedPattern = Random.Range(3, 4);

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

        yield return new WaitForSeconds(1);

        AnimController.SetTrigger("EndAttack1");

        yield return new WaitForSeconds(1);

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

            yield return new WaitForSeconds(.5f);

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

            yield return new WaitUntil(() => !AnimController.GetCurrentAnimatorStateInfo(0).IsName("Attack2_end"));

            yield return new WaitForSeconds(1);

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

        yield return new WaitForSeconds(2);

        AnimController.SetTrigger("EndAttack3");

        yield return new WaitForSeconds(1);
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
            healthComponent.Decrease(Mathf.RoundToInt(Stats.Attack.CurrentValue * power / (statsComponent.Defence.CurrentValue * 2)));
        }
    }

    #endregion

    protected override IEnumerator Die()
    {
        Destroy(gameObject);

        yield break;
    }

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
        /*
        Vector2 conjunctionLine = (player.transform.position - transform.position).normalized;

        rigidbody.AddForce(conjunctionLine * _repulsiveForce);
        player.ChangeColorTemporarily(Color.red, .5f);
        player.Health.Decrease(_contactDamage);*/
    }
}
