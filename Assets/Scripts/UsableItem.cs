using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableItem : Item
{
    /// <summary>
    /// Constructor <c>UsableItem</c>
    /// </summary>
    /// <param name="name"> the item's name </param>
    /// <param name="description"> the item's description </param>
    /// <param name="cumulative"> the name of the longevity potion </param>
    public UsableItem(string name, string description, bool cumulative) : base(name, description, cumulative)
    {
        Name = name;
        Description = description;
        Cumulative = cumulative;
    }
}
