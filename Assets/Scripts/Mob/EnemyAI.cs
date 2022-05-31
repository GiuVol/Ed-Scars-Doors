using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;
public class EnemyAI : MonoBehaviour
{
    private float Speed
    { get; set; }
    private float NextWayPointDistance
    { get; set; }
    private  Path Path;
    private int CurrentWaypoint = 0;
    private bool EndOfPath = false;
    private Transform Target
    { get; set; }

    private Transform Enemy;
    private Seeker Seeker;
    private Rigidbody2D Rb;
    private float Drag
    { get; set; }
    public const float StartUpdatePath = 0f;
    public const float RatingUpdatePath = .5f;


    void Setup(float speed, float nextWayPointDistance, float drag)
    {
        Speed = speed;
        NextWayPointDistance = nextWayPointDistance;
        Drag = drag;
    }
    void Start()
    {
        Seeker = GetComponent<Seeker>();
        Rb = GetComponent<Rigidbody2D>();
        Enemy = GetComponentInChildren<Transform>();
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
            Enemy.localScale = new Vector3(-1f, 1f, 1f);
        }
        else if (force.x <= -0.01f)
        {
            Enemy.localScale = new Vector3(1f, 1f, 1f);
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
        //??
    }
}
