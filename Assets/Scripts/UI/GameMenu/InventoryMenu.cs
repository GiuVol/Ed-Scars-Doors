using System.Collections.Generic;
using UnityEngine;

public class InventoryMenu : UIListMenu, ITabContent
{
    public bool HasControl { get; set; }

    protected override void FillElementsMetadata()
    {
        ElementsMetadata.Clear();

        PlayerController player = GameManager.Instance.Player;

        Dictionary<UsableItem, int> inventory = 
            player.Inventory.ContainerStructure.ToDictionary();

        foreach (UsableItem item in inventory.Keys)
        {
            int amount;
            inventory.TryGetValue(item, out amount);

            ElementMetadata newElement = 
                new ElementMetadata(item.Name, amount, item.ItemIcon, 
                                    item.Description, item.ItemImage);

            ListElementOperation useOperation = 
                new ListElementOperation("Usa", delegate { item.Use(player); });

            ListElementOperation throwOperation = 
                new ListElementOperation("Getta", 
                    delegate {
                        player.Inventory.RemoveIstances(item, 1);
                        UpdateElements();
                    }
                );

            newElement.Operations.Add(useOperation);
            newElement.Operations.Add(throwOperation);

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

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SelectedElementIndex--;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SelectedElementIndex++;
        }
    }
}
