using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flydier : GenericMob
{
    public override IEnumerator Attack()
    {
        throw new System.NotImplementedException();
    }

    public override void SetName()
    {
        Name = "Flydier";
    }

    public override void SetupMobAI()
    {
        MobAI.Speed = 200f;
        MobAI.NextWayPointDistance = 3f;
        MobAI.Drag = 1.5f;
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
        throw new System.NotImplementedException();
    }
}
