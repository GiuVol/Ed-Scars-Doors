public class StatChangeAbility : GenericAbility
{
    private readonly float _attackMultiplier;
    private readonly float _defenceMultiplier;

    public StatChangeAbility(float attackMultiplier, float defenceMultiplier)
    {
        _attackMultiplier = attackMultiplier;
        _defenceMultiplier = defenceMultiplier;
    }

    protected override void Setup(PlayerController playerController)
    {
        float actualAttackMultiplier = (_attackMultiplier <= 0) ? 1 : _attackMultiplier;
        float actualDefenceMultiplier = (_defenceMultiplier <= 0) ? 1 : _defenceMultiplier;

        playerController.Stats.Attack.StatMultiplier *= actualAttackMultiplier;
        playerController.Stats.Defence.StatMultiplier *= actualDefenceMultiplier;
    }
    
    protected override void Takedown(PlayerController playerController)
    {
        float actualAttackMultiplier = (_attackMultiplier <= 0) ? 1 : _attackMultiplier;
        float actualDefenceMultiplier = (_defenceMultiplier <= 0) ? 1 : _defenceMultiplier;

        playerController.Stats.Attack.StatMultiplier /= actualAttackMultiplier;
        playerController.Stats.Defence.StatMultiplier /= actualDefenceMultiplier;
    }
}
