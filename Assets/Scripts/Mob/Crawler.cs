using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawler : GenericMob
{
    Collision2D Collision;
    public override IEnumerator Attack()
    {
        if (Collision != null && _canAttack == true)
        {
            IHealthable collidedHealthable = Collision.gameObject.GetComponent<IHealthable>();
            collidedHealthable.Health.DecreaseHealth(Mathf.FloorToInt(Stats.Attack.CurrentValue));
            _canAttack = false;
            Collision = null;
            yield return new WaitForSeconds(_attackInterval);

            _canAttack = true;
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Collision = collision;
        }
    }
}
