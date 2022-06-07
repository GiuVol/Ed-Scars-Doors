using UnityEngine;

/// <summary>
/// Class <c>Stat</c>
/// Class that represents a generic character's statistic
/// </summary>
public class Stat
{
    private const float MinStatMultiplier = .5f;
    private const float MaxStatMultiplier = 10;
    
    /// <summary>
    /// Property <c>StandardValue</c>
    /// Property that represents the standard value of the stat
    /// </summary>
    public int StandardValue
    { get; private set; }

    /// <summary>
    /// Field <c>_statMultiplier</c>
    /// This value represents the multiplier of the standard stat value
    /// </summary>
    private float _statMultiplier;

    /// <summary>
    /// Property <c>StatMultiplier</c>
    /// Property that provides access to the <c>_statMultiplier</c> field in a controlled way
    /// </summary>
    public float StatMultiplier
    {
        get
        {
            return _statMultiplier;
        }
        set
        {
            _statMultiplier = Mathf.Clamp(value, MinStatMultiplier, MaxStatMultiplier);
        }
    }

    /// <summary>
    /// Property <c>CurrentValue</c>
    /// Property that represents the current value of the stat
    /// </summary>
    public int CurrentValue
    {
        get
        {
            int currentValue = Mathf.FloorToInt((float) StandardValue * StatMultiplier);
            currentValue = Mathf.Clamp(currentValue, MinValue, MaxValue);

            return currentValue;
        }
    }

    /// <summary>
    /// Property <c>MinValue</c>
    /// Property that represents the minimum value of the stat
    /// </summary>
    public int MinValue
    { get; private set; }

    /// <summary>
    /// Property <c>MaxValue</c>
    /// Property that represents the maximum value of the stat
    /// </summary>
    public int MaxValue
    { get; private set; }

    /// <summary>
    /// Constructor <c>Stat</c>
    /// </summary>
    /// <param name="standardValue"> the starting value of a character's statistic </param>
    /// <param name="minValue"> the minimum value of a character's statistic </param>
    /// <param name="maxValue"> the maximum value of a character's statistic </param>
    public Stat(int standardValue, int minValue, int maxValue)
    {
        StandardValue = standardValue;
        StatMultiplier = 1;
        MinValue = minValue;
        MaxValue = maxValue;
    }

    /// <summary>
    /// Procedure <c>ResetStat</c>
    /// Procedure that sets the current value of the character's statistic at its standard value
    /// </summary>
    public void ResetStat()
    {
        StatMultiplier = 1;
    }
}
