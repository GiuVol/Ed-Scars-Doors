using System;
using UnityEngine;

/// <summary>
/// Class <c>HealthComponent</c>
/// Component that stores all values and methods to manage the health of a character.
/// </summary>
public class HealthComponent 
{
    /// <summary>
    /// Const <c>DeathThreshold</c>
    /// Value from which the character can be considered dead.
    /// </summary>
    public const int DeathThreshold = 0;

    /// <summary>
    /// Property <c>CourrentHealth</c>
    /// Property that represents the current health of the character.
    /// </summary>
    public int CurrentHealth
    { get; private set; }

    /// <summary>
    /// Property <c>MaxHealth</c>
    /// Property that stores the max value that <c>CurrentHealth</c> can reach.
    /// </summary>
    public int MaxHealth
    { get; private set; }

    /// <summary>
    /// Property <c>IsDead</c>
    /// Returns whether the character is dead or not.
    /// </summary>
    public bool IsDead
    {
        get
        {
            return (CurrentHealth <= DeathThreshold);
        }
    }

    /// <summary>
    /// A new delegate type, to store the die procedure.
    /// </summary>
    public delegate void Die();

    /// <summary>
    /// It will store the procedure called when the health of the character reaches <c>DeathThreshold</c>.
    /// </summary>
    public Die DieProcedure { get; set; }

    /// <summary>
    /// Constructor <c>Healthable</c>
    /// The constructor of <c>Healthable</c>.
    /// </summary>
    /// <param name="maxHealth">The integer value to assign to <c>MaxHealth</c> and <c>CurrentHealth</c></param>
    /// <param name="dieProcedure">The desired procedure to call when the character dies</param>
    public HealthComponent(int maxHealth, Die dieProcedure)
    {
        MaxHealth = Math.Max(maxHealth, 0);
        CurrentHealth = MaxHealth; // a character when created will have CurrentHealth equal to MaxHealth
        DieProcedure = dieProcedure;
    }

    /// <summary>
    /// Method <c>IncreaseHealth</c>
    /// Increases the health of the character.
    /// </summary>
    /// <param name="increment">The integer value of the healing the character received</param>
    public void IncreaseHealth(int increment)
    {
        increment = Math.Max(increment, 0); // if increment has a negative value it will assume a value of 0, otherwise it remains unchanged
        CurrentHealth = Math.Min(CurrentHealth + increment, MaxHealth); // if the sum between the increase and the CurrentHealth is greater than the Maxhealth, then the CurrentHealth will be equal to the Maxhealth, otherwise it will be equal to the sum
    }

    /// <summary>
    /// Method <c>DecreaseHealth</c>
    /// Decreases the health of the character
    /// </summary>
    /// <param name="decrement">The integer value of the damage the character received</param>
    public void DecreaseHealth(int decrement)
    {
        decrement = Math.Max(decrement, 0); // if decrement has a negative value it will assume a value of 0, otherwise it remains unchanged
        CurrentHealth = Math.Max(CurrentHealth - decrement, DeathThreshold); // if the decrement greater than CurrentHealth, the CurrentHealth will be 0, otherwise it will be equal to the difference

        if (CurrentHealth <= DeathThreshold)
        {
            DieProcedure();
        }
    }

    /// <summary>
    /// Procedure <c>IncreasePercentage</c>
    /// Procedure that increases by a certain percentage the current value of the health
    /// </summary>
    /// <param name="variation">
    /// the percentage of the increment
    /// pre: this value should be clamped between 0 and 1.
    /// </param>
    public void IncreasePercentage(float variation)
    {
        variation = Mathf.Clamp01(variation);
        int increment = Mathf.FloorToInt((float)MaxHealth * variation);

        IncreaseHealth(increment);
    }

    /// <summary>
    /// Procedure <c>DecreasePercentage</c>
    /// Procedure that decreases by a certain percentage the current value of the health
    /// </summary>
    /// <param name="variation">
    /// the percentage of the decrement
    /// pre: this value should be clamped between 0 and 1.
    /// </param>
    public void DecreasePercentage(float variation)
    {
        variation = Mathf.Clamp01(variation);
        int decrement = Mathf.FloorToInt((float)MaxHealth * variation);

        DecreaseHealth(decrement);
    }

    /// <summary>
    /// Method <c>ResetCurrentHealth</c>
    /// Changes the <c>CurrentHealth</c> to <c>MaxHealth</c>
    /// </summary>
    public void ResetCurrentHealth()
    {
        CurrentHealth = MaxHealth;
    }

    /// <summary>
    /// Method <c>IncrementMaxHealth</c>
    /// Increases the maximum health of the character
    /// </summary>
    /// <param name="increment">The integer value of the increment the character's health received</param>
    public void IncreaseMaxHealth(int increment)
    {
        MaxHealth += Math.Max(increment, 0);
    }
}
