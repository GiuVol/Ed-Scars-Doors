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
        _uiOperationSelectorPrefab = Resources.Load<GameMenuOperationSelector>(OperationSelectorPath);
    }

    void Update()
    {
        if (!HasControl)
        {
            return;
        }

        if (InputHandler.Up("Down"))
        {
            if (SelectedElementIndex > 1)
            {
                AudioClipHandler.PlayAudio("Audio/SelectButton", 0, transform.position);
                SelectedElementIndex--;
            }
            else
            {
                AudioClipHandler.PlayAudio("Audio/Disabled", 0, transform.position);
            }
        }

        if (InputHandler.Down("Down"))
        {
            if (SelectedElementIndex < NumberOfElements)
            {
                AudioClipHandler.PlayAudio("Audio/SelectButton", 0, transform.position);
                SelectedElementIndex++;
            } else
            {
                AudioClipHandler.PlayAudio("Audio/Disabled", 0, transform.position);
            }
        }

        if (InputHandler.Submit("Down"))
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

        UIOperationSelector uiOperationSelector = Instantiate(_uiOperationSelectorPrefab, transform);

        if (!uiOperationSelector.PromptOperations(SelectedElement.Operations))
        {
            Destroy(uiOperationSelector.gameObject);
            yield break;
        }

        UIManager.Instance.GameMenu.HasControl = false;
        HasControl = false;
        
        yield return new WaitUntil(() => uiOperationSelector.SelectedOperation != null);

        UIManager.Instance.GameMenu.HasControl = true;
        HasControl = true;

        ListElementOperation selectedOperation = uiOperationSelector.SelectedOperation;

        if (selectedOperation != null)
        {
            selectedOperation.Operation();
        }

        Destroy(uiOperationSelector.gameObject);
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

            ListElementOperation noOperation =
                new ListElementOperation("Esci",
                    delegate {

                    }
                );

            newElement.Operations.Add(useOperation);
            newElement.Operations.Add(throwOperation);
            newElement.Operations.Add(noOperation);

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

    #endregion
}
