using System.Collections.Generic;
using UnityEngine;

public class TestInventory : ListMenu
{
    private List<string> _items;

    protected override List<ElementMetadata> ElementsMetadata
    {
        get
        {
            List<ElementMetadata> elementsMetadata = new List<ElementMetadata>();

            foreach (string item in _items)
            {
                string description = "This is a " + item.ToString();

                ElementMetadata newElement = 
                    new ElementMetadata(item.ToString(), null, description, null);
                elementsMetadata.Add(newElement);
            }

            return elementsMetadata;
        }
    }

    private new void Start()
    {
        _items = new List<string>();
        _items.Add("Weak Potion");
        _items.Add("Star");
        _items.Add("Speed booster");
        _items.Add("Strong Potion");
        _items.Add("Attack booster");
        _items.Add("Defence booster");
        _items.Add("Invisibility potion");
        _items.Add("Invincibility potion");
        _items.Add("+Attack -Defence potion");
        _items.Add("+Defence -Attack potion");

        base.Start();
    }

    private void Update()
    {
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
