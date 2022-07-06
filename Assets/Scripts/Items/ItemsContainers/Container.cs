using UnityEngine;

/// <summary>
/// Class <c>Container</c>
/// Class that represents a container of elements, considering the amount of each element.
/// </summary>
public class Container<T> where T : Item
{
    /// <summary>
    /// The data structure that contains the elements.
    /// </summary>
    public SerializableDictionary<T, int> ContainerStructure;

    /// <summary>
    /// Constructor <c>Container</c>
    /// </summary>
    public Container()
    {
        ContainerStructure = new SerializableDictionary<T, int>();
    }

    public int PickableUnits(T item)
    {

        int pickableUnits = item.MaxNumberOfHoldableInstances - GetAmount(item);
        pickableUnits = Mathf.Max(pickableUnits, 0);

        return pickableUnits;
    }

    /// <summary>
    /// Procedure that adds a certain amount of items in the Container.
    /// </summary>
    /// <param name="item">The item type to add</param>
    /// <param name="amount">The amount of instances to add</param>
    public void AddItem(T item, int amount)
    {
        if (!ContainerStructure.KeyExists(item))
        {
            ContainerStructure.Add(item, 0);
        }

        int currentUnits = 0;
        ContainerStructure.Get(item, ref currentUnits);

        int pickedUnits = Mathf.Min(amount, PickableUnits(item));

        ContainerStructure.ChangeValue(item, currentUnits + pickedUnits);
    }

    /// <summary>
    /// Procedure <c>RemoveIstances</c>
    /// Procedure that removes a certain amount of istances in the container
    /// </summary>
    /// <param name="item"> the item to which the istances are to be removed </param>
    /// <param name="amount"> the amount to be removed </param>
    public void RemoveIstances(T item, int amount)
    {
        int currentUnits = 0;
        ContainerStructure.Get(item, ref currentUnits);
        
        if (ContainerStructure.KeyExists(item))
        {
            int newValue = Mathf.Max(currentUnits - amount, 0);
            ContainerStructure.ChangeValue(item, newValue);

            int newAmount = 0;
            ContainerStructure.Get(item, ref newAmount);

            if (newAmount <= 0)
            {
                ContainerStructure.Remove(item);
            }
        } 
    }

    /// <summary>
    /// Returns the amount of elements of a given type.
    /// </summary>
    /// <param name="item">The type of the element</param>
    /// <returns>The amount of instances of the item given in input</returns>
    public int GetAmount(T item)
    {
        int amount = 0;

        ContainerStructure.Get(item, ref amount);

        return amount;
    }
}
