using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Flydier : GenericMob
{
    /// <summary>
    /// Specifies if the flydier should strictly follow the patrol points.
    /// </summary>
    [SerializeField]
    private bool _stayOnPattern;
    
    public override void Die()
    {
        Destroy(gameObject);
    }

    protected new void Start()
    {
        base.Start();
    }

    private void FixedUpdate()
    {
        if (_isAttacking)
        {
            return;
        }
        
        PlayerController player = _mobAI.FindPlayerInRadius(transform.position, _rangeToCheck);
        
        if (_stayOnPattern)
        {
            Patrol();
            
            Vector3 lookDirection;

            if (player == null)
            {
                lookDirection = (CurrentPatrolPoint.position - transform.position).normalized;
            } else
            {
                lookDirection = (player.transform.position - transform.position).normalized;
            }

            if (lookDirection.x > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }
            else if (lookDirection.x < 0)
            {
                transform.rotation = Quaternion.Euler(0, -180, 0);
            }

            if (player != null)
            {
                float heightDistance = Mathf.Abs(transform.position.y - player.transform.position.y);

                if (heightDistance < 3)
                {
                    StartCoroutine(HandleAttack(player));
                }
            }

            return;
        }

        if (player == null)
        {
            Patrol();
            _mobAI.Target = null;
        } else
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance > _attackRange)
            {
                ChasePlayer(player.transform);
            } else
            {
                if (_canAttack)
                {
                    StartCoroutine(HandleAttack(player));
                }
            }
        }
    }
    
    private void Patrol()
    {
        if (FirstPatrolPoint == null)
        {
            return;
        }

        Vector3 direction = (CurrentPatrolPoint.position - transform.position).normalized;
        float distance = Vector3.Distance(transform.position, CurrentPatrolPoint.position);

        if (distance > 3)
        {
            _attachedRigidbody.AddForce(direction * _speed * Time.deltaTime);
        } else
        {
            IncreasePatrolPoint();
        }
    }

    private void ChasePlayer(Transform playerTransform)
    {
        _mobAI.Target = playerTransform;

        Vector3 direction = (playerTransform.position - transform.position).normalized;

        if (direction.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (direction.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        
        _attachedRigidbody.AddForce(_mobAI.DesiredDirection * _speed * Time.deltaTime);
    }

    protected override IEnumerator Attack(PlayerController target)
    {
        Transform playerTransform = target.transform;

        Vector3 direction = 
            ((playerTransform.position + playerTransform.up * 2) - transform.position).normalized;

        if (direction.x > 0)
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (direction.x < 0)
        {
            transform.rotation = Quaternion.Euler(0, -180, 0);
        }

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion desiredRotation = Quaternion.AngleAxis(angle, Vector3.forward);

        Vector3 spawnPoint = transform.position + direction * 4;

        Projectile resource =
            Resources.Load<Projectile>(Projectile.ProjectileResourcesPath + Projectile.NormalProjectileName);

        Projectile projectile = Instantiate(resource, spawnPoint, desiredRotation);

        projectile.AttackerAttack = Stats.Attack.CurrentValue;

        yield break;
    }
}
