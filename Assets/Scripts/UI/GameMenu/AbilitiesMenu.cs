using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilitiesMenu : UIListMenu, ITabContent
{
    private const string OperationSelectorPath = "UI/OperationSelector";
    private const string PromptPath = "UI/GameMenuPrompt";

    [SerializeField]
    private Color _equippedColor;

    [SerializeField]
    private Color _unequippedColor;
    
    private UIOperationSelector _uiOperationSelectorPrefab;

    private UIPrompt _uiPromptPrefab;
    
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
        _uiOperationSelectorPrefab = Resources.Load<GameMenuOperationSelector>(OperationSelectorPath);
        _uiPromptPrefab = Resources.Load<UIPrompt>(PromptPath);
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
            }
            else
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
                            AudioClipHandler.PlayAudio("Audio/PressButton");
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
                            try
                            {
                                player.EquipAbility(ability);
                                AudioClipHandler.PlayAudio("Audio/PressButton");
                            } 
                            catch (UnequippableAbilityException ex)
                            {
                                if (_uiPromptPrefab == null)
                                {
                                    return;
                                }

                                AudioClipHandler.PlayAudio("Audio/Disabled");
                                UIPrompt prompt = Instantiate(_uiPromptPrefab, gameObject.transform);

                                if (prompt == null)
                                {
                                    return;
                                }

                                UIManager.Instance.GameMenu.HasControl = false;
                                HasControl = false;

                                Task promptTask = new Task(prompt.PromptText(ex.Message, 
                                    delegate {
                                        UIManager.Instance.GameMenu.HasControl = true;
                                        HasControl = true;
                                        AudioClipHandler.PlayAudio("Audio/PressButton");
                                    }, true));
                            }
                            finally
                            {
                                UpdateElements();
                            }
                        }
                    );
            }

            ListElementOperation noOperation =
                new ListElementOperation("Esci",
                    delegate {

                    }
                );
            
            newElement.Operations.Add(equipOperation);
            newElement.Operations.Add(noOperation);

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
            FirstElementIndex = FirstElementIndex;
            SelectedElementIndex = SelectedElementIndex;
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
