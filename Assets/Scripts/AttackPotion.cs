using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>AttackPotion</c>
/// Class that represents an attack potion
/// </summary>
public class AttackPotion : UsableItem
{  
    /// <summary>
    /// Attribute <c>Value</c>
    /// Attribute that represents the item's percentage value
    /// </summary>
    public float Value;

    /// <summary>
    /// Constructor <c>AttackPotion</c>
    /// </summary>
    /// <param name="name"> the name of the attack potion </param>
    /// <param name="description"> the description of the attack potion </param>
    /// <param name="value"> the percentage increment of the attack's statistic's current value </param>
    /// <param name="cumulative"> if true, represents the possibility to have more than one instance </param>
    public AttackPotion(string name, string description, float value, bool cumulative) : base(name, description, cumulative)
    {
        Name = name;
        Description = description;
        Value = value;
        Cumulative = cumulative;
    }

    /// <summary>
    /// Function <c>GetValue</c>
    /// Function that returns the value of the attack potion's bonus
    /// </summary>
    /// <returns> the value of the attack potion's bonus </returns>
    public float GetValue()
    {
        return Value;
    }

    /// <summary>
    /// Function <c>SetValue</c>
    /// Function that sets the value of the attack potion's bonus
    /// </summary>
    /// <param name="value"> the the value of the attack potion's bonus </param>
    public void SetValue(float value)
    {
        Value = value;
    }

    /// <summary>
    /// Procedure <c>Use</c>
    /// Procedure that increases by a certain percentage the current value of the attack's statistic
    /// </summary>
    /// <param name="component"> the component that have to be modified </param>
    public void Use(IStatsable Statsable)
    {
        Statsable.Stats.Attack.IncreasePercentage(Value);
    }
}
