using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>HealingPotion</c>
/// Class that represents an health potion
/// </summary>
public class HealingPotion : UsableItem
{
    /// <summary>
    /// Attribute <c>Value</c>
    /// Attribute that represents the item's percentage value
    /// </summary>
    public int Value;

    /// <summary>
    /// Constructor <c>HealingPotion</c>
    /// </summary>
    /// <param name="name"> the name of the health potion </param>
    /// <param name="value"> the percentage increment of the health's statistic's current value </param>
    public HealingPotion(string name, int value, string description, bool cumulative) : base(name, description, cumulative)
    {
        Name = name;
        Value = value;
        Description = description;
        Cumulative = cumulative;
    }

    /// <summary>
    /// Function <c>GetValue</c>
    /// Function that returns the value of the healing potion's bonus
    /// </summary>
    /// <returns> the value of the healing potion's bonus </returns>
    public int GetValue()
    {
        return Value;
    }

    /// <summary>
    /// Function <c>SetValue</c>
    /// Function that sets the value of the healing potion's bonus
    /// </summary>
    /// <param name="value"> the the value of the healing potion's bonus </param>
    public void SetValue(int value)
    {
        Value = value;
    }

    /// <summary>
    /// Procedure <c>Use</c>
    /// Procedure that increases by a certain percentage the current value of the health's statistic
    /// </summary>
    /// <param name="component"> the component that have to be modified </param>
    public void Use(IHealthable Healthable)
    {
        Healthable.Health.IncreasePercentage(Value);
    }
}
