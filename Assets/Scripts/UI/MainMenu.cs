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

    [SerializeField]
    private Button _quitButton;

    public Button QuitButton
    {
        get
        {
            return _quitButton;
        }
    }
    
    private void Start()
    {
        SelectedButtonIndex = 1;
    }

    private void Update()
    {
        if (InputHandler.Up("Down"))
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

        if (InputHandler.Down("Down"))
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

        if (InputHandler.Submit("Down"))
        {
            if (SelectedButton != null)
            {
                if (SelectedButtonInfo.Disabled)
                {
                    AudioClipHandler.PlayAudio("Audio/Disabled", 0, transform.position, false, 1, false);
                } else
                {
                    AudioClipHandler.PlayAudio("Audio/PressButton", 0, transform.position, false, 1, false);
                    SelectedButton.onClick.Invoke();
                }
            }
        }
    }
}
