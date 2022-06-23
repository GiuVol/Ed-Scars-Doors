using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>Collection</c>
/// Class that represents the protagonist's collection
/// </summary>
public class Collection : Container
{
    /// <summary>
    /// Constructor <c>Collection</c>
    /// </summary>
    public Collection()
    {
        SerializableDictionary<Item, int> Storage = new SerializableDictionary<Item, int>();
    }

    /// <summary>
    /// Procedure <c>AddItem</c>
    /// Procedure that adds an item in the collection
    /// </summary>
    /// <param name="item"> the item that have to be added </param>
    public void AddItem(Item item)
    {
        if (!item.IsCumulative() && item is CollectableItem && !Storage.KeyExists(item))
        {
            Storage.Add(item, 1);
        }

    }
}
