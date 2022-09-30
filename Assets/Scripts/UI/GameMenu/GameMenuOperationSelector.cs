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
        if (InputHandler.Up("Down"))
        {
            if (SelectedOperationIndex > 1)
            {
                AudioClipHandler.PlayAudio("Audio/SelectButton", 0, transform.position);
                SelectedOperationIndex--;
            } else
            {
                AudioClipHandler.PlayAudio("Audio/Disabled", 0, transform.position);
            }
        }

        if (InputHandler.Down("Down"))
        {
            if (SelectedOperationIndex < NumberOfOperations)
            {
                AudioClipHandler.PlayAudio("Audio/SelectButton", 0, transform.position);
                SelectedOperationIndex++;
            } else
            {
                AudioClipHandler.PlayAudio("Audio/Disabled", 0, transform.position);
            }
        }

        if (InputHandler.Submit("Down"))
        {
            SelectOperation();
        }
    }

}
