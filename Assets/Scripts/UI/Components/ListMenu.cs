using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class ListMenu : MonoBehaviour
{
    public class ListElementOperation
    {
        public delegate void OperationDelegate();

        public string Name { get; set; }

        public OperationDelegate Operation { get; set; }

        public ListElementOperation(string name, OperationDelegate operation)
        {
            Name = name;
            Operation = operation;
        }
    }

    public class ElementMetadata
    {
        public string Name { get; set; }
        public Sprite Icon { get; set; }
        public string Description { get; set; }
        public Sprite Image { get; set; }
        public List<ListElementOperation> operations { get; private set; }

        public ElementMetadata(string name, Sprite icon, 
                               string description, Sprite image)
        {
            Name = name;
            Icon = icon;
            Description = description;
            Image = image;
            operations = new List<ListElementOperation>();
        }
    }

    [System.Serializable]
    public class UIListElement
    {
        [SerializeField]
        private Image _label;
        
        [SerializeField]
        private TextMeshProUGUI _nameArea;

        public string NameAreaText
        {
            set
            {
                if (_nameArea == null)
                {
                    return;
                }

                _nameArea.text = value;
            }
        }

        [SerializeField]
        private Image _iconArea;

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

        [SerializeField]
        private GameObject _gameObject;

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

        public List<ListElementOperation> operations { get; set; }

        public void Clear()
        {
            NameAreaText = "";
            IconAreaSprite = null;
            operations = new List<ListElementOperation>();
        }

        public void SetSelected(bool selected)
        {
            if (_label == null)
            {
                return;
            }

            if (selected)
            {
                _label.color = Color.red;
            } else
            {
                _label.color = Color.white;
            }
        }
    }

    protected abstract List<ElementMetadata> ElementsMetadata
    {
        get;
    }

    [SerializeField]
    protected List<UIListElement> _uiElements;

    #region Shared Areas

    [SerializeField]
    private TextMeshProUGUI _descriptionArea;

    protected string SelectedElementDescription
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

    [SerializeField]
    private Image _imageArea;

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

    public int NumberOfUIElements
    {
        get
        {
            if (_uiElements == null)
            {
                return 0;
            }

            return _uiElements.Count;
        }
    }

    private int _selectedElementIndex;

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

            int verse = _selectedElementIndex - oldValue;

            if (verse == 0)
            {
                return;
            }

            int selectedUIElementIndex = _selectedElementIndex - FirstElementIndex;

            for (int i = 0; i < NumberOfUIElements; i++)
            {
                _uiElements[i].SetSelected(i == selectedUIElementIndex);
            }
        }
    }

    private int _firstElementIndex;

    public int FirstElementIndex
    {
        get
        {
            return _firstElementIndex;
        }

        set
        {
            int maxValue = 
                Mathf.Max(1, ElementsMetadata.Count - NumberOfUIElements + 1);

            int oldValue = FirstElementIndex;
            _firstElementIndex = Mathf.Clamp(value, 1, maxValue);

            if (oldValue == FirstElementIndex)
            {
                return;
            }

            UpdateElements();
        }
    }

    public int LastElementIndex
    {
        get
        {
            return SelectedElementIndex + NumberOfUIElements;
        }
    }

    public void Start()
    {
        FirstElementIndex = 1;
        SelectedElementIndex = 1;
        UpdateElements();
    }

    #region Deprecated

    public bool GetElement(int index, UIListElement listElement)
    {
        List<ElementMetadata> currentElementsMetadata = ElementsMetadata;

        if (index < 1 || index > currentElementsMetadata.Count)
        {
            return false;
        }

        ElementMetadata currentElement = currentElementsMetadata[index - 1];

        listElement.NameAreaText = currentElement.Name;

        return true;
    }

    public void UpdateElements()
    {
        if (NumberOfUIElements <= 0)
        {
            return;
        }

        int counter = FirstElementIndex;

        foreach (UIListElement uiElement in _uiElements)
        {
            if (!GetElement(counter, uiElement))
            {
                uiElement.Clear();
                uiElement.Enabled = false;
            } else
            {
                uiElement.Enabled = true;
            }

            counter++;
        }
    }

    #endregion
}
