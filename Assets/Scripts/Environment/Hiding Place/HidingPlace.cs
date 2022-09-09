using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingPlace : MonoBehaviour
{
    private const string HideMessageResourcesPath = "UI/HideMessage";

    private DynamicUIComponent _uiHideMessage;

    void Start()
    {
        Canvas currentCanvas = FindObjectOfType<Canvas>();
        
        if (currentCanvas != null)
        {
            _uiHideMessage = Instantiate(Resources.Load<DynamicUIComponent>(HideMessageResourcesPath),
                                                         currentCanvas.transform);
            _uiHideMessage.InitializeDynamic(transform, Vector3.zero);

            _uiHideMessage.gameObject.SetActive(false);
        }
    }

    public void EnableMessage(bool enabled)
    {
        if(_uiHideMessage == null)
        {
            return;
        }

        _uiHideMessage.gameObject.SetActive(enabled);
    }
}
