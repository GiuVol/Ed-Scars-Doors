using System;

/// <summary>
/// Class <c>HealthComponent</c>
/// Class that each <c>IHealthable</c> must have
/// </summary>
public class HealthComponent 
{
    /// <summary>
    /// Const <c>DeathThreshold</c>
    /// Value for which the character can be considered dead
    /// </summary>
    public const int DeathThreshold = 0;

    /// <summary>
    /// Property <c>CourrentHealth</c>
    /// properties for viewing and editing the current health of the character
    /// </summary>
    public int CurrentHealth
    { get; private set; }

    /// <summary>
    /// Property <c>MaxHealth</c>
    /// properties for viewing and editing the maximum health of the character
    /// </summary>
    public  int MaxHealth
    { get; private set; }

    /// <summary>
    /// Property <c>IsDead</c>
    /// Properties to see if the character is dead
    /// </summary>
    public bool IsDead
    {
        get
        {
            return (CurrentHealth <= DeathThreshold);
        }
    }

    /// <summary>
    /// Constructor <c>Healthable</c>
    /// The constructor of Healthable
    /// </summary>
    /// <param name="maxHealth">The integer value to be assigned to MaxHealth and CurrentHealth </param>
    public HealthComponent(int maxHealth)
    {
        MaxHealth = Math.Max(maxHealth, 0);
        CurrentHealth = MaxHealth; // a character when created will have CurrentHealth equal to MaxHealth
    }

    /// <summary>
    /// Method <c>IncrementHealth</c>
    /// Increases the health of the character
    /// </summary>
    /// <param name="increment">The integer value of the healing the character received </param>
    public void IncreaseHealth(int increment)
    {
        increment = Math.Max(increment, 0); // if increment has a negative value it will assume a value of 0, otherwise it remains unchanged
        CurrentHealth = Math.Min(increment + CurrentHealth, MaxHealth); // if the sum between the increase and the CurrentHealth is greater than the Maxhealth, then the CurrentHealth will be equal to the Maxhealth, otherwise it will be equal to the sum
    }

    /// <summary>
    /// Method <c>DecrementHealth</c>
    /// Decreases the health of the character
    /// </summary>
    /// <param name="decrement">The integer value of the damage the character received</param>
    public void DecreasetHealth(int decrement)
    {
        decrement = Math.Max(decrement, 0); // if decrement has a negative value it will assume a value of 0, otherwise it remains unchanged
        CurrentHealth = Math.Max(CurrentHealth - decrement, DeathThreshold); // if the decrement greater than CurrentHealth, the CurrentHealth will be 0, otherwise it will be equal to the difference
    }

    public void ResetCurrentHealth()
    {
        CurrentHealth = MaxHealth;
    }

    /// <summary>
    /// Method <c>incrementMaxHealth</c>
    /// Increases the maximum health of the character
    /// </summary>
    /// <param name="increment">The integer value of the increment the character received</param>
    public void incrementMaxHealth(int increment)
    {
        MaxHealth = MaxHealth + Math.Max(increment, 0);
    }
}
