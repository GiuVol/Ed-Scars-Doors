using System.Collections.Generic;
using UnityEngine;

public class TestInventory : ListMenu
{
    private List<string> _items;

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

        if (Input.GetKeyDown(KeyCode.R))
        {
            int randomIndex = Random.Range(0, _items.Count);

            if(_items.Count > 0)
            {
                _items.RemoveAt(SelectedElementIndex - 1);
                UpdateElements();
            }
        }
    }

    protected override void FillElementsMetadata()
    {
        ElementsMetadata = null;

        foreach (string item in _items)
        {
            string description = "This is a " + item.ToString();

            ElementMetadata newElement =
                new ElementMetadata(item, null, description, null);
            ElementsMetadata.Add(newElement);
        }
    }
}
