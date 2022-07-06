using System.Collections;
using UnityEngine;

/// <summary>
/// Class <c>StatsComponent</c>
/// Component that stores all values and methods to manage the stats of a character.
/// </summary>
public class StatsComponent : MonoBehaviour
{
    /// <summary>
    /// Property <c>Attack</c>
    /// Property that represents the character's attack stat.
    /// </summary>
    public Stat Attack
    { get; private set; }

    /// <summary>
    /// Property <c>Defence</c>
    /// Property that represents the character's defence stat.
    /// </summary>
    public Stat Defence
    { get; private set; }

    /// <summary>
    /// Stores the coroutine that is handling the stats changes. 
    /// Is <c>null</c> if nothing is temporarily changing the stats.
    /// </summary>
    private Coroutine _currentTemporaryStatsChangingCoroutine;

    /// <summary>
    /// Returns whether something is temporarily changing the stats.
    /// </summary>
    private bool StatsAreTemporarilyChanging
    {
        get
        {
            return _currentTemporaryStatsChangingCoroutine != null;
        }
    }

    /// <summary>
    /// Stores whether the component is initialized or not.
    /// </summary>
    private bool _initialized;

    /// <summary>
    /// This method initializes the Stats Component.
    /// </summary>
    /// <param name="attackStandardValue"> the starting value of a character's attack </param>
    /// <param name="attackMinValue"> the minimum value of a character's attack </param>
    /// <param name="attackMaxValue"> the maximum value of a character's attack </param>
    /// <param name="defenceStandardValue"> the starting value of a character's defence </param>
    /// <param name="defenceMinValue"> the minimum value of a character's defence </param>
    /// <param name="defenceMaxValue"> the maximum value of a character's defence </param>
    public void Setup(int attackStandardValue, int attackMinValue, int attackMaxValue, 
                      int defenceStandardValue, int defenceMinValue, int defenceMaxValue)
    {
        Attack = new Stat(attackStandardValue, attackMinValue, attackMaxValue);
        Defence = new Stat(defenceStandardValue, defenceMinValue, defenceMaxValue);

        _initialized = true;
    }

    /// <summary>
    /// Changes the stats of the character for a certain time.
    /// </summary>
    /// <param name="attackMultiplier">The attack multiplier</param>
    /// <param name="defenceMultiplier">The defence multiplier</param>
    /// <param name="timeToLast">The time for which changes will last</param>
    public void TemporarilyChangeStats(float attackMultiplier, float defenceMultiplier, float timeToLast)
    {
        if (!_initialized || StatsAreTemporarilyChanging)
        {
            return;
        }

        _currentTemporaryStatsChangingCoroutine = StartCoroutine(TemporarilyChangeStatsEnum(attackMultiplier, defenceMultiplier, timeToLast));
    }

    /// <summary>
    /// Handles the temporary changes to the character's stats.
    /// </summary>
    /// <param name="attackMultiplier">The attack multiplier</param>
    /// <param name="defenceMultiplier">The defence multiplier</param>
    /// <param name="timeToLast">The time for which changes will last</param>
    private IEnumerator TemporarilyChangeStatsEnum(float attackMultiplier, float defenceMultiplier, float timeToLast)
    {
        if (!_initialized)
        {
            yield break;
        }

        float oldAttackMultiplier = Attack.StatMultiplier;
        float oldDefenceMultiplier = Defence.StatMultiplier;

        Attack.StatMultiplier *= attackMultiplier;
        Defence.StatMultiplier *= defenceMultiplier;

        float actualAttackMultiplier = Attack.StatMultiplier / oldAttackMultiplier;
        float actualDefenceMultiplier = Defence.StatMultiplier / oldDefenceMultiplier;

        yield return new WaitForSeconds(timeToLast);

        Attack.StatMultiplier /= actualAttackMultiplier;
        Defence.StatMultiplier /= actualDefenceMultiplier;

        _currentTemporaryStatsChangingCoroutine = null;
    }

    /// <summary>
    /// This method resets the stats of the component.
    /// </summary>
    public void ResetStats()
    {
        if (!_initialized)
        {
            return;
        }
        
        if (StatsAreTemporarilyChanging)
        {
            StopCoroutine(_currentTemporaryStatsChangingCoroutine);
            _currentTemporaryStatsChangingCoroutine = null;
        }

        Attack.ResetStat();
        Defence.ResetStat();
    }
}
