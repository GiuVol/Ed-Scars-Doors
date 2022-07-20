using System;
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
    /// 
    /// </summary>
    [SerializeField]
    public Transform _attackPoint;

    /// <summary>
    /// Attribute <c>Speed</c>
    /// Represents the speed at which the mob moves.
    /// </summary>
    [SerializeField]
    private float _speed;

    /// <summary>
    /// Stores whether the mob can fly or not.
    /// </summary>
    [SerializeField]
    private bool _canFly;
    
    #endregion

    public virtual string Name
    {
        get
        {
            return _name;
        }
    }

    /// <summary>
    /// The <c>_mobAI</c> that stores values and method related to the Artificial Intelligence of the mob.
    /// to pathfinding
    /// </summary>
    protected MobAI _mobAI;
    
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

        Health = gameObject.GetComponent<HealthComponent>();
        Stats = gameObject.GetComponent<StatsComponent>();
        Status = gameObject.GetComponent<StatusComponent>();

        _mobAI = gameObject.GetComponent<MobAI>();

        SetupHealth();
        SetupStats();
        SetupStatus();
        SetupMob();
        SetupAI();
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

    /// <summary>
    /// Procedure <c>SetupMob</c>
    /// It is used to set the another values
    /// To customize the components according
    /// to the mob being implemented.
    /// </summary>
    abstract protected void SetupMob();
    
    /// <summary>
    /// Procedure <c>SetupMobAI</c>
    /// It is used to set the _mobAI's values
    /// It is used to customize the components according
    /// to the mob being implemented.
    /// </summary>
    public abstract void SetupAI();

    #endregion

    /// <summary>
    /// The procedure that handles the attack of the mob.
    /// </summary>
    /// <returns></returns>
    public abstract bool Attack();

    /// <summary>
    /// It's used to destroy the mob and perform other additional actions when he dies.
    /// </summary>
    public abstract void Die();
}
