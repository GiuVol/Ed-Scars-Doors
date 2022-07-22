using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MobAI : MonoBehaviour
{
    private Seeker _seeker;

    public Seeker AttachedSeeker
    {
        get
        {
            return _seeker;
        }
    }
    
    private Transform _target;

    public Transform Target
    {
        get
        {
            return _target;
        }

        set
        {
            if (_target == value)
            {
                return;
            }

            CancelInvoke();

            if (value == null)
            {
                return;
            }

            _target = value;
            InvokeRepeating("UpdatePath", 0, .5f);
        }
    }

    private Vector2 _desiredDirection;

    public Vector2 DesiredDirection
    {
        get
        {
            return _desiredDirection;
        }

        private set
        {
            _desiredDirection = value;
        }
    }

    private SimpleSmoothModifier _smoothModifier;

    private void OnEnable()
    {
        if (gameObject.GetComponent<Seeker>() == null)
        {
            gameObject.AddComponent<Seeker>();
        }

        if (gameObject.GetComponent<SimpleSmoothModifier>() == null)
        {
            gameObject.AddComponent<SimpleSmoothModifier>();
        }

        _seeker = gameObject.GetComponent<Seeker>();
        _smoothModifier = gameObject.GetComponent<SimpleSmoothModifier>();
    }

    #region Current Path Calculation

    #region fields for path calculation

    private Path _currentPath;
    private int _currentWaypoint = 0;
    private float _nextWaypointDistance = 3f;

    #endregion
    
    void UpdatePath()
    {
        if (Target == null)
        {
            _currentPath = null;
            return;
        }

        _seeker.StartPath(transform.position, Target.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            _currentPath = p;
            _currentWaypoint = 0;
        } else
        {
            _currentPath = null;
        }
    }

    private void FixedUpdate()
    {
        if (Target == null || _currentPath == null)
        {
            _desiredDirection = Vector3.zero;
            return;
        }

        _currentWaypoint = Mathf.Max(_currentWaypoint, 0);

        if (_currentWaypoint >= _currentPath.vectorPath.Count)
        {
            _desiredDirection = Vector3.zero;
            return;
        }

        _desiredDirection = (_currentPath.vectorPath[_currentWaypoint] - transform.position).normalized;

        float distance = Vector2.Distance(transform.position, _currentPath.vectorPath[_currentWaypoint]);

        if (distance < _nextWaypointDistance)
        {
            _currentWaypoint++;
        }
    }

    #endregion

    public IEnumerator GetPath(Vector2 startPosition, Vector2 endPosition, List<Vector3> waypoints)
    {
        if (waypoints == null)
        {
            yield break;
        }

        waypoints.Clear();

        bool operationCompleted = false;

        _seeker.StartPath(startPosition,
                          endPosition,
                          (Path p) =>
                          {
                              operationCompleted = true;

                              if (!p.error)
                              {
                                  foreach (Vector3 waypoint in p.vectorPath)
                                  {
                                      waypoints.Add(waypoint);
                                  }
                              }
                          }
                          );

        yield return new WaitUntil(() => operationCompleted);
    }

    public bool IsPositionReachable(Vector2 startPosition, Vector2 endPosition)
    {
        GraphNode mobNode = AstarPath.active.GetNearest(startPosition).node;
        GraphNode targetNode = AstarPath.active.GetNearest(endPosition).node;

        return targetNode.Walkable && PathUtilities.IsPathPossible(mobNode, targetNode);
    }

    public PlayerController FindPlayerInRadius(Vector2 center, float radiusToCheck)
    {
        Collider2D[] collidersWithinRadius = Physics2D.OverlapCircleAll(center, radiusToCheck);

        PlayerController player = null;

        foreach (Collider2D collider in collidersWithinRadius)
        {
            player = collider.gameObject.GetComponent<PlayerController>();

            if (player == null)
            {
                player = collider.gameObject.GetComponentInParent<PlayerController>();
            }

            if (player == null)
            {
                player = collider.gameObject.GetComponentInChildren<PlayerController>();
            }

            if (player != null)
            {
                break;
            }
        }

        return player;
    }

    #region ToKeep

    /*

    /// <summary>
    /// Generates a random position;
    /// </summary>
    /// <returns></returns>
    private Vector3 GenerateRandomPosition()
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


        if (_canFly)
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
        return CheckRandomPositionCorrectness(new Vector3(casualX, casualY, 0));
    }


    /// <summary>
    /// Checks if the generated position is within the bounds of the graph.
    /// </summary>
    /// <param name="randomPosition">The generated position to check</param>
    /// <returns>
    /// Returns the position given in input, if it's correct, otherwise it returns a new position 
    /// randomly generated.
    /// </returns>
    private Vector3 CheckRandomPositionCorrectness(Vector3 randomPosition)
    {
        if (randomPosition.x <= XStartGraph || randomPosition.x >= XEndGraph)
        {
            return GenerateRandomPosition();
        }
        else
        {
            return randomPosition;
        }
    }

    */

    #endregion
}
