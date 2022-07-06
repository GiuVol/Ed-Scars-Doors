using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TabMenu : MonoBehaviour
{
    /// <summary>
    /// Class that represents a tab of the menu.
    /// </summary>
    [System.Serializable]
    public class Tab
    {
        /// <summary>
        /// Class that represents the label of the tab.
        /// </summary>
        [System.Serializable]
        public class TabLabel
        {
            /// <summary>
            /// The image of the label.
            /// </summary>
            [SerializeField]
            private Image _image;

            /// <summary>
            /// The text of the label.
            /// </summary>
            [SerializeField]
            private TextMeshProUGUI _text;

            /// <summary>
            /// stores whether the label should use the colors passed from the outside, or use its custom colors.
            /// </summary>
            [SerializeField]
            private bool _useCustomColors;

            /// <summary>
            /// The color that the image of the label must have when enabled, if the label uses custom colors.
            /// </summary>
            [SerializeField]
            private Color _enabledLabelColor;

            /// <summary>
            /// The color that the text of the label must have when enabled, if the label uses custom colors.
            /// </summary>
            [SerializeField]
            private Color _enabledTextColor;

            /// <summary>
            /// The color that the image of the label must have when disabled, if the label uses custom colors.
            /// </summary>
            [SerializeField]
            private Color _disabledLabelColor;

            /// <summary>
            /// The color that the text of the label must have when disabled, if the label uses custom colors.
            /// </summary>
            [SerializeField]
            private Color _disabledTextColor;

            /// <summary>
            /// This method changes colors of the label.
            /// </summary>
            /// <param name="active">
            /// This parameter decides if the label should be enabled or disabled.
            /// </param>
            /// <param name="enabledColor">
            /// The color that the image of the label should have when the tab is enabled, if the label doesn't use custom colors.
            /// </param>
            /// <param name="enabledTextColor">
            /// The color that the text of the label should have when the tab is enabled, if the label doesn't use custom colors.
            /// </param>
            /// <param name="disabledColor">
            /// The color that the image of the label should have when the tab is disabled, if the label doesn't use custom colors.
            /// </param>
            /// <param name="disabledTextColor">
            /// The color that the text of the label should have when the tab is disabled, if the label doesn't use custom colors.
            /// </param>
            public void SetColors(bool active, Color enabledColor, Color enabledTextColor, Color disabledColor, Color disabledTextColor)
            {
                if (_useCustomColors)
                {
                    if (active)
                    {
                        _image.color = _enabledLabelColor;
                        _text.color = _enabledTextColor;
                    }
                    else
                    {
                        _image.color = _disabledLabelColor;
                        _text.color = _disabledTextColor;
                    }
                }
                else
                {
                    if (active)
                    {
                        _image.color = enabledColor;
                        _text.color = enabledTextColor;
                    }
                    else
                    {
                        _image.color = disabledColor;
                        _text.color = disabledTextColor;
                    }
                }
            }
        }

        /// <summary>
        /// The label of the tab.
        /// </summary>
        [SerializeField]
        private TabLabel _label;

        /// <summary>
        /// The <c>GameObject</c> that contains the content of the tab.
        /// </summary>
        [SerializeField]
        private GameObject _menu;

        /// <summary>
        /// Enables or disables the tab.
        /// </summary>
        /// <param name="active">
        /// This parameter decides if the tab should be enabled or disabled.
        /// </param>
        /// <param name="enabledColor">
        /// The color that the image of the label should have when the tab is enabled, if the label doesn't use custom colors.
        /// </param>
        /// <param name="enabledTextColor">
        /// The color that the text of the label should have when the tab is enabled, if the label doesn't use custom colors.
        /// </param>
        /// <param name="disabledColor">
        /// The color that the image of the label should have when the tab is disabled, if the label doesn't use custom colors.
        /// </param>
        /// <param name="disabledTextColor">
        /// The color that the text of the label should have when the tab is disabled, if the label doesn't use custom colors.
        /// </param>
        public void SetActive(bool active, Color enabledColor, Color enabledTextColor, Color disabledColor, Color disabledTextColor)
        {
            _label.SetColors(active, enabledColor, enabledTextColor, disabledColor, disabledTextColor);
            _menu.SetActive(active);
        }
    }

    /// <summary>
    /// List that contains the tabs of the menu.
    /// </summary>
    [SerializeField]
    private List<Tab> _tabs;
    
    /// <summary>
    /// The standard color that the tabs labels should have when enabled.
    /// </summary>
    [SerializeField]
    private Color _enabledTabLabelColor;

    /// <summary>
    /// The standard color that the tabs texts should have when enabled.
    /// </summary>
    [SerializeField]
    private Color _enabledTabTextColor;

    /// <summary>
    /// The standard color that the tabs labels should have when disabled.
    /// </summary>
    [SerializeField]
    private Color _disabledTabLabelColor;

    /// <summary>
    /// The standard color that the tabs texts should have when disabled.
    /// </summary>
    [SerializeField]
    private Color _disabledTabTextColor;

    /// <summary>
    /// Returns the number of tabs that the menu has.
    /// </summary>
    public int NumberOfTabs
    {
        get
        {
            if (_tabs == null)
            {
                return 0;
            }

            return _tabs.Count;
        }
    }

    /// <summary>
    /// Stores the selected tab's index.
    /// </summary>
    private int _selectedTabIndex;

    /// <summary>
    /// provides access to the selected tab in a controlled way.
    /// </summary>
    public int SelectedTab
    {
        get
        {
            if (NumberOfTabs == 0)
            {
                return 0;
            }

            return Mathf.Clamp(_selectedTabIndex, 1, NumberOfTabs);
        }
        set
        {
            if (NumberOfTabs == 0)
            {
                _selectedTabIndex = 0;
                return;
            }

            float oldValue = _selectedTabIndex;
            
            _selectedTabIndex = Mathf.Clamp(value, 1, NumberOfTabs);

            if (_selectedTabIndex == oldValue)
            {
                return;
            }

            for (int i = 0; i < NumberOfTabs; i++)
            {
                bool active = (i == _selectedTabIndex - 1);

                _tabs[i].SetActive(active, _enabledTabLabelColor, _enabledTabTextColor, _disabledTabLabelColor, _disabledTabTextColor);
            }
        }
    }

    void Start()
    {
        for (int i = 0; i < NumberOfTabs; i++)
        {
            _tabs[i].SetActive(false, _enabledTabLabelColor, _enabledTabTextColor, _disabledTabLabelColor, _disabledTabTextColor);
        }

        SelectedTab = 1;
    }
}
