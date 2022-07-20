using UnityEngine;

/// <summary>
/// Class <c>Item</c>
/// Class that represents a generic item.
/// </summary>
public abstract class Item : ScriptableObject
{
    /// <summary>
    /// Const that represents the standard maximum number of allowed instances of a certain type of item.
    /// </summary>
    public const int StandardMaxNumberOfHoldableInstances = 99;

    /// <summary>
    /// Field that represents the item's name.
    /// </summary>
    public string Name;

    /// <summary>
    /// Field that represents the item's description.
    /// </summary>
    public string Description;

    /// <summary>
    /// Field that stores the image of the item.
    /// </summary>
    public Sprite ItemImage;

    /// <summary>
    /// Field that stores the icon of the item.
    /// </summary>
    public Sprite ItemIcon;
    
    /// <summary>
    /// Field that stores the gameObject that represents the item in the overworld.
    /// </summary>
    public GameObject OverworldPrefab;

    /// <summary>
    /// Field that stores whether you can hold or not this type of.
    /// </summary>
    public bool Holdable;
    
    /// <summary>
    /// Field that stores whether you can have or not multiple instances of the item type.
    /// </summary>
    public bool Cumulative;

    /// <summary>
    /// Stores whether the item uses custom or standard max number of instances allowed.
    /// </summary>
    public bool UseCustomMaxNumber;

    /// <summary>
    /// The maximum number of allowed instances of a certain type of item, if it uses custom max number of instances allowed.
    /// </summary>
    public int CustomMaxNumberOfHoldableInstances;

    /// <summary>
    /// Property that returns the maximum number of instances holdable for a specific item.
    /// </summary>
    public int MaxNumberOfHoldableInstances
    {
        get
        {
            if (!Holdable)
            {
                return 0;
            }
            
            if (!Cumulative)
            {
                return 1;
            }

            int maxNumber = UseCustomMaxNumber ? CustomMaxNumberOfHoldableInstances : StandardMaxNumberOfHoldableInstances;

            maxNumber = Mathf.Max(maxNumber, 1);

            return maxNumber;
        }
    }
}
