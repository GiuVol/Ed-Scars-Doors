using UnityEngine;

/// <summary>
/// Class <c>Stat</c>
/// Class that represents a generic character's statistic
/// </summary>
public class Stat
{
    /// <summary>
    /// Property <c>CurrentValue</c>
    /// Property that represents the current value of a character's statistic
    /// </summary>
    public int CurrentValue
    { get; private set; }

    /// <summary>
    /// Property <c>StandardValue</c>
    /// Property that represents the starting value of a character's statistic
    /// </summary>
    public int StandardValue
    { get; private set; }

    /// <summary>
    /// Property <c>MaxValue</c>
    /// Property that represents the maximum value of a character's statistic
    /// </summary>
    public int MaxValue
    { get; private set; }

    /// <summary>
    /// Property <c>MinValue</c>
    /// Property that represents the minimum value of a character's statistic
    /// </summary>
    public int MinValue
    { get; private set; }

    /// <summary>
    /// Constructor <c>Stat</c>
    /// </summary>
    /// <param name="standardValue"> the starting value of a character's statistic </param>
    /// <param name="maxValue"> the maximum value of a character's statistic </param>
    /// <param name="minValue"> the minimum value of a character's statistic </param>
    public Stat(int standardValue, int maxValue, int minValue)
    {
        StandardValue = standardValue;
        CurrentValue = standardValue;
        MaxValue = maxValue;
        MinValue = minValue;
    }

    /// <summary>
    /// Procedure <c>IncreasePercentage</c>
    /// Procedure that increases by a certain percentage the current value of the statistic
    /// </summary>
    /// <param name="variation"> the percentage of the increment </param>
    public void IncreasePercentage(float variation)
    {
        variation = Mathf.Max(variation, 0);
        int increment = Mathf.FloorToInt(variation * (float)CurrentValue);
        CurrentValue = Mathf.Min(MaxValue, CurrentValue + increment); 
    }

    /// <summary>
    /// Procedure <c>DecreasePercentage</c>
    /// Procedure that decreases by a certain percentage the current value of the statistic
    /// </summary>
    /// <param name="variation"> the percentage of the decrement </param> 
    public void DecreasePercentage(float variation)
    {
        variation = Mathf.Max(variation, 0);
        int decrement = Mathf.FloorToInt(variation * (float)CurrentValue);
        CurrentValue = Mathf.Max(MinValue, CurrentValue - decrement);
    }

    /// <summary>
    /// Procedure <c>ResetStat</c>
    /// Procedure that sets the current value of the character's statistic at its standard value
    /// </summary>
    public void ResetStat()
    {
        CurrentValue = StandardValue;
    }
}
