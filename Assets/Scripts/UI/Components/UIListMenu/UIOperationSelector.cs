using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

class UIOperationSelector : MonoBehaviour
{
    [SerializeField]
    private Image _background;

    [SerializeField]
    private UIOperationElement _operationElementPrefab;

    private List<UIOperationElement> _uiOperationElements;

    [SerializeField]
    private Color _selectedLabelColor;

    [SerializeField]
    private Color _unselectedLabelColor;

    private void Start()
    {
        _operationElementPrefab.gameObject.SetActive(false);
        //gameObject.SetActive(false);
    }

    public void PromptOperations(List<UIListMenu.ListElementOperation> operations)
    {
        gameObject.SetActive(true);

        int numberOfOperations = operations.Count;

        float elementHeight = _operationElementPrefab.Label.rectTransform.rect.height;

        float width = _operationElementPrefab.Label.rectTransform.rect.width;
        float height = 
            (elementHeight * numberOfOperations) + 10;

        _background.rectTransform.sizeDelta = new Vector2(width, height);

        Vector2 startingPosition = 
            _background.transform.position + Vector3.up * (height / 2);
        int counter = 1;

        foreach (UIListMenu.ListElementOperation operation in operations)
        {
            UIOperationElement newOperationElement = 
                Instantiate(_operationElementPrefab, _background.transform);
            newOperationElement.transform.position = 
                startingPosition + Vector2.down * (counter * elementHeight);
            newOperationElement.NameArea.text = operation.Name;

            counter++;
        }
    }
}
