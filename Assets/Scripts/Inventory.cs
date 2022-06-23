using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>Inventory</c>
/// Class that represents the protagonist's inventory
/// </summary>
public class Inventory : Container
{
    /// <summary>
    /// Constructor <c>Inventory</c>
    /// </summary>
    public Inventory()
    {
        SerializableDictionary<Item, int> Storage = new SerializableDictionary<Item, int>();
    }

    /// <summary>
    /// Procedure <c>AddItem</c>
    /// Procedure that adds an item in the inventory
    /// </summary>
    /// <param name="item"> the item that have to be added </param>
    public new void AddItem(Item item, int amount)
    {
        if (item.IsCumulative() && amount > 0 && item is UsableItem && !Storage.KeyExists(item)) 
        {
            Storage.Add(item, amount);
        }
        else if (!item.IsCumulative() && item is UsableItem && !Storage.KeyExists(item))
        {
            Storage.Add(item, 1);
        }

    }
}
