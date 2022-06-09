public class StatChangeAbility : GenericAbility
{
    /// <summary>
    /// The attack multiplier given by this ability.
    /// </summary>
    public float AttackMultiplier;

    /// <summary>
    /// The defence multiplier given by this ability
    /// </summary>
    public float DefenceMultiplier;

    public override void Enable(PlayerController playerController)
    {
        float actualAttackMultiplier = (AttackMultiplier <= 0) ? 1 : AttackMultiplier;
        float actualDefenceMultiplier = (DefenceMultiplier <= 0) ? 1 : DefenceMultiplier;

        playerController.Stats.Attack.StatMultiplier *= actualAttackMultiplier;
        playerController.Stats.Defence.StatMultiplier *= actualDefenceMultiplier;
    }

    public override void Disable(PlayerController playerController)
    {
        float actualAttackMultiplier = (AttackMultiplier <= 0) ? 1 : AttackMultiplier;
        float actualDefenceMultiplier = (DefenceMultiplier <= 0) ? 1 : DefenceMultiplier;

        playerController.Stats.Attack.StatMultiplier /= actualAttackMultiplier;
        playerController.Stats.Defence.StatMultiplier /= actualDefenceMultiplier;
    }
}
