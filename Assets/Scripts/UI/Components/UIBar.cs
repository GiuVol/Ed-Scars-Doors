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

        _infoTextComponent.text = info;
        UpdateValueTextComponent();
    }

    /// <summary>
    /// Lerps the new value of the bar.
    /// </summary>
    /// <param name="newValue">The new value to assign</param>
    public IEnumerator LerpValue(int newValue)
    {
        float oldValue = CurrentValue;
        newValue = Mathf.Clamp(newValue, 0, MaxValue);

        float currentLength;
        float height = _bar.rectTransform.rect.height;
        
        float lerpFactor = 0;

        do
        {
            lerpFactor += Time.fixedDeltaTime;
            lerpFactor = Mathf.Clamp01(lerpFactor);

            CurrentValue = (int) Mathf.Lerp(oldValue, newValue, lerpFactor);

            currentLength = MaxLength * ((float) CurrentValue / (float) MaxValue);

            _bar.rectTransform.sizeDelta = new Vector2(currentLength, height);
            UpdateValueTextComponent();

            yield return null;

        } while (lerpFactor < 1);
    }

    /// <summary>
    /// Updates the value of the bar.
    /// </summary>
    /// <param name="newValue">The new value to assign</param>
    public void UpdateValue(int newValue)
    {
        CurrentValue = Mathf.Clamp(newValue, 0, MaxValue);

        float currentLength = MaxLength * ((float)CurrentValue / (float)MaxValue);
        float height = _bar.rectTransform.rect.height;

        _bar.rectTransform.sizeDelta = new Vector2(currentLength, height);
    }

    /// <summary>
    /// Updates the text value of the bar.
    /// </summary>
    private void UpdateValueTextComponent()
    {
        _valueTextComponent.text = CurrentValue.ToString() + "/" + MaxValue.ToString();
    }
}