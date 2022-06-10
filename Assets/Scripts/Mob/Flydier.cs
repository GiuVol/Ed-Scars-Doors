using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flydier : GenericMob
{
    private Spawnest _father;
    public override bool Attack()
    {
        if (_mobAI.IsHookedPlayer)
            {
                float distance = Vector2.Distance(_mobAI.GetMobTransform().position, MobAI.GetPlayerTarget().position);
                if (distance <= _attackRange)
                {
                //posso iniziare con l'attacco

                //play attck animation
                //shot of the enemy

                //detectk enemy in range to point attck
                Instantiate(Resources.Load<Projectile>("Projectiles/FireballPrefab"),
                _attackPoint.position, _attackPoint.rotation).Power *= Stats.Attack.CurrentValue;

                return true;
            }
            else
                {
                    return false;
                }
            }
        else
        {
            return false;
        }
    }

    internal void SetFather(Spawnest father)
    {
        _father = father;
    }

    public override void SetName()
    {
        _name = "Flydier";
    }

    public override void SetupMobAI()
    {
        _mobAI.Setup(200f, 3f, 5f, true);
    }

    public override void SetupHealth()
    {
        Health = new HealthComponent(100, Die);
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
        if (_father != null)
        {
            _father.DecrementCountFlydier();
        }
        Destroy(gameObject);
    }
}
