using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

/// <summary>
/// This component is used as the brain of the mob.
/// </summary>
public class MobAI : MonoBehaviour
{
    /// <summary>
    /// The Seeker component; It's needed to find the path that leads to a target.
    /// </summary>
    private Seeker _seeker;

    /// <summary>
    /// Stores the current target that the mob should reach.
    /// </summary>
    private Transform _target;

    /// <summary>
    /// Provides access to the target in a controlled manner.
    /// </summary>
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

            _target = value;
            CancelInvoke();

            if (value == null)
            {
                return;
            }

            InvokeRepeating("UpdatePath", 0, .5f);
        }
    }

    /// <summary>
    /// Stores the direction that the mob should follow in order to reach the target.
    /// </summary>
    private Vector2 _desiredDirection;

    /// <summary>
    /// Provides public access to the desired direction.
    /// </summary>
    public Vector2 DesiredDirection
    {
        get
        {
            return _desiredDirection;
        }
    }

    /// <summary>
    /// The component needed to smooth the path to follow.
    /// </summary>
    private SimpleSmoothModifier _smoothModifier;

    private void Start()
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

        _currentWaypoint = 0;
        _nextWaypointDistance = 1.5f;
    }

    #region Current Path Calculation

    #region fields for path calculation

    /// <summary>
    /// The path that the mob is currently following.
    /// </summary>
    private Path _currentPath;

    /// <summary>
    /// The index of the point of the path towards which the mob is currently heading to.
    /// </summary>
    private int _currentWaypoint;

    /// <summary>
    /// The distance needed for the mob to head towards the next waypoint.
    /// </summary>
    private float _nextWaypointDistance;

    #endregion
    
    /// <summary>
    /// This method updates the path to the target.
    /// </summary>
    void UpdatePath()
    {
        if (Target == null)
        {
            _currentPath = null;
            return;
        }

        _seeker.StartPath(transform.position, Target.position, OnPathComplete);
    }

    /// <summary>
    /// This is a callback method, called when a new path is found.
    /// </summary>
    /// <param name="p"></param>
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

    /// <summary>
    /// Calculates a path, if it exists, that connects two points. 
    /// </summary>
    /// <param name="startPosition">The initial position</param>
    /// <param name="endPosition">The final position</param>
    /// <param name="waypoints">The list that will be filled with the points of the path</param>
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

    /// <summary>
    /// Returns whether a path that connects two points exists or not.
    /// </summary>
    /// <param name="startPosition">The initial position</param>
    /// <param name="endPosition">The final position</param>
    /// <returns>Returns whether a path exists or not</returns>
    public bool IsPositionReachable(Vector2 startPosition, Vector2 endPosition)
    {
        GraphNode startNode = AstarPath.active.GetNearest(startPosition).node;
        GraphNode endNode = AstarPath.active.GetNearest(endPosition).node;

        return startNode.Walkable && endNode.Walkable && PathUtilities.IsPathPossible(startNode, endNode);
    }

    /// <summary>
    /// Returns the reachable position nearest to the desired position.
    /// </summary>
    /// <param name="desiredPosition">The desired position</param>
    /// <returns>A reachable position, according the desired one</returns>
    public Vector3 GetNearestReachablePosition(Vector3 desiredPosition)
    {
        NNConstraint constraint = NNConstraint.None;
        constraint.constrainWalkability = true;
        constraint.walkable = true;

        GraphNode reachableNode = AstarPath.active.GetNearest(desiredPosition, constraint).node;

        return (Vector3) reachableNode.position;
    }

    /// <summary>
    /// Method that searches for the player in an area.
    /// </summary>
    /// <param name="center">The center of the area to check</param>
    /// <param name="radiusToCheck">The radius of the area</param>
    /// <returns>A player, if it is found, null otherwise</returns>
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
}
