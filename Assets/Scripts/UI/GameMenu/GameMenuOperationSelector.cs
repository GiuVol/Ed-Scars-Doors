using UnityEngine;

public class GameMenuOperationSelector : UIOperationSelector
{
    private const string UIOperationElementPath = "UI/OperationSelectorElement";

    protected override UIOperationElement OperationElementPrefab
    {
        get
        {
            if (_operationElementPrefab == null)
            {
                _operationElementPrefab = Resources.Load<UIOperationElement>(UIOperationElementPath);
            }

            return _operationElementPrefab;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SelectOperation();
        }

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            SelectedOperationIndex--;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SelectedOperationIndex++;
        }
    }

}
