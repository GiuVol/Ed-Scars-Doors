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
            SelectedButtonIndex--;
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            SelectedButtonIndex++;
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
