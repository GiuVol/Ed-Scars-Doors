using System.Collections.Generic;
using UnityEngine;

public class PatrolPointsGroup : MonoBehaviour
{
    [SerializeField]
    /// <summary>
    /// The points that a mob could cover when it has not found the player.
    /// </summary>
    private List<Transform> _patrolPoints;

    /// <summary>
    /// Property to access in a controlled way to the patrol points.
    /// </summary>
    public List<Transform> PatrolPoints
    {
        get
        {
            if (_patrolPoints == null)
            {
                _patrolPoints = new List<Transform>();
            }

            return _patrolPoints;
        }

        #region Unused set

        /*

        set
        {
            if (_patrolPoints == null)
            {
                _patrolPoints = new List<Transform>();
            }

            _patrolPoints.Clear();

            if (value == null)
            {
                return;
            }

            foreach (Transform patrolPoint in value)
            {
                _patrolPoints.Add(patrolPoint);
            }
        }

        */

        #endregion
    }

    /// <summary>
    /// Returns the first patrol point in the list, or null, if it doesn't exist.
    /// </summary>
    public Transform FirstPatrolPoint
    {
        get
        {
            if (PatrolPoints.Count == 0)
            {
                return null;
            }

            return PatrolPoints[0];
        }
    }
    
    /// <summary>
    /// The mob who is covering this patrol points group.
    /// </summary>
    private GenericMob _subscriber;

    /// <summary>
    /// Property to access in a controlled way to the subscriber.
    /// </summary>
    public GenericMob Subscriber
    {
        get
        {
            if (_subscriber != null)
            {
                if (_subscriber.Health.CurrentHealth <= 0)
                {
                    _subscriber = null;
                }
            }

            return _subscriber;
        }

        set
        {
            if (Subscriber == null || value == null)
            {
                _subscriber = value;
            }
        }
    }

    /// <summary>
    /// Returns whether the group is being covered by a mob.
    /// </summary>
    public bool IsBusy
    {
        get
        {
            return Subscriber != null;
        }
    }
}
