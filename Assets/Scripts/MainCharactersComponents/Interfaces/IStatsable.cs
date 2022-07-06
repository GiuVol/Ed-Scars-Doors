/// <summary>
/// Interface <c>IStatsable</c>
/// Interface for character stats management.
/// </summary>
public interface IStatsable
{
    /// <summary>
    /// Property <c>Stats</c>
    /// This property returns the <c>StatsComponent</c> attached to a character.
    /// </summary>
    public StatsComponent Stats
    { get; }
}
