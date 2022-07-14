using System.Collections.Generic;
using UnityEngine;

public class CollectionMenu : ListMenu, ITabContent
{
    public bool HasControl { get; set; }

    protected override void FillElementsMetadata()
    {
        ElementsMetadata.Clear();

        Dictionary<CollectableItem, int> inventory =
            GameManager.Instance.Player.Collection.ContainerStructure.ToDictionary();

        foreach (CollectableItem item in inventory.Keys)
        {
            ElementMetadata newElement =
                new ElementMetadata(item.Name, 1, item.ItemIcon, 
                                    item.Description, item.ItemImage);

            ElementsMetadata.Add(newElement);
        }
    }

    public void Activate(bool active)
    {
        HasControl = active;

        if (active)
        {
            UpdateElements();
            FirstElementIndex = 1;
            SelectedElementIndex = 1;
        }
    }

    void Update()
    {
        if (!HasControl)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SelectedElementIndex++;
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SelectedElementIndex--;
        }
    }
}
