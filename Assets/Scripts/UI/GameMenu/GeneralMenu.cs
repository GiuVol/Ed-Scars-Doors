using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralMenu : MultiButtonsMenu, ITabContent
{
    private const string ControlsMenuResourcePath = "UI/ControlsMenu";

    [SerializeField]
    private Button _controlsButton;

    private bool HasControl { get; set; }

    private void Start()
    {
        _controlsButton.onClick.AddListener(
            delegate
            {
                if (UIManager.Instance == null)
                {
                    return;
                }

                ControlsMenu controlsMenuResource = Resources.Load<ControlsMenu>(ControlsMenuResourcePath);

                if (UIManager.Instance.CurrentCanvas != null && controlsMenuResource != null)
                {
                    ControlsMenu controlsMenu = Instantiate(controlsMenuResource, UIManager.Instance.CurrentCanvas.transform);
                }
            }
        );

        SelectedButtonIndex = 1;
    }

    private void Update()
    {
        if (HasControl)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                if (SelectedButton != null)
                {
                    SelectedButtonIndex--;
                }
            }

            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                if (SelectedButton != null)
                {
                    SelectedButtonIndex++;
                }
            }

            if (Input.GetKeyDown(KeyCode.Return))
            {
                if (SelectedButton != null)
                {
                    SelectedButton.onClick.Invoke();
                }
            }
        }
    }

    #region Override and Implementation

    public void Activate(bool active)
    {
        HasControl = active;
    }

    #endregion
}
