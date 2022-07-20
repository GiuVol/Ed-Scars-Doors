using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawler : GenericMob
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

    #region Old Code

    /*

    /// <summary>
    /// The <c>_attackPointRange</c>
    /// </summary>
    protected float _attackPointRange;
    Collision2D Collision;
    [SerializeField]
    private float _dashForce;
    private PointToAttack _pointToAttack;

    public override bool Attack()
    {
        if (_mobAI.IsHookedPlayer) // check if crawler hookeds the player to check if can attack
        {
            float distance = Vector2.Distance(_mobAI.GetMobTransform().position, MobAI.GetPlayerTarget().position);
            if (distance <= _attackRange)
            {
                Vector3 forceToApply;
                //posso iniziare con l'attacco
                //play attck animation
                if (_mobAI.Target.position.x > transform.position.x)
                {
                    forceToApply = transform.right.normalized * _dashForce;
                }
                else
                {
                    forceToApply = (transform.right.normalized * -1) * _dashForce;
                }
                
                _mobAI._rb.AddForce(forceToApply * _mobAI._rb.mass, ForceMode2D.Impulse);

                //shot of the enemy
                _pointToAttack.Activate();
                //detectk enemy in range to point attck
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

    public override void SetupMobAI()
    {
        _mobAI.Setup(200f, 3f, 5f, true, false);
    }

    protected override void SetupMob()
    {
        _attackRange = 2.5f;
        _attackInterval = 5f;
        _dashForce = 10;
        if (_attackPoint.gameObject.GetComponent<PointToAttack>() == null)
        {
            _attackPoint.gameObject.AddComponent<PointToAttack>();
        }
        _pointToAttack = _attackPoint.gameObject.GetComponent<PointToAttack>();
        _pointToAttack.Setup(Stats.Attack.CurrentValue, 1.5f);
    }

    */

    #endregion
}
