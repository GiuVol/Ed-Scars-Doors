using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class UIOperationSelector : MonoBehaviour
{
    [SerializeField]
    private Image _background;

    protected UIOperationElement _operationElementPrefab;
    
    protected abstract UIOperationElement OperationElementPrefab { get; }

    [SerializeField]
    private Color _selectedLabelColor;

    [SerializeField]
    private Color _unselectedLabelColor;

    [SerializeField]
    private float _xPadding;

    [SerializeField]
    private float _yPadding;
    
    [SerializeField]
    private float _distanceBetweenElements;
    
    private List<UIListMenu.ListElementOperation> _operations;

    private List<UIListMenu.ListElementOperation> Operations
    {
        get
        {
            if (_operations == null)
            {
                _operations = new List<UIListMenu.ListElementOperation>();
            }

            return _operations;
        }

        set
        {
            if (_operations == null)
            {
                _operations = new List<UIListMenu.ListElementOperation>();
            }

            _operations.Clear();

            if (value == null)
            {
                return;
            }

            foreach (UIListMenu.ListElementOperation operation in value)
            {
                _operations.Add(operation);
            }
        }
    }

    protected int NumberOfOperations
    {
        get
        {
            return Operations.Count;
        }
    }

    private List<UIOperationElement> _uiOperationElements;

    private List<UIOperationElement> UIOperationElements
    {
        get
        {
            if (_uiOperationElements == null)
            {
                _uiOperationElements = new List<UIOperationElement>();
            }

            return _uiOperationElements;
        }

        set
        {
            if (_uiOperationElements == null)
            {
                _uiOperationElements = new List<UIOperationElement>();
            }

            _uiOperationElements.Clear();

            if (value == null)
            {
                return;
            }

            foreach (UIOperationElement operationElement in value)
            {
                _uiOperationElements.Add(operationElement);
            }
        }
    }

    private int _selectedOperationIndex;

    public int SelectedOperationIndex
    {
        get
        {
            if (NumberOfOperations == 0)
            {
                return 0;
            }

            return Mathf.Clamp(_selectedOperationIndex, 1, NumberOfOperations);
        }

        set
        {
            if (NumberOfOperations == 0)
            {
                _selectedOperationIndex = 0;
                return;
            }

            int oldValue = _selectedOperationIndex;
            _selectedOperationIndex = Mathf.Clamp(value, 1, NumberOfOperations);

            int index = 1;

            foreach (UIOperationElement operationElement in UIOperationElements)
            {
                bool active = index == _selectedOperationIndex;

                operationElement.SetSelected(active, _selectedLabelColor, _unselectedLabelColor);

                index++;
            }
        }
    }

    private UIListMenu.ListElementOperation _selectedOperation;

    public UIListMenu.ListElementOperation SelectedOperation
    {
        get
        {
            return _selectedOperation;
        }

        private set
        {
            _selectedOperation = value;
        }
    }

    public bool PromptOperations(List<UIListMenu.ListElementOperation> operations)
    {
        #region Pre-condition check

        if (OperationElementPrefab == null)
        {
            return false;
        }

        if (OperationElementPrefab.Label == null)
        {
            return false;
        }

        if (_background == null)
        {
            return false;
        }

        #endregion

        Operations = operations;

        int numberOfOperations = Operations.Count;

        if (numberOfOperations == 0)
        {
            return false;
        }

        UIOperationElement operationElementPrefab = OperationElementPrefab;

        float elementHeight = operationElementPrefab.Label.rectTransform.rect.height;

        float width = operationElementPrefab.Label.rectTransform.rect.width + (_xPadding * 2);
        float height = (elementHeight * numberOfOperations) + 
            (_distanceBetweenElements * (numberOfOperations - 1)) + (_yPadding * 2);
        
        _background.rectTransform.sizeDelta = new Vector2(width, height);

        Vector2 startingPosition = 
            _background.transform.position + 
            Vector3.up * (((height / 2) - _yPadding) - (elementHeight / 2));
        int counter = 1;

        foreach (UIListMenu.ListElementOperation operation in Operations)
        {
            UIOperationElement newOperationElement = 
                Instantiate(operationElementPrefab, _background.transform);
            newOperationElement.transform.position = 
                startingPosition + 
                Vector2.down * ((counter - 1) * (elementHeight + _distanceBetweenElements));
            newOperationElement.NameArea.text = operation.Name;
            newOperationElement.gameObject.SetActive(true);

            UIOperationElements.Add(newOperationElement);

            counter++;
        }

        SelectedOperationIndex = 1;

        return true;
    }

    public void SelectOperation()
    {
        if (NumberOfOperations > 0)
        {
            SelectedOperation = Operations[_selectedOperationIndex - 1];
        }
    }

    public void Clear()
    {
        Operations.Clear();

        foreach (UIOperationElement operationElement in _uiOperationElements)
        {
            Destroy(operationElement.gameObject);
        }

        _uiOperationElements.Clear();
        _selectedOperationIndex = 0;
        SelectedOperation = null;
    }
}
