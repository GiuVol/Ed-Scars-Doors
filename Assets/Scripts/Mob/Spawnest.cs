using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnest : GenericMob
{
    /// <summary>
    /// Attribute <c>_attackIntervalPlayerHocked</c>
    /// stores the time interval that the spawnet respects when hockeds player
    /// </summary>
    [SerializeField]
    private float _attackIntervalPlayerHocked;

    /// <summary>
    /// Const <c>_maxFlydier</c>
    /// the max number of flydiers that spawnest can spawn at the same time
    /// </summary>
    private const int _maxFlydier = 2;

    /// <summary>
    /// Attribute <c>_countFlydier</c>
    /// stores the number of flydiers spawn by spawnest actualy not dead
    /// </summary>
    private int _countFlydier = 0;
    public override bool Attack()
    {
        if (_countFlydier < _maxFlydier)
        {
            Instantiate(Resources.Load<Flydier>("Mobs/FlydierPrefab"), _attackPoint.position, _attackPoint.rotation).SetFather(this);
            _countFlydier++;
            return true;
        }
        else
        {
            return false;
        }
    }

    internal void DecrementCountFlydier()
    {
        if(_countFlydier > 0)
            _countFlydier--;
    }

    public override void SetName()
    {
        _name = "Spawnest";
    }

    public override void SetupHealth()
    {
        Health = new HealthComponent(100, Die);
    }

    public override void SetupMobAI()
    {
        _mobAI.Setup(200f, 3f, 5f, false, false);
    }

    public override void SetupStats()
    {
        Stats = new StatsComponent(100, 200, 50, 100, 200, 50);
    }

    public override void SetupStatus()
    {
        Status.Setup(50, 10, 2.5f, 0, 2, 15, 0);
    }

    protected override void SetupMob()
    {
        _attackRange = 2.5f;
        _attackInterval = 5f;
        _attackIntervalPlayerHocked = 2.5f;
    }

    protected override void AttackTime(Func<bool> Attack)
    {
        if (_timeLeftToAttack < 0f)
        {
            if (Attack())
            {
                if (_mobAI.IsHookedPlayer)
                {
                    _timeLeftToAttack = _attackIntervalPlayerHocked;
                }
                else
                {
                    _timeLeftToAttack = _attackInterval;
                }
            }
        }
        else
        {
            if(_countFlydier != _maxFlydier)
            {
                _timeLeftToAttack -= Time.deltaTime;
            }
        }
    }

    public override void Die()
    {
        Destroy(gameObject);
    }
}
