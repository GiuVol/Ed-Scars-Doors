using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesMenu : UIListMenu, ITabContent
{
    private const string OperationSelectorPath = "UI/OperationSelector";

    [SerializeField]
    private Color _equippedColor;

    [SerializeField]
    private Color _unequippedColor;
    
    private UIOperationSelector _uiOperationSelectorPrefab;

    private List<int> _equippedIndexes;

    private List<int> EquippedIndexes
    {
        get
        {
            if (_equippedIndexes == null)
            {
                _equippedIndexes = new List<int>();
            }

            return _equippedIndexes;
        }
    }

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
        EquippedIndexes.Clear();

        #region Pre-conditions check

        if (GameManager.Instance == null)
        {
            return;
        }

        if (GameManager.Instance.Player == null)
        {
            return;
        }

        if (GameManager.Instance.Player.ObtainedAbilities == null)
        {
            return;
        }

        #endregion

        PlayerController player = GameManager.Instance.Player;

        int counter = 1;

        foreach (GenericAbility ability in player.ObtainedAbilities)
        {
            ElementMetadata newElement = 
                new ElementMetadata(ability.Name, 1, null, ability.Description, null);

            ListElementOperation equipOperation;

            if (player.IsEquipped(ability))
            {
                EquippedIndexes.Add(counter);
                equipOperation =
                    new ListElementOperation(
                        "Disequipaggia",
                        delegate
                        {
                            player.UnequipAbility(ability);
                            UpdateElements();
                        }
                    );
            } else 
            {
                EquippedIndexes.Remove(counter);
                equipOperation =
                    new ListElementOperation(
                        "Equipaggia",
                        delegate
                        {
                            player.EquipAbility(ability);
                            UpdateElements();
                        }
                    );
            }

            newElement.Operations.Add(equipOperation);

            ElementsMetadata.Add(newElement);

            counter++;
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

    /// <summary>
    /// Method that updates the displayed elements.
    /// </summary>
    protected override void UpdateUIElements()
    {
        if (NumberOfUIElements <= 0)
        {
            return;
        }

        int counter = FirstElementIndex;

        foreach (UIListElement uiElement in UIElements)
        {
            if (!FillUIElement(counter, uiElement))
            {
                uiElement.Clear();
                uiElement.Enabled = false;
            }
            else
            {
                uiElement.Enabled = true;

                if (EquippedIndexes.Contains(counter))
                {
                    uiElement.Label.color = _equippedColor;
                } else
                {
                    uiElement.Label.color = _unequippedColor;
                }
            }

            counter++;
        }
    }
    
    #endregion
}
