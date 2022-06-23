using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static HealthComponent;

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

    /// <summary>
    /// The <c>_mobAI</c> that stores values and method related to the Artificial Intelligence of the mob
    /// to pathfinding
    /// </summary>
    protected MobAI _mobAI;

    /// <summary>
    /// The <c>_name</c> that stores the name of the mob
    /// </summary>
    [SerializeField]
    protected string _name;

    /// <summary>
    /// The <c>_attackInterval</c> that stores the interval time between Attack() calls
    /// </summary>
    [SerializeField]
    protected float _attackInterval;

    /// <summary>
    /// The <c>_attackPoint</c> that stores the values of the coordinates where the attack starts
    /// </summary>
    [SerializeField]
    public Transform _attackPoint;

    /// <summary>
    /// The <c>_attackRange</c> that stores the interval in which the player must be in order
    /// to be attacked by the crowd
    /// </summary>
    [SerializeField]
    protected float _attackRange;

    /// <summary>
    /// The <c>_timeLeftToAttack</c> that stores the value of the time remaining to invoke attack
    /// </summary>
    [SerializeField]
    protected float _timeLeftToAttack;

    /// <summary>
    /// The <c>Debug</c> if its value is true the function Start uses the function Setup whith standard value
    /// </summary>
    public bool Debug = false;

    /// <summary>
    /// Attribute <c>Speed</c>
    /// Represents the speed at which the mob moves
    /// </summary>
    [SerializeField]
    private float _speed;

    /// <summary>
    /// Attribute <c>NextWayPointDistance</c>
    /// Distance mob must have to the next waypoint
    /// </summary>
    [SerializeField]
    private float _nextWayPointDistance;

    /// <summary>
    /// Attribute <c>_rangeToCheck</c>
    /// Range in which the player must be in order to
    /// be hooked by the mob
    /// </summary>
    [SerializeField]
    private float _rangeToCheck;

    /// <summary>
    /// Attribute <c>_activate</c>
    /// Variable to activate/deactivate the MobAI
    /// </summary>
    [SerializeField]
    private bool _activeMobAI;

    /// <summary>
    /// Attribute <c>_isFlidier</c>
    /// Indicate if the mob is a flydier
    /// </summary>
    [SerializeField]
    private bool _canFly;

    /// <summary>
    /// Attribute <c>MaxHealth</c>
    /// Property that stores the max value that <c>CurrentHealth</c> can reach.
    /// </summary>
    [SerializeField]
    private int _maxHelath;




    /// <summary>
    /// Procedure <c>Setup</c> 
    /// It is used to set the components' value of:
    /// _mobAI, Helth, Stats, Status
    /// It is used to customize the components according
    /// to the mob being implemented.
    /// </summary>
    private void Setup()
    {
        SetupMobAI();
        SetupHealth();
        SetupStats();
        SetupStatus();
        SetName();
        SetupMob();
    }


    // Start is called before the first frame update
    void Start()
    {
        if (gameObject.GetComponent<StatusComponent>() == null)
        {
            gameObject.AddComponent<StatusComponent>();
        }

        if (gameObject.GetComponent<MobAI>() == null)
        {
            gameObject.AddComponent<MobAI>();
        }

        _mobAI = gameObject.GetComponent<MobAI>();
        Status = gameObject.GetComponent<StatusComponent>();

        if (Debug)
        {
            Setup();
        }
        else
        {
            _mobAI.Setup(_speed, _nextWayPointDistance, _rangeToCheck, _activeMobAI, _canFly);
            Health = new HealthComponent(_maxHelath, Die);
            SetupStats();
            SetupStatus();
        }

        _timeLeftToAttack = _attackInterval;
    }

    // Update is called once per frame
     void Update()
    {
        AttackTime(Attack);
    }

    /// <summary>
    /// Procedure <c>SetupHealth</c>
    /// It is used to set the Health's values
    /// It is used to customize the components according
    /// to the mob being implemented.
    /// </summary>
    abstract public void SetupHealth();

    /// <summary>
    /// Procedure <c>SetupStatus</c>
    /// It is used to set the Status' values
    /// It is used to customize the components according
    /// to the mob being implemented. 
    /// </summary>
    abstract public void SetupStatus();

    /// <summary>
    /// Procedure <c>SetupMobAI</c>
    /// It is used to set the _mobAI's values
    /// It is used to customize the components according
    /// to the mob being implemented.
    /// </summary>
    abstract public void SetupMobAI();

    /// <summary>
    /// Procedure <c>SetupStats</c>
    /// It is used to set the Stats' values
    /// It is used to customize the components according
    /// to the mob being implemented.
    /// </summary>
    abstract public void SetupStats();

    /// <summary>
    /// Procedure <c>SetupMob</c>
    /// It is used to set the another values
    /// To customize the components according
    /// to the mob being implemented.
    /// </summary>
    abstract protected void SetupMob();

    /// <summary>
    /// Procedure <c>Attack</c>
    /// It's used to implements the attack of mob
    /// </summary>
    /// <returns></returns>
    abstract public bool Attack();

    /// <summary>
    /// Procedure <c>SetName</c>
    /// It's used for set the name of mob
    /// </summary>
    abstract public void SetName();

    /// <summary>
    /// Procedure <c>Die</c>
    /// It's used to destroy mob and to perform additional instructions
    /// when mob die
    /// </summary>
    public abstract void Die();

    /// <summary>
    /// Procedure <c>AttackTime</c>
    /// This method handles attack calls through a timer
    /// </summary>
    /// <param name="Attack">Argument referring to the attack function</param>
    protected abstract void AttackTime(Func<bool> Attack);
}
