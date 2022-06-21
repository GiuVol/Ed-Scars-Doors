using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>Container</c>
/// Class that represents the protagonist's Container
/// </summary>
public class Container
{
    /// <summary>
    /// Attribute <c>Storage</c>
    /// Attribute that represents the items that the protagonist has got
    /// </summary>
    public SerializableDictionary<Item, int> Storage;

    /// <summary>
    /// Constructor <c>Container</c>
    /// </summary>
    public Container()
    {
        SerializableDictionary<Item, int> Storage = new SerializableDictionary<Item, int>();
    }

    /// <summary>
    /// Procedure <c>GetIstances</c>
    /// Procedure that takes the istances' number of an item
    /// </summary>
    /// <param name="item"> the item to take the istances from </param>
    /// <param name="value"> the the istances' number </param>
    public void GetIstances(Item item, ref int value)
    {
        Storage.Get(item, ref value);
    }

    /// <summary>
    /// Procedure <c>AddItem</c>
    /// Procedure that adds an item at the end of the dictionary
    /// </summary>
    /// <param name="item"> the item that have to be added </param>
    public void AddItem(Item item, int amount)
    {
        if (item.IsCumulative() && amount > 0 && !Storage.KeyExists(item))
        {
            Storage.Add(item, amount);
        }
        else if (!item.IsCumulative() && !Storage.KeyExists(item))
        {
            Storage.Add(item, 1);
        }

    }

    /// <summary>
    /// Procedure <c>RemoveItem</c>
    /// Procedure that removes an item from the dictionary
    /// </summary>
    /// <param name="item"> the item that have to be removed </param>
    public void RemoveItem(Item item)
    {
        if (Storage.KeyExists(item))
            Storage.Remove(item);
    }

    /// <summary>
    /// Procedure <c>AddIstances</c>
    /// Procedure that adds a certain amount of istances in the container
    /// </summary>
    /// <param name="item"> the item to which the istances are to be added </param>
    /// <param name="amount"> the amount to be added </param>
    public void AddIstances(Item item, int amount)
    {
        if(Storage.KeyExists(item) && item.IsCumulative())
        {
            int PreviousAmount = 0;
            GetIstances(item, ref PreviousAmount);

            if(PreviousAmount >= 0)
            {
                amount = Mathf.Min(item.GetMaxIstances(), PreviousAmount + amount);
                Storage.ChangeValue(item, amount);
            }
        }
        
    }

    /// <summary>
    /// Procedure <c>RemoveIstances</c>
    /// Procedure that removes a certain amount of istances in the container
    /// </summary>
    /// <param name="item"> the item to which the istances are to be removed </param>
    /// <param name="amount"> the amount to be removed </param>
    public void RemoveIstances(Item item, int amount)
    {
        if(Storage.KeyExists(item) && item.IsCumulative())
        {
            int PreviousAmount = 0;
            GetIstances(item, ref PreviousAmount);

            if(PreviousAmount >= 0)
            {
                amount = Mathf.Max(0, PreviousAmount + amount);
                Storage.ChangeValue(item, amount);
            }
        } 
    }
}
