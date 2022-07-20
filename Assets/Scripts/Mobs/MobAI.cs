using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class MobAI : MonoBehaviour
{
    public Transform target;

    public float speed = 50f;
    public float nextWaypointDistance = 3f;

    public Seeker _seeker;
    public Rigidbody2D _attachedRigidbody;

    Path currentPath;
    int currentWaypoint = 0;
    bool reachedEndOfPath;

    private void OnEnable()
    {
        if (gameObject.GetComponent<Seeker>() == null)
        {
            gameObject.AddComponent<Seeker>();
        }

        if (gameObject.GetComponent<Rigidbody2D>() == null)
        {
            gameObject.AddComponent<Rigidbody2D>();
        }

        _seeker = gameObject.GetComponent<Seeker>();
        _attachedRigidbody = gameObject.GetComponent<Rigidbody2D>();

        _attachedRigidbody.drag = 3;
        _attachedRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

        MobAI ai = GetComponent<MobAI>();

        if (ai != null)
        {
            if (ai.target == null)
            {
                ai.target = GameObject.FindWithTag("Player").transform;
            }
        }

        InvokeRepeating("UpdatePath", 0, .5f);
    }

    void UpdatePath()
    {
        _seeker.StartPath(_attachedRigidbody.position, target.position, OnPathComplete);
    }

    void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            currentPath = p;
            currentWaypoint = 0;
        }
    }

    private void FixedUpdate()
    {
        if (currentPath == null)
        {
            return;
        }

        if (currentWaypoint >= currentPath.vectorPath.Count)
        {
            reachedEndOfPath = true;
            return;
        } else
        {
            reachedEndOfPath = false;
        }

        Vector2 direction = 
            ((Vector2)currentPath.vectorPath[currentWaypoint] - _attachedRigidbody.position).normalized;
        Vector2 force = direction * speed * Time.deltaTime;

        _attachedRigidbody.AddForce(direction * speed);

        float distance = 
            Vector2.Distance(_attachedRigidbody.position, currentPath.vectorPath[currentWaypoint]);

        if (distance <= nextWaypointDistance)
        {
            currentWaypoint++;
        }

        if (_attachedRigidbody.velocity.x >= .01f)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        } else if (_attachedRigidbody.velocity.x <= -.01f)
        {
            transform.rotation = Quaternion.Euler(0, -180, 0);
        }
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
