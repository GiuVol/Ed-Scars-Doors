using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEventTrigger : EventTrigger
{
    protected override IEnumerator Action(PlayerController player)
    {
        player.Health.DecreasePercentage(1);
        yield break;
    }
}
