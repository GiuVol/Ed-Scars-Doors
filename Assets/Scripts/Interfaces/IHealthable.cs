/// <summary>
/// Interface <c>IHealthable</c>
/// Interface for character health management.
/// </summary>
interface IHealthable
{
    /// <summary>
    /// Property <c>HealthComponent</c>
    /// This property returns the <c>HealthComponent</c> attached to a character.
    /// </summary>
    public HealthComponent Health
    { get; }
}
