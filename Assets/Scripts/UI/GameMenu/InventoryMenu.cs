using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryMenu : ListMenu, ITabContent
{
    public bool HasControl { get; set; }

    protected override void FillElementsMetadata()
    {
        ElementsMetadata.Clear();

        Dictionary<UsableItem, int> inventory = 
            GameManager.Instance.Player.Inventory.ContainerStructure.ToDictionary();

        foreach (UsableItem item in inventory.Keys)
        {
            ElementMetadata newElement = 
                new ElementMetadata(item.Name, null, item.Description, null);

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

    private void OnDisable()
    {
        HasControl = false;
    }
}
