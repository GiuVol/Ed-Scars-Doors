using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawler : GenericMob
{
    /// <summary>
    /// The <c>_attackPointRange</c>
    /// </summary>
    protected float _attackPointRange;
    private LayerMask _playerLayer;
    Collision2D Collision;
    public override bool Attack()
    {
        /*
        if(_canAttack)
        if (_mobAI.IsHookedPlayer)
        {
            float distance = Vector2.Distance(MobAI.GetPlayerTarget().position, transform.position);
            if(distance <= _attackRange)
            {
                    //posso iniziare con l'attacco

                    //play attck animation
                    //shot of the enemy

                    //detectk enemy in range to point attck
                    Collider2D [] hitPlayers = Physics2D.OverlapCircleAll(_attackPoint.position, _attackPointRange, _playerLayer);
                    foreach (Collider2D player in hitPlayers)
                    {
                        IHealthable playerHealth = player.gameObject.GetComponent<IHealthable>();
                        if ( playerHealth != null)
                        {
                            playerHealth.Health.DecreaseHealth(Stats.Attack.CurrentValue);

                            _canAttack = false;

                            yield return new WaitForSeconds(_attackInterval);

                            _canAttack = true;
                        }
                    }
                }
            }*/
        return false;
    }

    protected override void AttackTime(Func<bool> Attack)
    {
        if (_timeLeftToAttack < 0f)
        {
            if (Attack())
            {
                _timeLeftToAttack = _attackInterval;
            }
        }
        else
        {
            _timeLeftToAttack -= Time.deltaTime;
        }
    }

    public override void Die()
    {
        Destroy(gameObject);
    }

    public override void SetName()
    {
        _name = "Crawler";
    }

    public override void SetupHealth()
    {
        Health = new HealthComponent(100, Die);
    }

    public override void SetupMobAI()
    {
        _mobAI.Setup(200f, 3f, 5f, true, false);
    }

    public override void SetupStats()
    {
        Stats = new StatsComponent(100, 200, 50, 100, 200, 50, 100, 200, 50);
    }

    public override void SetupStatus()
    {
        Status.Setup(50, 10, 2.5f, 0, 2, 15, 0);
    }

    protected override void SetupMob()
    {
        _attackRange = 2.5f;
        _attackInterval = 5f;
        _playerLayer = LayerMask.GetMask("Player");
    }
}
