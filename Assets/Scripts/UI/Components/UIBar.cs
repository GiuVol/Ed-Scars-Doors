using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIBar : DynamicUIComponent
{
    [SerializeField]
    private Image barBackground;
    [SerializeField]
    private Image bar;
    [SerializeField]
    private TextMeshProUGUI infoTextComponent;
    [SerializeField]
    private TextMeshProUGUI valueTextComponent;

    public bool ShowInfo
    {
        set
        {
            infoTextComponent.gameObject.SetActive(value);
        }
    }
    public bool ShowValue
    {
        set
        {
            valueTextComponent.gameObject.SetActive(value);
        }
    }

    private float MaxLength { get; set; }
    private int MaxValue { get; set; }
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
        MaxLength = bar.rectTransform.rect.width;
        MaxValue = maxValue;
        CurrentValue = MaxValue;

        infoTextComponent.text = info;
        UpdateValueTextComponent();
    }

    /// <summary>
    /// Updates the value of the bar.
    /// </summary>
    /// <param name="newValue">The new value to assign</param>
    /// <returns></returns>
    public IEnumerator LerpValue(int newValue)
    {
        float oldValue = CurrentValue;
        newValue = Mathf.Clamp(newValue, 0, MaxValue);

        float currentLength;
        float height = bar.rectTransform.rect.height;
        
        float lerpFactor = 0;

        do
        {
            lerpFactor += Time.fixedDeltaTime;
            lerpFactor = Mathf.Clamp01(lerpFactor);

            CurrentValue = (int) Mathf.Lerp(oldValue, newValue, lerpFactor);

            currentLength = MaxLength * ((float) CurrentValue / (float) MaxValue);

            bar.rectTransform.sizeDelta = new Vector2(currentLength, height);
            UpdateValueTextComponent();

            yield return null;

        } while (lerpFactor < 1);
    }

    public void UpdateValue(int newValue)
    {
        CurrentValue = Mathf.Clamp(newValue, 0, MaxValue);

        float currentLength = MaxLength * ((float)CurrentValue / (float)MaxValue);
        float height = bar.rectTransform.rect.height;

        bar.rectTransform.sizeDelta = new Vector2(currentLength, height);
    }

    /// <summary>
    /// Updates the text value of the bar.
    /// </summary>
    private void UpdateValueTextComponent()
    {
        valueTextComponent.text = CurrentValue.ToString() + "/" + MaxValue.ToString();
    }
}
