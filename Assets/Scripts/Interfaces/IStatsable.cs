using System;


/// <summary>
/// Interface <c>IStatsable</c>
/// Interface for character statistics management
/// </summary>
public interface IStatsable
{
    /// <summary>
    /// Property <c>Stats</c>
    /// This property returns the StatsComponent attached to a character
    /// </summary>
    public StatsComponent Stats
    { get; }
}
