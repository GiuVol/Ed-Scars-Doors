using System;
using UnityEngine;

/// <summary>
/// Class <c>HealthComponent</c>
/// Component that stores all values and methods to manage the health of a character.
/// </summary>
public class HealthComponent : MonoBehaviour
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
    /// A new delegate type.
    /// </summary>
    public delegate void Die();

    /// <summary>
    /// A new delegate type.
    /// </summary>
    public delegate void OnHealthIncrease();
    
    /// <summary>
    /// A new delegate type.
    /// </summary>
    public delegate void OnHealthDecrease();

    /// <summary>
    /// It will store the procedure called when the health of the character reaches <c>DeathThreshold</c>.
    /// </summary>
    public Die DieProcedure { get; set; }

    /// <summary>
    /// It will store the procedure called when the health of the character is increased.
    /// </summary>
    public OnHealthIncrease OnHealthIncreaseProcedure { get; set; }
    
    /// <summary>
    /// It will store the procedure called when the health of the character decreases.
    /// </summary>
    public OnHealthDecrease OnHealthDecreaseProcedure { get; set; }

    /// <summary>
    /// Stores whether the component is initialized or not.
    /// </summary>
    private bool _initialized;

    /// <summary>
    /// This method initializes the Health Component.
    /// </summary>
    /// <param name="maxHealth">The integer value to assign to <c>MaxHealth</c> and <c>CurrentHealth</c></param>
    /// <param name="dieProcedure">The desired procedure to call when the character dies</param>
    public void Setup(int maxHealth, Die dieProcedure, 
                      OnHealthIncrease onHealthIncreaseProcedure = null,
                      OnHealthDecrease onHealthDecreaseProcedure = null)
    {
        MaxHealth = Math.Max(maxHealth, 0);
        CurrentHealth = MaxHealth; // a character when created will have CurrentHealth equal to MaxHealth
        DieProcedure = dieProcedure;
        OnHealthIncreaseProcedure = onHealthIncreaseProcedure;
        OnHealthDecreaseProcedure = onHealthDecreaseProcedure;

        _initialized = true;
    }

    /// <summary>
    /// Method <c>IncreaseHealth</c>
    /// Increases the health of the character.
    /// </summary>
    /// <param name="increment">The integer value of the healing the character received</param>
    public void Increase(int increment)
    {
        if (!_initialized)
        {
            return;
        }

        int oldValue = CurrentHealth;

        increment = Math.Max(increment, 0);
        CurrentHealth = Math.Min(CurrentHealth + increment, MaxHealth);

        if (CurrentHealth != oldValue)
        {
            if (OnHealthIncreaseProcedure != null)
            {
                OnHealthIncreaseProcedure();
            }
        }
    }

    /// <summary>
    /// Method <c>DecreaseHealth</c>
    /// Decreases the health of the character
    /// </summary>
    /// <param name="decrement">The integer value of the damage the character received</param>
    public void DecreaseHealth(int decrement)
    {
        if (!_initialized)
        {
            return;
        }

        int oldValue = CurrentHealth;
        
        decrement = Math.Max(decrement, 0);
        CurrentHealth = Math.Max(CurrentHealth - decrement, DeathThreshold);

        if (CurrentHealth != oldValue)
        {
            if (OnHealthDecreaseProcedure != null)
            {
                OnHealthDecreaseProcedure();
            }
        }

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
        if (!_initialized)
        {
            return;
        }
        
        variation = Mathf.Clamp01(variation);
        int increment = Mathf.FloorToInt((float)MaxHealth * variation);

        Increase(increment);
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
        if (!_initialized)
        {
            return;
        }
        
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
        if (!_initialized)
        {
            return;
        }
        
        CurrentHealth = MaxHealth;
    }

    /// <summary>
    /// Method <c>IncrementMaxHealth</c>
    /// Increases the maximum health of the character
    /// </summary>
    /// <param name="increment">The integer value of the increment the character's health received</param>
    public void IncreaseMaxHealth(int increment)
    {
        if (!_initialized)
        {
            return;
        }
        
        MaxHealth += Math.Max(increment, 0);
    }

    /// <summary>
    /// Method <c>IncrementMaxHealth</c>
    /// Increases the maximum health of the character
    /// </summary>
    /// <param name="increment">The integer value of the increment the character's health received</param>
    public void IncreaseMaxHealthPercentage(float variation)
    {
        if (!_initialized)
        {
            return;
        }

        variation = Mathf.Clamp01(variation);
        int increment = Mathf.FloorToInt((float)MaxHealth * variation);

        MaxHealth += Math.Max(increment, 0);
    }
}
