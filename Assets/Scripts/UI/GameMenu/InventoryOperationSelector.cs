using UnityEngine;

public class InventoryOperationSelector : UIOperationSelector
{
    private const string UIOperationElementPath = "UI/InventoryUIOE";

    protected override UIOperationElement OperationElementPrefab
    {
        get
        {
            if (_operationElementPrefab == null)
            {
                _operationElementPrefab =
                    Resources.Load<UIOperationElement>(UIOperationElementPath);
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
