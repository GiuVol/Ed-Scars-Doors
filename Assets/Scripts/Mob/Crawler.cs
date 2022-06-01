using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawler : GenericMob
{
    Collision2D Collision;
    public override IEnumerator Attack()
    {
        if (Collision != null && CanAttack == true)
        {
            IHealthable collidedHealthable = Collision.gameObject.GetComponent<IHealthable>();
            collidedHealthable.Health.DecreaseHealth(Mathf.FloorToInt(Stats.Attack.CurrentValue));
            CanAttack = false;
            Collision = null;
            yield return new WaitForSeconds(AttackInterval);

            CanAttack = true;
        }
    }

    public override void SetName()
    {
        Name = "Crawler";
    }

    public override void SetupHealth()
    {
        Health = new HealthComponent(100, Die);
    }

    public override void SetupMobAI()
    {
        MobAI.Speed = 200f;
        MobAI.NextWayPointDistance = 3f;
        MobAI.Drag = 1.5f;
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
        AttackInterval = 5f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            Collision = collision;
        }
    }
}
