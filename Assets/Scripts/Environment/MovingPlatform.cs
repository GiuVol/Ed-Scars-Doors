using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour, IPatroller
{
    private const float NextWayPointDistance = .3f;

    [SerializeField]
    private float _movementSpeed;

    #region Patrolling

    [Header("Patrolling")]
    /// <summary>
    /// The group of patrol points that this mob will be able to cover.
    /// </summary>
    [SerializeField]
    private PatrolPointsGroup _patrolPointsGroup;
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
    /// Property to access to the patrol points group in a controlled way.
    /// </summary>
    protected PatrolPointsGroup PPGroup
    {
        get
        {
            return _patrolPointsGroup;
        }

        set
        {
            if (value != null)
            {
                if (!value.IsBusy)
                {
                    _patrolPointsGroup = value;
                    _patrolPointsGroup.Subscriber = this;
                    return;
                }
            }

            _patrolPointsGroup = null;
        }
    }

    /// <summary>
    /// Property that returns whether the mob can patrol or not.
    /// </summary>
    protected bool CanPatrol
    {
        get
        {
            bool canPatrol = false;

            if (PPGroup != null)
            {
                canPatrol = (PPGroup.FirstPatrolPoint != null);
            }

            return canPatrol;
        }
    }

    /// <summary>
    /// Property that returns the patrol points of the current patrol points group.
    /// </summary>
    protected List<Transform> PatrolPoints
    {
        get
        {
            if (_patrolPointsGroup == null)
            {
                return null;
            }

            return _patrolPointsGroup.PatrolPoints;
        }
    }

    /// <summary>
    /// Returns the current patrol point.
    /// </summary>
    protected Transform CurrentPatrolPoint
    {
        get
        {
            if (PatrolPoints == null)
            {
                return null;
            }

            if (PatrolPoints.Count == 0)
            {
                return null;
            }

            int index = Mathf.Clamp(_currentPatrolPointIndex, 0, PatrolPoints.Count - 1);

            return PatrolPoints[index];
        }
    }

    /// <summary>
    /// Method that increases the current patrol point index in a controlled manner.
    /// </summary>
    protected void IncreasePatrolPoint()
    {
        if (PatrolPoints == null)
        {
            return;
        }

        if (PatrolPoints.Count == 0)
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

        if (_currentPatrolPointIndex >= PatrolPoints.Count)
        {
            if (_circular)
            {
                _currentPatrolPointIndex = 0;
            }
            else
            {
                _descending = true;
                _currentPatrolPointIndex = Mathf.Max(PatrolPoints.Count - 2, 0);
            }
        }

        if (_currentPatrolPointIndex < 0)
        {
            if (_circular)
            {
                _descending = false;
                _currentPatrolPointIndex = 0;
            }
            else
            {
                _descending = false;
                _currentPatrolPointIndex = Mathf.Min(PatrolPoints.Count - 1, 1);
            }
        }

        _currentPatrolPointIndex = Mathf.Clamp(_currentPatrolPointIndex, 0, PatrolPoints.Count - 1);
    }

    #endregion

    private Rigidbody2D AttachedRigidbody { get; set; }

    void Start()
    {
        AttachedRigidbody = GetComponent<Rigidbody2D>();
        AttachedRigidbody.gravityScale = 0;

        PPGroup = _patrolPointsGroup;
    }
    
    void FixedUpdate()
    {
        if (AttachedRigidbody != null && PatrolPoints != null)
        {
            float distance = Vector2.Distance(transform.position, CurrentPatrolPoint.position);

            Vector3 moveDirection;

            if (distance > 1)
            {
                moveDirection = (CurrentPatrolPoint.position - transform.position).normalized;
            } else
            {
                moveDirection = (CurrentPatrolPoint.position - transform.position);
            }

            moveDirection.z = 0;

            AttachedRigidbody.AddForce(moveDirection * _movementSpeed * AttachedRigidbody.mass, ForceMode2D.Force);

            if (distance < NextWayPointDistance)
            {
                AttachedRigidbody.velocity = Vector2.zero;
                IncreasePatrolPoint();
            }
        }
    }
}
