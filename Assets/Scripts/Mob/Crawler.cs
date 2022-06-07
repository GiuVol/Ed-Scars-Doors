using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawler : GenericMob
{
    Collision2D Collision;
    public override IEnumerator Attack()
    {
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
            }
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
        _mobAI.Setup(200f, 3f, 1.5f, 5f, true);
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
        _attackInterval = 5f;
    }
}
