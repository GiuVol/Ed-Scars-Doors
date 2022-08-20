using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBar : DynamicUIComponent
{
    /// <summary>
    /// The background of the bar.
    /// </summary>
    [SerializeField]
    private Image _barBackground;

    /// <summary>
    /// The slider of the bar.
    /// </summary>
    [SerializeField]
    private Image _bar;

    /// <summary>
    /// The text component that displays infos about the bar.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _infoTextComponent;

    /// <summary>
    /// The text component that displays the current value of the bar.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _valueTextComponent;

    /// <summary>
    /// This property allows to show or hide <c>_infoTextComponent</c>
    /// </summary>
    public bool ShowInfo
    {
        set
        {
            if (_infoTextComponent == null)
            {
                return;
            }

            _infoTextComponent.gameObject.SetActive(value);
        }
    }

    /// <summary>
    /// This property allows to show or hide <c>_valueTextComponent</c>
    /// </summary>
    public bool ShowValue
    {
        set
        {
            if (_valueTextComponent == null)
            {
                return;
            }
            
            _valueTextComponent.gameObject.SetActive(value);
        }
    }

    /// <summary>
    /// The max length that the slider can have.
    /// </summary>
    private float MaxLength { get; set; }

    /// <summary>
    /// The max value that the slider can represent.
    /// </summary>
    private int MaxValue { get; set; }

    /// <summary>
    /// The current value that the slider represents.
    /// </summary>
    private int CurrentValue { get; set; }

    public void InitializeDynamic(Transform targetToFollow, Vector3 positionOffset, 
                                  int maxValue, string info = "")
    {
        base.InitializeDynamic(targetToFollow, positionOffset);
        SetupBar(maxValue, info);
    }

    public void InitializeStatic(int maxValue, string info = "")
    {
        base.InitializeStatic();
        SetupBar(maxValue, info);
    }

    /// <summary>
    /// This method is used to initialize the bar.
    /// </summary>
    /// <param name="maxValue">the max value that the bar can represent</param>
    /// <param name="info">the info text displayed near the bar</param>
    private void SetupBar(int maxValue, string info)
    {
        MaxLength = _bar.rectTransform.rect.width;
        MaxValue = maxValue;
        CurrentValue = MaxValue;

        if (_infoTextComponent != null)
        {
            _infoTextComponent.text = info;
        }

        UpdateValueTextComponent();
    }

    /// <summary>
    /// Updates the value of the bar.
    /// </summary>
    /// <param name="newValue">The new value to assign</param>
    public void UpdateCurrentValue(int newValue)
    {
        CurrentValue = Mathf.Clamp(newValue, 0, MaxValue);

        float currentLength = MaxLength * ((float) CurrentValue / (float) MaxValue);
        float height = _bar.rectTransform.rect.height;

        _bar.rectTransform.sizeDelta = new Vector2(currentLength, height);

        UpdateValueTextComponent();
    }

    /// <summary>
    /// Updates the max value of the bar.
    /// </summary>
    /// <param name="newMaxValue">The new max value</param>
    public void UpdateMaxValue(int newMaxValue)
    {
        MaxValue = Mathf.Max(newMaxValue, 1);
        CurrentValue = Mathf.Clamp(CurrentValue, 0, MaxValue);

        float currentLength = MaxLength * ((float) CurrentValue / (float) MaxValue);
        float height = _bar.rectTransform.rect.height;

        _bar.rectTransform.sizeDelta = new Vector2(currentLength, height);

        UpdateValueTextComponent();
    }
    
    /// <summary>
    /// Updates the text value of the bar.
    /// </summary>
    private void UpdateValueTextComponent()
    {
        if (_valueTextComponent == null)
        {
            return;
        }

        _valueTextComponent.text = CurrentValue.ToString() + "/" + MaxValue.ToString();
    }
}
