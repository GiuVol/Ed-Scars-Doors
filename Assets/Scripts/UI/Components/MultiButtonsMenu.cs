using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public abstract class MultiButtonsMenu : MonoBehaviour
{
    #region Buttons Logic

    [System.Serializable]
    public class ButtonInfo
    {
        public static implicit operator ButtonInfo(Button b) => new ButtonInfo(b, !b.enabled);

        public static implicit operator Button(ButtonInfo bi) => (bi != null) ? bi._button : null;

        [SerializeField]
        private Button _button;

        public Button Button
        {
            get
            {
                return _button;
            }
        }

        [SerializeField]
        private bool _disabled;

        public bool Disabled
        {
            get
            {
                return _disabled;
            }
        }

        public ButtonInfo(Button button, bool enabled)
        {
            _button = button;
            _disabled = enabled;
        }
    }

    [SerializeField]
    private List<ButtonInfo> _menuButtons;

    private List<ButtonInfo> MenuButtons
    {
        get
        {
            if (_menuButtons == null)
            {
                _menuButtons = new List<ButtonInfo>();
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
                ButtonInfo currentButtonInfo = _menuButtons[i - 1];

                if (currentButtonInfo == null || currentButtonInfo.Button == null)
                {
                    continue;
                }

                TextMeshProUGUI currentButtonText = currentButtonInfo.Button.GetComponentInChildren<TextMeshProUGUI>();

                Color selectedColor;

                if (_selectedButtonIndex == i)
                {
                    selectedColor = (currentButtonInfo.Disabled) ? _selectedDisabledTextColor : _selectedEnabledTextColor;
                } else
                {
                    selectedColor = (currentButtonInfo.Disabled) ? _unselectedDisabledTextColor : _unselectedEnabledTextColor;
                }

                if (currentButtonText != null)
                {
                    currentButtonText.color = selectedColor;
                }
            }
        }
    }

    [SerializeField]
    private Color _selectedEnabledTextColor;

    [SerializeField]
    private Color _unselectedEnabledTextColor;

    [SerializeField]
    private Color _selectedDisabledTextColor;

    [SerializeField]
    private Color _unselectedDisabledTextColor;
    
    public int NumberOfButtons
    {
        get
        {
            return MenuButtons.Count;
        }
    }

    public ButtonInfo SelectedButtonInfo
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
    
    public Button SelectedButton
    {
        get
        {
            return SelectedButtonInfo.Button;
        }
    }

    #endregion
}
