using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GenericMob : MonoBehaviour, IHealthable, IStatsable, IStatusable
{
    /// <summary>
    /// The <c>HealthComponent</c> that stores values and methods related to the health of the mob.
    /// </summary>
    public HealthComponent Health { get; protected set; }

    /// <summary>
    /// The <c>StatsComponent</c> that stores values and methods related to the stats of the mob.
    /// </summary>
    public StatsComponent Stats { get; protected set; }

    /// <summary>
    /// The <c>StatusComponent</c> that stores values and methods related to the status of the mob.
    /// </summary>
    public StatusComponent Status { get; protected set; }

    #region Parameters

    /// <summary>
    /// The name of the mob.
    /// </summary>
    [SerializeField]
    private string _name;
    
    /// <summary>
    /// The max value of the health, used to initialize the Health Component.
    /// </summary>
    [SerializeField]
    private int _maxHealthValue;
    
    /// <summary>
    /// The standard value of the attack.
    /// </summary>
    [SerializeField]
    private int _attackStandardValue;

    /// <summary>
    /// The min value that the attack can reach.
    /// </summary>
    [SerializeField]
    private int _attackMinValue;

    /// <summary>
    /// The max value that the attack can reach.
    /// </summary>
    [SerializeField]
    private int _attackMaxValue;

    /// <summary>
    /// The standard value of the defence.
    /// </summary>
    [SerializeField]
    private int _defenceStandardValue;

    /// <summary>
    /// The min value that the defence can reach.
    /// </summary>
    [SerializeField]
    private int _defenceMinValue;

    /// <summary>
    /// The max value that the defence can reach.
    /// </summary>
    [SerializeField]
    private int _defenceMaxValue;

    /// <summary>
    /// The blindness level threshold, after which the blinded status will be set.
    /// </summary>
    [SerializeField]
    private float _maxBlindnessLevel;

    /// <summary>
    /// The duration of the blindness.
    /// </summary>
    [SerializeField]
    private float _blindnessDuration;

    /// <summary>
    /// After how much the mob can be blinded again.
    /// </summary>
    [SerializeField]
    private float _blindnessCooldownTime;

    /// <summary>
    /// How much the mob resists to the attacks which inflict blindness.
    /// </summary>
    [SerializeField]
    private float _blindnessResistence;

    /// <summary>
    /// How fast the blindness level must decrease.
    /// </summary>
    [SerializeField]
    private float _blindnessLevelDecrementSpeed;

    /// <summary>
    /// The max time that the corrosion can last on the mob.
    /// </summary>
    [SerializeField]
    private float _maxCorrosionTime;

    /// <summary>
    /// How much the mob resists to the corrosion.
    /// </summary>
    [SerializeField]
    private float _corrosionResistence;

    /// <summary>
    /// How much the mob should wait to attack again after it has already attacked.
    /// </summary>
    [SerializeField]
    protected float _attackInterval;

    /// <summary>
    /// The distance that the mob must have from a target to hook it.
    /// </summary>
    [SerializeField]
    protected float _rangeToCheck;

    /// <summary>
    /// The distance that the mob must have from a target to attack it.
    /// </summary>
    [SerializeField]
    protected float _attackRange;
    
    /// <summary>
    /// Attribute <c>Speed</c>
    /// Represents the speed at which the mob moves.
    /// </summary>
    [SerializeField]
    protected float _speed;

    /// <summary>
    /// Stores whether the mob can fly or not.
    /// </summary>
    [SerializeField]
    private bool _canFly;

    #endregion

    #region Patrolling

    [Header("Patrolling")]
    /// <summary>
    /// The points that the mob could cover when it has not found the player.
    /// </summary>
    [SerializeField]
    private List<Transform> _patrolPoints;
    /// <summary>
    /// Specifies the way in which patrol points will be covered.
    /// </summary>
    [SerializeField]
    private bool _circular;

    /// <summary>
    /// Specifies the order in which the patrol points list is being checked.
    /// </summary>
    private bool _descending;
    /// <summary>
    /// Stores the current target patrol point.
    /// </summary>
    private int _currentPatrolPointIndex;
    
    /// <summary>
    /// Returns the first patrol point in the list, or null, if it doesn't exist.
    /// </summary>
    protected Transform FirstPatrolPoint
    {
        get
        {
            if (_patrolPoints == null)
            {
                return null;
            }

            if (_patrolPoints.Count == 0)
            {
                return null;
            }

            return _patrolPoints[0];
        }
    }

    /// <summary>
    /// Returns the current patrol point.
    /// </summary>
    protected Transform CurrentPatrolPoint
    {
        get
        {
            if (_patrolPoints == null)
            {
                return null;
            }

            if (_patrolPoints.Count == 0)
            {
                return null;
            }
            
            int index = Mathf.Clamp(_currentPatrolPointIndex, 0, _patrolPoints.Count - 1);

            return _patrolPoints[index];
        }
    }

    /// <summary>
    /// Method that increases the current patrol point index in a controlled manner.
    /// </summary>
    protected void IncreasePatrolPoint()
    {
        if (_patrolPoints == null)
        {
            return;
        }

        if (_patrolPoints.Count == 0)
        {
            return;
        }
        
        if (_descending)
        {
            _currentPatrolPointIndex--;
        }
        else
        {
            _currentPatrolPointIndex++;
        }

        if (_currentPatrolPointIndex >= _patrolPoints.Count)
        {
            if (_circular)
            {
                _currentPatrolPointIndex = 0;
            }
            else
            {
                _descending = true;
                _currentPatrolPointIndex = Mathf.Max(_patrolPoints.Count - 2, 0);
            }
        }

        if (_currentPatrolPointIndex < 0)
        {
            if (_circular)
            {

            }
            else
            {
                _descending = false;
                _currentPatrolPointIndex = Mathf.Min(_patrolPoints.Count - 1, 1);
            }
        }

        _currentPatrolPointIndex = Mathf.Clamp(_currentPatrolPointIndex, 0, _patrolPoints.Count - 1);
    }

    #endregion

    /// <summary>
    /// Returns the name of the mob.
    /// </summary>
    public virtual string Name
    {
        get
        {
            return _name;
        }
    }

    /// <summary>
    /// The <c>_mobAI</c> that stores values and method related to the Artificial Intelligence of the mob.
    /// </summary>
    protected MobAI _mobAI;

    /// <summary>
    /// The rigidbody attached to the character.
    /// </summary>
    protected Rigidbody2D _attachedRigidbody;

    /// <summary>
    /// Stores whether the mob is currently attacking.
    /// </summary>
    protected bool _isAttacking;

    /// <summary>
    /// Stores whether the mob can attack or not.
    /// </summary>
    protected bool _canAttack;
    
    protected void Start()
    {
        Setup();
    }

    #region Setup

    /// <summary>
    /// Procedure <c>Setup</c> 
    /// It is used to setup the generic mob.
    /// </summary>
    private void Setup()
    {
        if (gameObject.GetComponent<HealthComponent>() == null)
        {
            gameObject.AddComponent<HealthComponent>();
        }

        if (gameObject.GetComponent<StatsComponent>() == null)
        {
            gameObject.AddComponent<StatsComponent>();
        }

        if (gameObject.GetComponent<StatusComponent>() == null)
        {
            gameObject.AddComponent<StatusComponent>();
        }

        if (gameObject.GetComponent<MobAI>() == null)
        {
            gameObject.AddComponent<MobAI>();
        }

        if (gameObject.GetComponent<Rigidbody2D>() == null)
        {
            gameObject.AddComponent<Rigidbody2D>();
        }
        
        Health = gameObject.GetComponent<HealthComponent>();
        Stats = gameObject.GetComponent<StatsComponent>();
        Status = gameObject.GetComponent<StatusComponent>();

        _mobAI = gameObject.GetComponent<MobAI>();

        _attachedRigidbody = gameObject.GetComponent<Rigidbody2D>();

        _attachedRigidbody.drag = 3;
        _attachedRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

        if (_canFly)
        {
            _attachedRigidbody.gravityScale = 0;
        }

        _isAttacking = false;
        _canAttack = true;

        SetupHealth();
        SetupStats();
        SetupStatus();
    }

    /// <summary>
    /// Procedure <c>SetupHealth</c>
    /// It is used to setup the Health component.
    /// </summary>
    public virtual void SetupHealth()
    {
        if (Health != null)
        {
            Health.Setup(_maxHealthValue, Die);
        }
    }

    /// <summary>
    /// Procedure <c>SetupStats</c>
    /// It is used to setup the Stats component.
    /// </summary>
    public virtual void SetupStats()
    {
        if (Stats != null)
        {
            Stats.Setup(_attackStandardValue, _attackMinValue, _attackMaxValue, 
                        _defenceStandardValue, _defenceMinValue, _defenceMaxValue);
        }
    }

    /// <summary>
    /// Procedure <c>SetupStatus</c>
    /// It is used to setup the Status component.
    /// </summary>
    public virtual void SetupStatus()
    {
        if (Status != null)
        {
            Status.Setup(_maxBlindnessLevel, _blindnessDuration, _blindnessCooldownTime, 
                _blindnessResistence, _blindnessLevelDecrementSpeed, _maxCorrosionTime, _corrosionResistence);
        }
    }

    #endregion

    /// <summary>
    /// The IEnumerator that handles the attack of the mob.
    /// </summary>
    /// <param name="target">the player to attack</param>
    protected virtual IEnumerator HandleAttack(PlayerController target)
    {
        if (!_canAttack)
        {
            yield break;
        }

        _isAttacking = true;
        _canAttack = false;

        yield return StartCoroutine(Attack(target));

        _isAttacking = false;

        yield return new WaitForSeconds(_attackInterval);

        _canAttack = true;
    }

    /// <summary>
    /// The IEnumerator that specifies how the mob attacks.
    /// </summary>
    /// <param name="target">the player to attack</param>
    protected abstract IEnumerator Attack(PlayerController target);

    /// <summary>
    /// It's used to destroy the mob and perform other additional actions when he dies.
    /// </summary>
    public abstract void Die();
}
