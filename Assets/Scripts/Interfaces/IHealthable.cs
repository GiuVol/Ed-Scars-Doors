/// <summary>
/// Interface <c>IHealthable</c>
/// Interface for character health management.
/// </summary>
interface IHealthable
{
    /// <summary>
    /// Property <c>HealthComponent</c>
    /// This property returns the HealthComponent attached to a character
    /// </summary>
    public HealthComponent Health
    { get; }

    /// <summary>
    /// Method <c>Die</c>
    /// Procedure that is called every time a character reaches 0 health
    /// </summary>
    protected void Die();
}
