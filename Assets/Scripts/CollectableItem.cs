using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableItem : Item
{
    /// <summary>
    /// Constructor <c>CollectableItem</c>
    /// </summary>
    /// <param name="name"> the item's name</param>
    /// <param name="description"> the item's description </param>
    public CollectableItem(string name, string description) : base(name, description, false)
    {
        Name = name;
        Description = description;
        Cumulative = false;
    }
}
