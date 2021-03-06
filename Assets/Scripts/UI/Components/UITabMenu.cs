using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UITabMenu : MonoBehaviour
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
                if (_image == null || _text == null)
                {
                    return;
                }
                
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
        private GameObject _content;

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
            if (_label != null)
            {
                _label.SetColors(active, enabledColor, 
                    enabledTextColor, disabledColor, disabledTextColor);
            }

            if (_content != null)
            {
                _content.SetActive(active);

                ITabContent tabContent = _content.GetComponent<ITabContent>();

                if (tabContent != null)
                {
                    tabContent.Activate(active);
                }
            }
        }
    }

    /// <summary>
    /// List that contains the tabs of the menu.
    /// </summary>
    [SerializeField]
    private List<Tab> _tabs;
    
    /// <summary>
    /// Property that provides access to the tabs in a controlled manner.
    /// </summary>
    private List<Tab> Tabs
    {
        get
        {
            if (_tabs == null)
            {
                _tabs = new List<Tab>();
            }

            return _tabs;
        }
    }

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
            if (Tabs == null)
            {
                return 0;
            }

            return Tabs.Count;
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

                Tabs[i].SetActive(active, _enabledTabLabelColor, _enabledTabTextColor, _disabledTabLabelColor, _disabledTabTextColor);
            }
        }
    }

    protected void Start()
    {
        for (int i = 0; i < NumberOfTabs; i++)
        {
            Tabs[i].SetActive(false, _enabledTabLabelColor, _enabledTabTextColor, _disabledTabLabelColor, _disabledTabTextColor);
        }

        SelectedTab = 1;
    }
}
