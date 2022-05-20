
using System;

/// <summary>
/// Interface <c>IHealthable</c>
/// Interface for character health management.
/// </summary>
interface IHealthable
{
    /// <summary>
    /// Const <c>PointOfdead</c>
    /// Value for which the character can be considered dead
    /// </summary>
    public const int DeathThreshold  = 0;
    /// <summary>
    /// Property <c>CourrentHealth</c>
    /// properties for viewing and editing the current health of the character
    /// </summary>
    public int CurrentHealth
    { get; private protected set; }

    /// <summary>
    /// Property <c>MaxHealth</c>
    /// properties for viewing and editing the maximum health of the character
    /// </summary>
    public int MaxHealth
    { get; private protected set;}

    /// <summary>
    /// Property <c>IsDead</c>
    /// Properties to see if the character is dead
    /// </summary>
    public bool IsDead
    {
        get
        {
            return (CurrentHealth <= DeathThreshold );
        }
    }

    /// <summary>
    /// Method <c>DecrementHealth</c>
    /// Decreases the health of the character
    /// </summary>
    /// <param name="decrement">The integer value of the damage the character received</param>
    public void DecrementHealth(int decrement)
    {
        decrement = Math.Max(decrement, 0); // if decrement has a negative value it will assume a value of 0, otherwise it remains unchanged
        CurrentHealth = Math.Max(CurrentHealth - decrement, DeathThreshold ); // if the decrement greater than CurrentHealth, the CurrentHealth will be 0, otherwise it will be equal to the difference
        if (IsDead)
        {
            Dead();
        }
    }

    /// <summary>
    /// Method <c>IncrementHealth</c>
    /// Increases the health of the character
    /// </summary>
    /// <param name="increment">The integer value of the healing the character received </param>
    public void IncrementHealth(int increment)
    {
        increment = Math.Max(increment, 0); // if increment has a negative value it will assume a value of 0, otherwise it remains unchanged
        CurrentHealth = Math.Min(increment + CurrentHealth, MaxHealth); // if the sum between the increase and the CurrentHealth is greater than the Maxhealth, then the CurrentHealth will be equal to the Maxhealth, otherwise it will be equal to the sum
    }

    /// <summary>
    /// Method <c>Dead</c>
    /// Procedure that is called every time a character reaches 0 health
    /// </summary>
    protected void Dead();
}
