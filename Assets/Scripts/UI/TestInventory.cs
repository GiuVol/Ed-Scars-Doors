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

        for(int i = 1; i <= 15; i++)
        {
            _items.Add("Item " + i);
        }

        base.Start();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SelectedElementIndex++;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            SelectedElementIndex+=4;
        }
        
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SelectedElementIndex--;
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            SelectedElementIndex-=4;
        }
    }
}
