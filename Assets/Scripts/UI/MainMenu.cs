using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    #region Buttons Logic

    [SerializeField]
    private List<Button> _menuButtons;

    private List<Button> MenuButtons
    {
        get
        {
            if (_menuButtons == null)
            {
                _menuButtons = new List<Button>();
            }

            return _menuButtons;
        }
    }

    private int _selectedButtonIndex;

    public int SelectedButtonIndex
    {
        get
        {
            if (MenuButtons.Count <= 0)
            {
                return 0;
            }

            return Mathf.Clamp(_selectedButtonIndex, 1, MenuButtons.Count);
        }

        set
        {
            if (MenuButtons.Count <= 0)
            {
                _selectedButtonIndex = 0;
                return;
            }

            int oldValue = _selectedButtonIndex;
            _selectedButtonIndex = Mathf.Clamp(value, 1, MenuButtons.Count);

            if (_selectedButtonIndex == oldValue)
            {
                return;
            }

            for (int i = 1; i <= MenuButtons.Count; i++)
            {
                Button currentButton = _menuButtons[i - 1];

                if (currentButton == null)
                {
                    continue;
                }

                TextMeshProUGUI currentButtonText = currentButton.GetComponentInChildren<TextMeshProUGUI>();

                Color selectedColor = (_selectedButtonIndex == i) ? _enabledTextColor : _disabledTextColor;

                if (currentButtonText != null)
                {
                    currentButtonText.color = selectedColor;
                }
            }
        }
    }

    [SerializeField]
    private Color _enabledTextColor;

    [SerializeField]
    private Color _disabledTextColor;
    
    public Button SelectedButton
    {
        get
        {
            if (MenuButtons.Count <= 0)
            {
                return null;
            }

            return MenuButtons[SelectedButtonIndex - 1];
        }
    }

    #endregion

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
