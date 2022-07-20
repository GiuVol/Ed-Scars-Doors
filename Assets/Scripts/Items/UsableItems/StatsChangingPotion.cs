/// <summary>
/// Class <c>AttackPotion</c>
/// Class that represents a potion that can modify stats.
/// </summary>
public class StatsChangingPotion : UsableItem
{
    /// <summary>
    /// Attribute <c>AttackMultiplier</c>
    /// Attribute that represents for which value the attack must be multiplied when this potion is used.
    /// </summary>
    public float AttackMultiplier;

    /// <summary>
    /// Attribute <c>DefenceMultiplier</c>
    /// Attribute that represents for which value the defence must be multiplied when this potion is used.
    /// </summary>
    public float DefenceMultiplier;

    /// <summary>
    /// Stores how long the stats change will last.
    /// </summary>
    public float TimeToLast;

    public override void Use(PlayerController player)
    {
        if (player == null)
        {
            return;
        }

        player.Stats.TemporarilyChangeStats(AttackMultiplier, DefenceMultiplier, TimeToLast);
    }
}
