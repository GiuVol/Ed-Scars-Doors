using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class <c>Item</c>
/// Class that represents a generic item
/// </summary>
public abstract class Item : ScriptableObject
{
    /// <summary>
    /// Attribute <c>Name</c>
    /// Attribute that represents the item's name
    /// </summary>
    public string Name;

    /// <summary>
    /// Attribute <c>Description</c>
    /// Attribute that represents the item's description
    /// </summary>
    public string Description;

    /// <summary>
    /// Property <c>Cumulative</c>
    /// Property that represents the possibility to have multiple items of the same type
    /// </summary>
    public bool Cumulative;

    /// <summary>
    /// Const <c>MaxNumber</c>
    /// Constant that represents the maximum number of non so come scrivere
    /// </summary>
    public const int MaxNumber = 99;

    /// <summary>
    /// Constructor <c>Item</c>
    /// </summary>
    /// <param name="name"> the item's name </param>
    /// <param name="description"> the item's description </param>
    /// <param name="cumulative"> if true, the item can have multiple istances </param>
    public Item(string name, string description, bool cumulative)
    {
        Name = name;
        Description = description;
        Cumulative = cumulative;
    }

    /// <summary>

    public string GetName()
    {
        return Name;
    }

    /// <summary>
    /// Procedure <c>SetName</c>
    /// Procedure that sets the item's name
    /// </summary>
    /// <param name="name"> the item's name </param>
    public void SetName(string name)
    {
        Name = name;
    }

    /// <summary>
    /// Function <c>GetDescription</c>
    /// Function that returns the item's description
    /// </summary>
    /// <returns> the item's description </returns>
    public string GetDescription()
    {
        return Description;
    }

    /// <summary>
    /// Procedure <c>SetDescription</c>
    /// Procedure that sets the item's description
    /// </summary>
    /// <param name="description"> the item's description </param>
    public void SetDescription(string description)
    {
        Description = description;
    }

    /// <summary>
    /// Function <c>IsCumulative</c>
    /// Function that returns the item's cumulative flag
    /// </summary>
    /// <returns> the item's cumulative flag </returns>
    public bool IsCumulative()
    {
        return Cumulative;
    }

    /// <summary>
    /// Procedure <c>SetCumulative</c>
    /// Procedure that sets the item's cumulative flag
    /// </summary>
    /// <param name="cumulative"> the item's cumulative flag </param>
    public void SetCumulative(bool cumulative)
    {
        Cumulative = cumulative;
    }

    /// <summary>
    /// Function <c>GetDescription</c>
    /// Function that returns the item's maximum istances' number
    /// </summary>
    /// <returns> the item's maximum istances' number </returns>
    public int GetMaxIstances()
    {
        return MaxNumber;
    }
}
