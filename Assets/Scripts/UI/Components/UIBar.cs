using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBar : DynamicUIComponent
{
    #region Serialized

    /// <summary>
    /// The background of the bar.
    /// </summary>
    [SerializeField]
    private Image _barBackground;

    /// <summary>
    /// The slider of the bar.
    /// </summary>
    [SerializeField]
    private Image _slider;

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

    #endregion

    /// <summary>
    /// The max value that the slider can represent.
    /// </summary>
    private float MaxValue { get; set; }

    /// <summary>
    /// The max length that the slider can have.
    /// </summary>
    private float MaxLength { get; set; }
    
    /// <summary>
    /// The current value that the slider represents.
    /// </summary>
    public float CurrentValue { get; private set; }

    /// <summary>
    /// The current length of the slider.
    /// </summary>
    private float CurrentLength { get; set; }
    
    /// <summary>
    /// The target value that the slider should represent.
    /// </summary>
    private float ActualValue { get; set; }

    /// <summary>
    /// The target length that the slider should reach.
    /// </summary>
    private float ActualLength { get; set; }
    
    /// <summary>
    /// Returns whether the bar has reached its stable value or not.
    /// </summary>
    public bool IsStable { get; private set; }

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
    /// Support variable used to calculate the current value smoothly.
    /// </summary>
    private float _currentRefVelocity;
    
    #region Setup

    public void InitializeDynamic(Transform targetToFollow, Vector3 positionOffset, 
                                  float maxValue, string info = "")
    {
        base.InitializeDynamic(targetToFollow, positionOffset);
        SetupBar(maxValue, info);
    }

    public void InitializeStatic(float maxValue, string info = "")
    {
        base.InitializeStatic();
        SetupBar(maxValue, info);
    }

    /// <summary>
    /// This method is used to initialize the bar.
    /// </summary>
    /// <param name="maxValue">the max value that the bar can represent</param>
    /// <param name="info">the info text displayed near the bar</param>
    private void SetupBar(float maxValue, string info)
    {
        MaxValue = maxValue;
        MaxLength = _slider.rectTransform.rect.width;
        CurrentValue = MaxValue;
        CurrentLength = MaxLength;
        ActualValue = CurrentValue;
        ActualLength = CurrentLength;

        if (_infoTextComponent != null)
        {
            _infoTextComponent.text = info;
        }

        UpdateValueTextComponent();
    }

    #endregion

    protected new void Update()
    {
        base.Update();

        //Lerping value.
        if (Mathf.Abs(ActualValue - CurrentValue) >= .1f || Mathf.Abs(ActualLength - CurrentLength) >= .1f)
        {
            CurrentValue = Mathf.SmoothDamp(CurrentValue, ActualValue, ref _currentRefVelocity, .05f);
            CurrentLength = MaxLength * (CurrentValue / MaxValue);
            IsStable = false;
        } else
        {
            CurrentValue = ActualValue;
            CurrentLength = ActualLength;
            IsStable = true;
        }

        _slider.rectTransform.sizeDelta = new Vector2(CurrentLength, _slider.rectTransform.sizeDelta.y);

        UpdateValueTextComponent();
    }

    /// <summary>
    /// Updates the value of the bar.
    /// </summary>
    /// <param name="newValue">The new value to assign</param>
    public void UpdateValue(float newValue)
    {
        ActualValue = Mathf.Clamp(newValue, 0, MaxValue);
        ActualLength = MaxLength * (ActualValue / MaxValue);
    }

    /// <summary>
    /// Updates the value of the bar.
    /// </summary>
    /// <param name="newValue">The new value to assign</param>
    public void UpdateValueInstantly(float newValue)
    {
        ActualValue = Mathf.Clamp(newValue, 0, MaxValue);
        ActualLength = MaxLength * (ActualValue / MaxValue);
        CurrentValue = ActualValue;
        CurrentLength = ActualLength;
        _slider.rectTransform.sizeDelta = new Vector2(CurrentLength, _slider.rectTransform.sizeDelta.y);
    }
    
    /// <summary>
    /// Updates the max value of the bar.
    /// </summary>
    /// <param name="newMaxValue">The new max value</param>
    public void UpdateMaxValue(float newMaxValue)
    {
        MaxValue = Mathf.Max(newMaxValue, 1);
        CurrentValue = Mathf.Clamp(CurrentValue, 0, MaxValue);
        ActualValue = CurrentValue;
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

        _valueTextComponent.text = Mathf.RoundToInt(CurrentValue).ToString() + "/" + Mathf.RoundToInt(MaxValue).ToString();
    }
}
