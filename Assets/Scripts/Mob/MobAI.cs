using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class MobAI : MonoBehaviour
{
    public bool _debug = false;
    /// <summary>
    /// Attribute <c>Speed</c>
    /// Represents the speed at which the mob moves
    /// </summary>
    private float _speed;

    /// <summary>
    /// Attribute <c>NextWayPointDistance</c>
    /// Distance mob must have to the next waypoint
    /// </summary>
    private float _nextWayPointDistance;

    /// <summary>
    /// Attribute <c>_path</c>
    /// Contains the path of the mob
    /// </summary>
    private Path _path;

    /// <summary>
    /// Attribute <c>_currentWaypoint</c>
    /// The current waypoint
    /// </summary>
    private int _currentWaypoint = 0;

    /// <summary>
    /// Attribute <c>_endOfPath</c> 
    /// End to path
    /// </summary>
    private bool _endOfPath = false;

    /// <summary>
    /// Property <c>Target</c>
    /// The target that the mob is chasing at the moment
    /// </summary>
    public Transform Target
    { get; private set; }

    /// <summary>
    /// Attribute <c>_casualTarget</c>
    /// Value to assign to the target when the enemy is too far away
    /// </summary>
    private Transform _casualTarget;

    /// <summary>
    /// Static attribute <c>_playerTarget</c>
    /// Reference to the player's position
    /// </summary>
    private static Transform PlayerTarget;

    /// <summary>
    /// Attribute <c>_mob</c>
    /// Reference to the mob's position
    /// </summary>
    private Transform _mob;

    /// <summary>
    /// Attribute <c>_seeker</c>
    /// Handles path calls for a single unit
    /// </summary>
    private Seeker _seeker;

    /// <summary>
    /// Attribute <c>_rb</c>
    /// Used for mob physics
    /// </summary>
    public Rigidbody2D _rb;

    /// <summary>
    /// Const <c>StartUpdatePath</c>
    /// How long to wait to call the UpdatePath method after
    /// calling InvokeRepeating
    /// </summary>
    public const float StartUpdatePath = 0f;

    /// <summary>
    /// Const <c>RatingUpdatePath</c>
    /// Time interval between UpdatePath calls
    /// </summary>
    public const float RatingUpdatePath = .5f;

    /// <summary>
    /// Attribute <c>_rangeToCheck</c>
    /// Range in which the player must be in order to
    /// be hooked by the mob
    /// </summary>
    private float _rangeToCheck;

    /// <summary>
    /// Attribute <c>_activate</c>
    /// Variable to activate/deactivate the MobAI
    /// </summary>
    private bool _activate;

    /// <summary>
    /// Const <c>MinCasualRange</c>
    /// Constant used for generating the position of the random target.
    /// It represents the smallest value on the x-axis that the casual target can reach
    /// </summary>
    const float MinCasualRangeX = 5f;

    /// <summary>
    /// Const <c>MaxCasualRange</c>
    /// Constant used for generating the position of the random target.
    /// It represents the largest value on the x-axis that the casual target can reach
    /// </summary>
    const float MaxCasualRangeX = 10f;

    /// <summary>
    /// Const <c>MaxCasualRangeY</c>
    /// Constant used for generating the position of the random target.
    /// It represents the largest value on the y-axis that the casual target can reach
    /// </summary>
    const float MaxCasualRangeY = 5f;

    /// <summary>
    /// Property <c>IsHookedPlayer</c>
    /// Return true if mob hooked player
    /// </summary>
    public bool IsHookedPlayer
    {
        get
        {
            return (Target.position.x == PlayerTarget.position.x);
        }
    }

    /// <summary>
    /// Static attribute <c>XStartGraph</c>
    /// Attribute that stores the value of coordinate of the x of the beginning of the graph
    /// </summary>
    private static float XStartGraph;

    /// <summary>
    /// Static attribute <c>XEndGraph</c>
    /// Attribute that stores the value of coordinate of the x of the ending of the graph
    /// </summary>
    private static float XEndGraph;

    /// <summary>
    /// Attribute <c>_nextCasualPositionDirection</c>
    /// Attribute that marks the next causal direction x-axis of the mob
    /// if value is true the direction is right, else the direction is left
    /// </summary>
    private bool _nextCasualPositionDirectionX = true;

    /// <summary>
    /// Attribute <c>_nextCasualPositionDirectionY</c>
    /// Attribute that marks the next causal direction y-axis of the mob
    /// if the value is true the direction increment y-value, else the y-value is equal to the _yMob
    /// </summary>
    private bool _nextCasualPositionDirectionY = true;

    /// <summary>
    /// Attribute <c>_timeToChangeTarget</c>
    /// Strores the remaining time to change a casual target with another casual target
    /// </summary>
    private float _timeToChangeTarget;

    /// <summary>
    /// Attribute <c>_yMob</c>
    /// Stores the value of y-axis of initial position of mob
    /// </summary>
    private float _yMob;

    /// <summary>
    /// Attribute <c>_isFlidier</c>
    /// Indicate if the mob is a flydier
    /// </summary>
    private bool _isFlydier;

    /// <summary>
    /// Const attribute <c>ChangeAttackInterval</c>
    /// Stores the interval time between change casual target
    /// </summary>
    private const float ChangeTargetInterval = 15f;

    private void SetupDebug()
    {
        _speed = 200f;
        _nextWayPointDistance = 3f;
        _rb.drag = 1.5f;
        _rangeToCheck = 5f;
        _activate = true;
    }
    /// <summary>
    /// Method <c>GetMobTransform</c>
    /// return the transform of mob
    /// </summary>
    /// <returns></returns>
    public Transform GetMobTransform()
    {
        return _mob;
    }

    /// <summary>
    /// Static method <c>SetExtremeGraph</c>
    /// It's used to set the value of:
    /// XStartGraph,XEndGraph
    /// </summary>
    /// <param name="xCenter">The center of grath</param>
    /// <param name="width">The width dimension of grath</param>
    public static void SetExtremeGraph(float xCenter, float width)
    {
        XStartGraph = xCenter - width / 2;
        XEndGraph = xCenter + width / 2;
    }

    /// <summary>
    /// Static method <c>SetPlayerTarget</c>
    ///  It's used to set the value of PlayerTarget
    /// </summary>
    /// <param name="playerTarget">Player's transform </param>
    public static void SetPlayerTarget(Transform playerTarget)
    {
        PlayerTarget = playerTarget;
    }

    /// <summary>
    /// Static function <c>GetPlayerTarget</c>
    /// Return the player transform
    /// </summary>
    /// <returns></returns>
    public static Transform GetPlayerTarget()
    {
        return PlayerTarget;
    }

    /// <summary>
    /// Procedure <c>Setup</c>
    /// It is used to modify the values of:
    /// Speed, NextWayPointDistance, Drag,
    /// _rangeToCheck, _activate.
    /// It is used to customize the MobAI according
    /// to the mob being implemented.
    /// </summary>
    /// <param name="speed">Value to assigned to Speed</param>
    /// <param name="nextWayPointDistance">Value to assign to NextWayPointDistance</param>
    /// <param name="rangeToCheck">Value to assign to _rangeToCheck</param>
    /// <param name="activate">Value to assign to _activate</param>
    public void Setup(float speed, float nextWayPointDistance, float rangeToCheck, bool activate, bool isFlydier)
    {
        _speed = speed;
        _nextWayPointDistance = nextWayPointDistance;
        _rangeToCheck = rangeToCheck;
        _activate = activate;
        _isFlydier = isFlydier;
    }

    void Start()
    {
        GameObject go = new GameObject();
        if (gameObject.GetComponent<Seeker>() == null)
        {
            gameObject.AddComponent<Seeker>();
        }

        if (gameObject.GetComponent<Rigidbody2D>() == null)
        {
            gameObject.AddComponent<Rigidbody2D>();
        }

        gameObject.AddComponent<Transform>();
        _seeker = GetComponent<Seeker>();
        _rb = GetComponent<Rigidbody2D>();
        _mob = GetComponent<Transform>();
        _rb.drag = 1.5f;
        /*
         *InvokeRepeating starts calling the UpdatePath method,
         *after a period of time equal to the value of StartUpdatePath has passed,
         *with a rating equal to the value of RatingUpdatePath
         */
        InvokeRepeating("UpdatePath", StartUpdatePath, RatingUpdatePath);
        _rb.freezeRotation = true;
        _casualTarget = go.transform;
        Target = _casualTarget;
        _rb.gravityScale = 0;
        _timeToChangeTarget = ChangeTargetInterval;
        _yMob = _mob.position.y;
        _casualTarget.position = GenerateCasualPosition();
        /*if (_debug)
        {
            SetupDebug();
        }*/
    }

    void DebugChangeTarget()
    {
        float distance;
        bool controlPlayer = false;

        if (PlayerTarget != null)
        {
            distance = Vector2.Distance(PlayerTarget.position, _mob.position);
            controlPlayer = (distance <= _rangeToCheck);
        }

        if (controlPlayer)
        {
            Target = PlayerTarget;
        }
    }

    void FixedUpdate()
    {
        if (_activate)
        {
            if (_path == null)
                return;

            IsPathComplete();

            if (_endOfPath)
                return;

            Vector2 force = CalculateForce();
            _rb.AddForce(force);
            float distance = Vector2.Distance(_rb.position, _path.vectorPath[_currentWaypoint]);
            UpgradeNextWayPoint(distance);
            InvertEnemy(force);
            ChangeTarget();
        }
    }

    /// <summary>
    /// Procedure <c>UpdatePath</c>
    /// Update the path of the mob
    /// </summary>
    private void UpdatePath()
    {
        if (_seeker.IsDone())
            /*
             * This function is used to generate the path to be taken by the mob.
             * To prevent the path from being generated, the OnPathComplete procedure is called each time
             */
            _seeker.StartPath(_rb.position, Target.position, OnPathComplete);
    }

    /// <summary>
    /// It is called at the end of the path generation.
    /// Check if the path has any errors, if it has none use that path and start from the first waypoint of the path
    /// </summary>
    /// <param name="p">Path to check if it is correct</param>
    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            _path = p;
            _currentWaypoint = 0;
        }
    }

    /// <summary>
    /// Procedure <c>IsPathComplete</c>
    /// Check if the route is over
    /// </summary>
    public void IsPathComplete()
    {
        /*I check if I have reached the end of the path by checking if the
         * currentWaypoint value is greater than the number of waypoints
         */
        if (_currentWaypoint >= _path.vectorPath.Count)
        {
            _endOfPath = true;
        }
        else
        {
            _endOfPath = false;
        }
    }

    /// <summary>
    /// Function <c>CalculateForce</c>
    /// calculates the force to be applied to the mob
    /// </summary>
    /// <returns></returns>
    public Vector2 CalculateForce()
    {
        //I find the direction of the next waypoint
        Vector2 direction = ((Vector2)_path.vectorPath[_currentWaypoint] - _rb.position).normalized;
        return (direction * _speed * Time.deltaTime);
    }

    /// <summary>
    /// Procedure <c>InverEnemy</c>
    /// Reverse the mob's body based on the direction
    /// </summary>
    /// <param name="force"></param>
    public void InvertEnemy(Vector2 force)
    {
        if (force.x >= 0.01f)
        {
            _mob.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (force.x <= -0.01f)
        {
            _mob.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    /// <summary>
    /// Check if you need to upgrade the currentWaypoint, if so, increase it by one
    /// </summary>
    /// <param name="distance"></param>
    private void UpgradeNextWayPoint(float distance)
    {
        if (distance < _nextWayPointDistance)
        {
            _currentWaypoint++;
        }
    }

    /// <summary>
    /// Check if the enemy is in his range, if so, target the player.
    /// Otherwise it checks if it is already following a random point,
    /// if it has arrived it generates a new one
    /// </summary>
    public void ChangeTarget()
    {
        float distance;
        bool controlPlayer = false;
        if (PlayerTarget != null) // Check if a playerController has been created in the scene
        {
            distance = Vector2.Distance(PlayerTarget.position, _mob.position);
            controlPlayer = (distance <= _rangeToCheck);
        }

        if (controlPlayer)
        {
            if (_isFlydier)
            {
               Target = PlayerTarget;
            }
            else
            {
                _casualTarget.position = new Vector3(PlayerTarget.position.x, _yMob, 0);
            }
           
        }
        else
        {
            if (IsHookedPlayer) //This control is used when the player is initially hooked and then moves away
            {
                Target = _casualTarget;
                Target.position = GenerateCasualPosition();
            }
            else
            {
                if (_timeToChangeTarget < 0f)
                {
                    Target.position = GenerateCasualPosition();
                    _timeToChangeTarget = ChangeTargetInterval;
                }
                else
                {
                    _timeToChangeTarget -= Time.deltaTime;
                    distance = Vector2.Distance(Target.position, _mob.position);
                    if (distance <= 1f) // We check if the mob has reached the current target and then we need to create a new one
                        Target.position = GenerateCasualPosition();
                }
                
            }

        }
    }

    /// <summary>
    /// Function <c>GenerateCasualPosition</c>
    /// Generate a random position
    /// </summary>
    /// <returns></returns>
    private Vector3 GenerateCasualPosition()
    {
        float casualY;
        float casualX;
        if (_nextCasualPositionDirectionX) // If _nextCasualPositionDirection is equal to true then go to the right otherwise go to the left
        {
            _nextCasualPositionDirectionX = false; // The next target will be on the left
            casualX = Random.Range(_mob.position.x + MinCasualRangeX, MaxCasualRangeX + _mob.position.x);
            // Generate a random dot to the right of the mob
        }
        else
        {
            _nextCasualPositionDirectionX = true; // The next target will be on the right
            casualX = Random.Range(_mob.position.x - MaxCasualRangeX, _mob.position.x - MinCasualRangeX);
            // Generate a random dot to the left of the mob
        }


        if (_isFlydier)
        {
            if (_nextCasualPositionDirectionY)
            {
                casualY = Random.Range(MaxCasualRangeY+_mob.position.y, _mob.position.y);
                _nextCasualPositionDirectionY = false;
            }
            else
            {
                casualY = _yMob;
                _nextCasualPositionDirectionY = true;
            }
        }
        else
        {
            casualY = _yMob;
        }
        return ControlGenerateCasualPosition(new Vector3(casualX, casualY, 0));
    }


    /// <summary>
    /// Function <c>ControlGenerateCasualPosition</c>
    /// Check if the generated position is within the bounds of the graph
    /// </summary>
    /// <param name="casualPosition">Generated position</param>
    /// <returns>If position is correct return the him else it generated a new position</returns>
    private Vector3 ControlGenerateCasualPosition(Vector3 casualPosition)
    {
        if (casualPosition.x <= XStartGraph || casualPosition.x >= XEndGraph)
        {
            return GenerateCasualPosition();
        }
        else
        {
            return casualPosition;
        }
    }

    /// <summary>
    /// Procedure <c>ActiveMobAI</c>
    /// Activate mobAI
    /// </summary>
    public void ActiveMobAI()
    {
        _activate = true;
    }

    /// <summary>
    /// Procedure <c>DisableMobAI</c>
    /// Disable MobAI
    /// </summary>
    public void DisableMobAI()
    {
        _activate = false;
    }
}
