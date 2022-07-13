using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class ListMenu : MonoBehaviour
{
    #region Inner Classes

    /// <summary>
    /// A class which represents an operation you could do on a list element.
    /// </summary>
    protected class ListElementOperation
    {
        /// <summary>
        /// A new type of delegate, which will store a void procedure.
        /// </summary>
        public delegate void OperationDelegate();

        /// <summary>
        /// The name of the operation.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The procedure involved with the operation.
        /// </summary>
        public OperationDelegate Operation { get; set; }

        /// <summary>
        /// The constructor of ListElementOperation.
        /// </summary>
        /// <param name="name">The name of the operation</param>
        /// <param name="operation">The void procedure of the operation</param>
        public ListElementOperation(string name, OperationDelegate operation)
        {
            Name = name;
            Operation = operation;
        }
    }

    /// <summary>
    /// A class which represents the graphic component that displays the actual element.
    /// </summary>
    [System.Serializable]
    protected class UIListElement
    {
        /// <summary>
        /// The part of the GUI component which has to change color when 
        /// the element is selected.
        /// </summary>
        [SerializeField]
        private Image _label;

        /// <summary>
        /// The part of the GUI component which displays the name of the element.
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI _nameArea;

        /// <summary>
        /// A property that provides access to the text of the name area in a controlled manner.
        /// </summary>
        public string NameAreaText
        {
            get
            {
                if (_nameArea == null)
                {
                    return "";
                }

                return _nameArea.text;
            }

            set
            {
                if (_nameArea == null)
                {
                    return;
                }

                _nameArea.text = value;
            }
        }

        /// <summary>
        /// The part of the GUI component which displays the sprite of the element.
        /// </summary>
        [SerializeField]
        private Image _iconArea;

        /// <summary>
        /// A property that provides access to the sprite of the icon area 
        /// in a controlled manner.
        /// </summary>
        public Sprite IconAreaSprite
        {
            set
            {
                if (_iconArea == null)
                {
                    return;
                }

                _iconArea.sprite = value;
            }
        }

        /// <summary>
        /// The GameObject that has to be enabled/disabled when the element is filled/empty.
        /// </summary>
        [SerializeField]
        private GameObject _gameObject;

        /// <summary>
        /// Property needed to enable or disable the GUI component.
        /// </summary>
        public bool Enabled
        {
            get
            {
                if (_gameObject == null)
                {
                    return false;
                }

                return _gameObject.activeInHierarchy;
            }

            set
            {
                if (_gameObject == null)
                {
                    return;
                }

                _gameObject.SetActive(value);
            }
        }

        /// <summary>
        /// The list of the operations you could do on the element.
        /// </summary>
        private List<ListElementOperation> _operations;

        /// <summary>
        /// This property provides access to the operations list in a controlled manner.
        /// </summary>
        public List<ListElementOperation> Operations
        {
            get
            {
                if (_operations == null)
                {
                    _operations = new List<ListElementOperation>();
                }

                return _operations;
            }

            set
            {
                if (_operations == null)
                {
                    _operations = new List<ListElementOperation>();
                }
                
                _operations.Clear();

                if (value == null)
                {
                    return;
                }

                foreach (ListElementOperation listElementOperation in value)
                {
                    _operations.Add(listElementOperation);
                }
            }
        }

        /// <summary>
        /// A method to clear the GUI component.
        /// </summary>
        public void Clear()
        {
            NameAreaText = "";
            IconAreaSprite = null;
            Operations.Clear();
        }

        /// <summary>
        /// A method to select/deselect the element.
        /// </summary>
        /// <param name="selected">stores whether the element must be selected or not</param>
        /// <param name="enabledColor">
        /// stores the color that the label must have when enabled
        /// </param>
        /// <param name="disabledColor">
        /// stores the color that the label must have when disabled
        /// </param>
        public void SetSelected(bool selected, Color enabledColor, Color disabledColor)
        {
            if (_label == null)
            {
                return;
            }

            if (selected)
            {
                _label.color = enabledColor;
            } else
            {
                _label.color = disabledColor;
            }
        }
    }

    /// <summary>
    /// A class which contains the metadata of an element.
    /// </summary>
    protected class ElementMetadata
    {
        /// <summary>
        /// The name of the element.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The icon of the element.
        /// </summary>
        public Sprite Icon { get; set; }

        /// <summary>
        /// The description of the element.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// The image of the element.
        /// </summary>
        public Sprite Image { get; set; }

        /// <summary>
        /// The list of the operations you could do on the element.
        /// </summary>
        private List<ListElementOperation> _operations;

        /// <summary>
        /// This property provides access to the operations list in a controlled manner.
        /// </summary>
        public List<ListElementOperation> Operations
        {
            get
            {
                if (_operations == null)
                {
                    _operations = new List<ListElementOperation>();
                }

                return _operations;
            }

            set
            {
                if (_operations == null)
                {
                    _operations = new List<ListElementOperation>();
                }
                
                _operations.Clear();

                if (value == null)
                {
                    return;
                }

                foreach (ListElementOperation listElementOperation in value)
                {
                    _operations.Add(listElementOperation);
                }
            }
        }
        
        /// <summary>
        /// The constructor of ElementMetadata.
        /// </summary>
        /// <param name="name">The name of the element</param>
        /// <param name="icon">The icon of the element</param>
        /// <param name="description">The description of the element</param>
        /// <param name="image">The image of the element</param>
        public ElementMetadata(string name, Sprite icon,
                               string description, Sprite image)
        {
            Name = name;
            Icon = icon;
            Description = description;
            Image = image;
        }
    }

    #endregion

    /// <summary>
    /// Stores the GUI elements of the menu.
    /// </summary>
    [SerializeField]
    private List<UIListElement> _uiElements;

    /// <summary>
    /// Property that provides access in a controlled manner to the UI Elements of the menu.
    /// </summary>
    protected List<UIListElement> UIElements
    {
        get
        {
            if (_uiElements == null)
            {
                _uiElements = new List<UIListElement>();
            }

            return _uiElements;
        }
    }

    /// <summary>
    /// The color that the GUI elements must have when selected.
    /// </summary>
    [SerializeField]
    private Color enabledLabelColor;

    /// <summary>
    /// The color that the GUI elements must have when not selected.
    /// </summary>
    [SerializeField]
    private Color disabledLabelColor;
    
    /// <summary>
    /// Property that returns the number of UI Elements.
    /// </summary>
    protected int NumberOfUIElements
    {
        get
        {
            if (UIElements == null)
            {
                return 0;
            }

            return UIElements.Count;
        }
    }
    
    /// <summary>
    /// The list of the metadata of the elements that the menu must display.
    /// </summary>
    private List<ElementMetadata> _elementsMetadata;

    /// <summary>
    /// A property that provides access in a controlled manner to the metadata of the elements.
    /// </summary>
    protected List<ElementMetadata> ElementsMetadata
    {
        get
        {
            if (_elementsMetadata == null)
            {
                _elementsMetadata = new List<ElementMetadata>();
            }

            return _elementsMetadata;
        }

        set
        {
            if (_elementsMetadata == null)
            {
                _elementsMetadata = new List<ElementMetadata>();
            }
            
            _elementsMetadata.Clear();

            if (value == null)
            {
                return;
            }

            foreach (ElementMetadata elementMetadata in value)
            {
                _elementsMetadata.Add(elementMetadata);
            }
        }
    }

    #region Shared Areas

    /// <summary>
    /// The text area which contains the description of the selected element.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _descriptionArea;

    /// <summary>
    /// A property that provides access to the text of the description area.
    /// </summary>
    protected string DescriptionAreaText
    {
        get
        {
            if (_descriptionArea == null)
            {
                return null;
            }

            return _descriptionArea.text;
        }

        set
        {
            if (_descriptionArea == null)
            {
                return;
            }

            _descriptionArea.text = value;
        }
    }

    /// <summary>
    /// The image component which displays the image of the selected element.
    /// </summary>
    [SerializeField]
    private Image _imageArea;

    /// <summary>
    /// A property that provides access to the sprite of the image area.
    /// </summary>
    protected Sprite SelectedElementSprite
    {
        get
        {
            if (_imageArea == null)
            {
                return null;
            }

            return _imageArea.sprite;
        }

        set
        {
            if (_imageArea == null)
            {
                return;
            }

            _imageArea.sprite = value;
        }
    }

    #endregion

    /// <summary>
    /// The index of the selected element
    /// </summary>
    private int _selectedElementIndex;

    /// <summary>
    /// A property that allows to change the selected element in a controlled manner.
    /// </summary>
    public int SelectedElementIndex
    {
        get
        {
            if (ElementsMetadata.Count == 0)
            {
                _selectedElementIndex = 0;
            }
            
            return _selectedElementIndex;
        }

        set
        {
            if (ElementsMetadata.Count == 0)
            {
                _selectedElementIndex = 0;
                return;
            }

            int oldValue = _selectedElementIndex;
            _selectedElementIndex = Mathf.Clamp(value, 1, ElementsMetadata.Count);

            int displacement = _selectedElementIndex - oldValue;

            int selectedUIElementIndex = _selectedElementIndex - FirstElementIndex + 1;
            
            if (selectedUIElementIndex < 1 || 
                selectedUIElementIndex > NumberOfUIElements)
            {
                FirstElementIndex += displacement;
            }

            selectedUIElementIndex = _selectedElementIndex - FirstElementIndex + 1;

            for (int i = 0; i < NumberOfUIElements; i++)
            {
                bool selected = i == selectedUIElementIndex - 1;
                UIElements[i].SetSelected(selected, enabledLabelColor, disabledLabelColor);
            }

            #region Debug

            ClearLog();
            Debug.Log(ElementsMetadata[SelectedElementIndex - 1].Name);

            #endregion
        }
    }

    #region Debug

    public void ClearLog()
    {
        var assembly = System.Reflection
            .Assembly.GetAssembly(typeof(UnityEditor.Editor));
        var type = assembly.GetType("UnityEditor.LogEntries");
        var method = type.GetMethod("Clear");
        method.Invoke(new object(), null);
    }

    #endregion

    /// <summary>
    /// The index of the first element displayed.
    /// </summary>
    private int _firstElementIndex;

    /// <summary>
    /// A property that allows to change the first element displayed.
    /// </summary>
    private int FirstElementIndex
    {
        get
        {
            if (ElementsMetadata.Count == 0)
            {
                _firstElementIndex = 0;
            }
            
            return _firstElementIndex;
        }

        set
        {
            if (ElementsMetadata.Count == 0)
            {
                _firstElementIndex = 0;
                return;
            }
            
            int maxValue = Mathf.Max(1, ElementsMetadata.Count - NumberOfUIElements + 1);

            int oldValue = FirstElementIndex;
            _firstElementIndex = Mathf.Clamp(value, 1, maxValue);

            UpdateUIElements();
        }
    }

    /// <summary>
    /// A property which returns the index of the last element displayed.
    /// </summary>
    private int LastElementIndex
    {
        get
        {
            return SelectedElementIndex + NumberOfUIElements;
        }
    }

    public void Start()
    {
        UpdateElements();
        FirstElementIndex = 1;
        SelectedElementIndex = 1;
    }

    #region Methods

    /// <summary>
    /// Method that updates the displayed elements.
    /// </summary>
    private void UpdateUIElements()
    {
        if (NumberOfUIElements <= 0)
        {
            return;
        }

        int counter = FirstElementIndex;

        foreach (UIListElement uiElement in UIElements)
        {
            if (!GetElement(counter, uiElement))
            {
                uiElement.Clear();
                uiElement.Enabled = false;
            }
            else
            {
                uiElement.Enabled = true;
            }

            counter++;
        }
    }
    
    /// <summary>
    /// Method that fills a GUI component with the data of an element in the desired position.
    /// </summary>
    /// <param name="index">The index of the element; starts from 1, not from 0</param>
    /// <param name="listElement">The GUI component to fill</param>
    /// <returns></returns>
    private bool GetElement(int index, UIListElement listElement)
    {
        if (index < 1 || index > ElementsMetadata.Count)
        {
            return false;
        }

        ElementMetadata currentElement = ElementsMetadata[index - 1];

        listElement.NameAreaText = currentElement.Name;

        return true;
    }

    /// <summary>
    /// Method that fills the list containing the metadata of the elements.
    /// It depends on the specific class that extends ListMenu.
    /// </summary>
    protected abstract void FillElementsMetadata();

    /// <summary>
    /// Method that updates the elements and the view.
    /// </summary>
    protected void UpdateElements()
    {
        FillElementsMetadata();
        UpdateUIElements();
        FirstElementIndex = FirstElementIndex;
        SelectedElementIndex = SelectedElementIndex;
    }

    #endregion
}
