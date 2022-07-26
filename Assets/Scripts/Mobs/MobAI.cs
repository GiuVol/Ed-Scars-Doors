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

            _target = value;
            CancelInvoke();

            if (value == null)
            {
                return;
            }

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
    }

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
    }

    #region Current Path Calculation

    #region fields for path calculation

    private Path _currentPath;
    private int _currentWaypoint = 0;
    private float _nextWaypointDistance = .5f;

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
        GraphNode startNode = AstarPath.active.GetNearest(startPosition).node;
        GraphNode endNode = AstarPath.active.GetNearest(endPosition).node;

        return endNode.Walkable && PathUtilities.IsPathPossible(startNode, endNode);
    }

    public Vector3 GetNearestReachablePosition(Vector3 desiredPosition)
    {
        NNConstraint constraint = NNConstraint.None;

        constraint.constrainWalkability = true;
        constraint.walkable = true;

        GraphNode reachableNode = AstarPath.active.GetNearest(desiredPosition, constraint).node;

        return (Vector3) reachableNode.position;
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
}
