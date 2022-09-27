using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MultiButtonsMenu
{
    [SerializeField]
    private Button _playDemoButton;

    public Button PlayDemoButton
    {
        get
        {
            return _playDemoButton;
        }
    }

    private void Start()
    {
        SelectedButtonIndex = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (SelectedButtonIndex > 1)
            {
                AudioClipHandler.PlayAudio("Audio/SelectButton", 0, transform.position);
                SelectedButtonIndex--;
            } else
            {
                AudioClipHandler.PlayAudio("Audio/Disabled", 0, transform.position);
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (SelectedButtonIndex < NumberOfButtons)
            {
                AudioClipHandler.PlayAudio("Audio/SelectButton", 0, transform.position);
                SelectedButtonIndex++;
            } else
            {
                AudioClipHandler.PlayAudio("Audio/Disabled", 0, transform.position);
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (SelectedButton != null)
            {
                AudioClipHandler.PlayAudio("Audio/PressButton", 0, transform.position, false, 1, false);
                SelectedButton.onClick.Invoke();
            }
        }
    }
}
