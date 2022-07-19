using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryMenu : UIListMenu, ITabContent
{
    private const string OperationSelectorPath = "UI/OperationSelector";

    private UIOperationSelector _uiOperationSelectorPrefab;
    
    public bool HasControl { get; set; }

    private new void Start()
    {
        base.Start();
        _uiOperationSelectorPrefab =
            Resources.Load<GameMenuOperationSelector>(OperationSelectorPath);
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

        if (Input.GetKeyDown(KeyCode.Return))
        {
            StartCoroutine(ChooseOperation());
        }
    }

    private IEnumerator ChooseOperation()
    {
        if (SelectedElement == null)
        {
            yield break;
        }

        if (SelectedElement.Operations.Count == 0)
        {
            yield break;
        }

        if (_uiOperationSelectorPrefab == null)
        {
            yield break;
        }

        UIOperationSelector uiOperationSelector = 
            Instantiate(_uiOperationSelectorPrefab, transform);

        if (!uiOperationSelector.PromptOperations(SelectedElement.Operations))
        {
            Destroy(uiOperationSelector.gameObject);
            yield break;
        }

        HasControl = false;
        
        yield return new WaitUntil(() => uiOperationSelector.SelectedOperation != null);

        ListElementOperation selectedOperation = uiOperationSelector.SelectedOperation;

        if (selectedOperation != null)
        {
            selectedOperation.Operation();
        }

        Destroy(uiOperationSelector.gameObject);

        HasControl = true;
    }

    #region Override and Implementation

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

        if (GameManager.Instance.Player.Inventory == null)
        {
            return;
        }
        
        #endregion

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
                new ListElementOperation("Usa",
                    delegate {
                        item.Use(player);
                        player.Inventory.RemoveIstances(item, 1);
                        UpdateElements();
                    }
                );

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

    #endregion
}
