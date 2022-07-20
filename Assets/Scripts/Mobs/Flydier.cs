using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flydier : GenericMob
{
    protected override void SetupMob()
    {

    }

    public override void SetupAI()
    {

    }

    public override bool Attack()
    {
        return false;
    }

    public override void Die()
    {

    }

    private void Update()
    {

    }

    #region OldCode

    /*

    /// <summary>
    /// Attribute <c>_father</c>
    /// if flydier is spawnest by a spawnest, this attribute stores the spawnest that he generate flydier
    /// </summary>
    private Spawnest _father;

    public override bool Attack()
    {
        if (_mobAI.IsHookedPlayer) // check if flydier hookeds the player to check if can attack
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
                    // the palyer is not in attack range
                    return false;
                }
            }
        else
        {
            // the player has not been hooked
            return false;
        }
    }

    /// <summary>
    /// Method <c>SetFather</c>
    /// this methos is called when a spawnest spawn flydier and set the father
    /// </summary>
    /// <param name="father"></param>
    internal void SetFather(Spawnest father)
    {
        _father = father;
    }

    public override void SetupMobAI()
    {
        _attackRange = 2.5f;
        _attackInterval = 5f;
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
        if (_father != null) // check if flydier was created by a spawnest
        {
            _father.DecrementCountFlydier();
        }
        Destroy(gameObject);
    }

    */

    #endregion
}
