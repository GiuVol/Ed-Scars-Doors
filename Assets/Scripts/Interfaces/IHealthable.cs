/// <summary>
/// Interface <c>IHealthable</c>
/// Interface for character health management.
/// </summary>
public interface IHealthable
{
    /// <summary>
    /// Property <c>Health</c>
    /// This property returns the <c>HealthComponent</c> attached to a character.
    /// </summary>
    public HealthComponent Health
    { get; }
}
