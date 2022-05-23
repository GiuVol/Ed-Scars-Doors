using System;


/// <summary>
/// Interface <c>IStatsable</c>
/// Interface for character statistics management
/// </summary>
interface IStatsable
{
    /// <summary>
    /// Property <c>CurrentSpeed</c>
    /// properties for viewing and editing the current speed of the character
    /// </summary>
    public int CurrentSpeed
    { get; set; }

    /// <summary>
    /// Property <c>CurrentAttack</c>
    /// properties for viewing and editing the current attack of the character
    /// </summary>
    public int CurrentAttack
    { get; set; }

    /// <summary>
    /// Property <c>CurrentDefence</c>
    /// properties for viewing and editing the current defence of the character
    /// </summary>
    public int CurrentDefence
    { get; set; }

    /// <summary>
    /// Const <c>MinSpeed</c>
    /// Minimum speed at which a character can move
    /// </summary>
    private const int MinSpeed = 0;

    /// <summary>
    /// Const <c>MinDefence</c>
    /// Minimum defence a character can have to reduce damage recieved
    /// </summary>
    private const int MinDefence = 0;

    /// <summary>
    /// Const <c>MinDamage</c>
    /// Minimum damage a character can inflict
    /// </summary>
    private const int MinAttack = 0;

    /// <summary>
    /// Property <c>MaxSpeed</c>
    /// properties for viewing and editing the maximum speed of the character
    /// </summary>
    public int MaxSpeed
    { get; set; }

    /// <summary>
    /// Property <c>CurrentDefence</c>
    /// properties for viewing and editing the maximum defence of the character
    /// </summary>
    public int MaxDefence
    { get; set; }

    /// <summary>
    /// Property <c>CurrentAttack</c>
    /// properties for viewing and editing the maximum attack of the character
    /// </summary>
    public int MaxAttack
    { get; set; }

    /// <summary>
    /// Method <c>IncrementStat</c>
    /// Method used to increment any character's statistic
    /// </summary>
    /// <param name="Variation">The integer value that have to be added to the statistic</param>
    /// <param name="Stat">The character's statistic that have to be increased</param>
    /// <param name="Threshold">The minimum threshold reachable by the statistic "Stat"</param>
    public void IncrementStat(int Variation, ref int Stat, int Threshold)
    {
        if (Variation + Stat > Threshold)
            Stat = Threshold;
        else
            Stat = Variation + Stat;
    }

    /// <summary>
    /// Method <c>DecrementStat</c>
    /// Method used to decrement any character's statistic
    /// </summary>
    /// <param name="Variation">The integer value that have to be subtracted to the statistic</param>
    /// <param name="Stat">The character's statistic that have to be decremented</param>
    /// <param name="Threshold">The minimum threshold reachable by the statistic "Stat"</param>
    public void DecrementStat(int Variation, ref int Stat, int Threshold)
    {
        if (Variation - Stat < Threshold)
            Stat = Threshold;
        else
            Stat = Variation + Stat;
    }
}
