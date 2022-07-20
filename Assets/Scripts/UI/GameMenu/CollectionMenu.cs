using System.Collections.Generic;
using UnityEngine;

public class CollectionMenu : UIListMenu, ITabContent
{
    public bool HasControl { get; set; }

    protected override void FillElementsMetadata()
    {
        ElementsMetadata.Clear();

        #region Pre-conditions check

        if (GameManager.Instance == null)
        {
            return;
        }

        if (GameManager.Instance.Player == null)
        {
            return;
        }

        if (GameManager.Instance.Player.Collection == null)
        {
            return;
        }

        #endregion
        
        Dictionary<CollectableItem, int> collection =
            GameManager.Instance.Player.Collection.ContainerStructure.ToDictionary();

        foreach (CollectableItem item in collection.Keys)
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
            FirstElementIndex = FirstElementIndex;
            SelectedElementIndex = SelectedElementIndex;
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
