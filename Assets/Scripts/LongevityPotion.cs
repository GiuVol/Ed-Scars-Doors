using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>LongevityPotion</c>
/// Class that represents a longevity potion
/// </summary>
public class LongevityPotion : UsableItem
{
    /// <summary>
    /// Attribute <c>Value</c>
    /// Attribute that represents the item's percentage value
    /// </summary>
    public int Value;

    /// <summary>
    /// Constructor <c>LongevityPotion</c>
    /// </summary>
    /// <param name="name"> the name of the longevity potion </param>
    /// <param name="description"> the description of the longevity potion </param>
    /// <param name="value"> the percentage increment of the max health's statistic's current value </param>
    /// <param name="cumulative"> if true, represents the possibility to have more than one instance </param>
    public LongevityPotion(string name, string description, int value, bool cumulative) : base(name, description, cumulative)
    {
        Name = name;
        Description = description;
        Value = value;
        Cumulative = cumulative;
    }

    /// <summary>
    /// Function <c>GetValue</c>
    /// Function that returns the value of the longevity potion's bonus
    /// </summary>
    /// <returns> the value of the longevity potion's bonus </returns>
    public int GetValue()
    {
        return Value;
    }

    /// <summary>
    /// Procedure <c>SetValue</c>
    /// Procedure that sets the potion's bonus given to the character
    /// </summary>
    /// <param name="value"> the potion's bonus </param>
    public void SetValue(int value)
    {
        Value = value;
    }

    /// <summary>
    /// Procedure <c>Use</c>
    /// Procedure that increases by a certain percentage the maximum value of the health's statistic and set the current health at the maximum value
    /// </summary>
    /// <param name="component"> the component that have to be modified </param>
    public void Use(IHealthable Healthable)
    {
        Healthable.Health.IncreaseMaxHealth(Value);
        Healthable.Health.IncreaseHealth(Healthable.Health.MaxHealth);
    }
}
