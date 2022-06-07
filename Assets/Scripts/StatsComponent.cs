/// <summary>
/// Class <c>StatsComponent</c>
/// Class that contains all the character's components
/// </summary>
public class StatsComponent
{
    /// <summary>
    /// Property <c>Attack</c>
    /// Property that represents the value of the character's attack
    /// </summary>
    public Stat Attack
    { get; private set; }

    /// <summary>
    /// Property <c>Defence</c>
    /// Property that represents the value of the character's defence
    /// </summary>
    public Stat Defence
    { get; private set; }

    /// <summary>
    /// Constructor <c>Statcomponent</c> 
    /// </summary>
    /// <param name="attackStandardValue"> the starting value of a character's attack </param>
    /// <param name="attackMinValue"> the minimum value of a character's attack </param>
    /// <param name="attackMaxValue"> the maximum value of a character's attack </param>
    /// <param name="defenceStandardValue"> the starting value of a character's defence </param>
    /// <param name="defenceMinValue"> the minimum value of a character's defence </param>
    /// <param name="defenceMaxValue"> the maximum value of a character's defence </param>
    public StatsComponent(int attackStandardValue, int attackMinValue, int attackMaxValue, 
                          int defenceStandardValue, int defenceMinValue, int defenceMaxValue)
    {
        Attack = new Stat(attackStandardValue, attackMinValue, attackMaxValue);
        Defence = new Stat(defenceStandardValue, defenceMinValue, defenceMaxValue);
    }
}
