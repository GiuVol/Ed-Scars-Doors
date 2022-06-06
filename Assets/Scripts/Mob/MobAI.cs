using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class MobAI : MonoBehaviour
{
    public float Speed
    { get; set; }
    public float NextWayPointDistance
    { get; set; }
    private  Path Path;
    private int CurrentWaypoint = 0;
    private bool EndOfPath = false;
    private Transform Target
    { get; set; }
    private Transform PlayerTarget;
    private Transform Mob;
    private Seeker Seeker;
    private Rigidbody2D Rb;
    public float Drag
    { get; set; }
    public const float StartUpdatePath = 0f;
    public const float RatingUpdatePath = .5f;
    private float _rangeToCheck;

    void Setup(float speed, float nextWayPointDistance, float drag, float rangeToCheck)
    {
        Speed = speed;
        NextWayPointDistance = nextWayPointDistance;
        Drag = drag;
        _rangeToCheck = rangeToCheck;
    }
    void Start()
    {
        if(gameObject.GetComponent<Seeker>() == null)
        {
            gameObject.AddComponent<Seeker>();
        }

        if (gameObject.GetComponent<Rigidbody2D>() == null)
        {
            gameObject.AddComponent<Rigidbody2D>();
        }

        gameObject.AddComponent<Transform>();
        Seeker = GetComponent<Seeker>();
        Rb = GetComponent<Rigidbody2D>();
        Mob = GetComponentInChildren<Transform>();
        InvokeRepeating("UpdatePath", StartUpdatePath, RatingUpdatePath);
        Rb.drag = Drag;
        Rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        if (Path == null)
            return;

        IsPathComplete();

        if (EndOfPath)
            return;

        Vector2 force = CalculateForce();
        Rb.AddForce(force);
        float distance = Vector2.Distance(Rb.position, Path.vectorPath[CurrentWaypoint]);
        UpgradeNextWayPoint(distance);
        InvertEnemy(force);


    }

    private void UpdatePath()
    {
        if (Seeker.IsDone())
            Seeker.StartPath(Rb.position, Target.position, OnPathComplete); 
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            Path = p;
            CurrentWaypoint = 0;
        }
    }

    public void IsPathComplete()
    {
        if (CurrentWaypoint >= Path.vectorPath.Count)
        {
            EndOfPath = true;
        }
        else
        {
            EndOfPath = false;
        }
    }

    public Vector2 CalculateForce()
    {
        Vector2 direction = ((Vector2)Path.vectorPath[CurrentWaypoint] - Rb.position).normalized;
        return (direction * Speed * Time.deltaTime);
    }

    public void InvertEnemy(Vector2 force)
    {
        if (force.x >= 0.01f)
        {
            Mob.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (force.x <= -0.01f)
        {
            Mob.localScale = new Vector3(1f, 1f, 1f);
        }
    }

    private void UpgradeNextWayPoint(float distance)
    {
        if (distance < NextWayPointDistance)
        {
            CurrentWaypoint++;
        }
    }

    public void changeTarget()
    {
        
    }

    public Transform GetTarget()
    {
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter = contactFilter.NoFilter();
        List<Collider2D> collidersFound = new List<Collider2D>();
        Physics2D.OverlapCircle(transform.position, _rangeToCheck, contactFilter, collidersFound);
        foreach (var item in collection)
        {
            if()
        }
        return;
    }
}
