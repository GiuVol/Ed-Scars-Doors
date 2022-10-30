/// <summary>
/// Class <c>HealingPotion</c>
/// Class that represents an healing potion.
/// </summary>
public class HealingPotion : UsableItem
{
    /// <summary>
    /// Changes the way in which Health is increased.
    /// </summary>
    public bool UsePercentageIncrement;

    /// <summary>
    /// Attribute that represents the integer increment that the potion allows.
    /// </summary>
    public int IntegerIncrement;

    /// <summary>
    /// Attribute that represents the percentage increment that the potion allows.
    /// </summary>
    public float PercentageIncrement;
    
    public override void Use(PlayerController player)
    {
        if (player == null)
        {
            return;
        }

        if (player.Health.CurrentHealth >= player.Health.MaxHealth)
        {
            throw new NoNeedToUseThisItemException();
        }

        int oldHealthValue = player.Health.CurrentHealth;

        if (UsePercentageIncrement)
        {
            player.Health.IncreasePercentage(PercentageIncrement);
        }
        else
        {
            player.Health.Increase(IntegerIncrement);
        }

        if (player.Health.CurrentHealth != oldHealthValue)
        {
            AudioClipHandler.PlayAudio("Audio/Healing", 0, player.transform.position, false, .4f);
        }
    }
}
