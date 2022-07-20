/// <summary>
/// Class <c>LongevityPotion</c>
/// Class that represents a longevity potion
/// </summary>
public class LongevityPotion : UsableItem
{
    /// <summary>
    /// Changes the way in which Max Health is increased.
    /// </summary>
    public bool UsePercentageIncrement;

    /// <summary>
    /// Attribute <c>IntegerIncrement</c>
    /// Attribute that represents the integer increment that the potion allows.
    /// </summary>
    public int IntegerIncrement;

    /// <summary>
    /// Attribute <c>PercentageIncrement</c>
    /// Attribute that represents the percentage increment that the potion allows.
    /// </summary>
    public float PercentageIncrement;

    public override void Use(PlayerController player)
    {
        if (player == null)
        {
            return;
        }

        if (UsePercentageIncrement)
        {
            player.Health.IncreaseMaxHealthPercentage(PercentageIncrement);
        } else
        {
            player.Health.IncreaseMaxHealth(IntegerIncrement);
        }

        player.Health.ResetCurrentHealth();
    }
}
