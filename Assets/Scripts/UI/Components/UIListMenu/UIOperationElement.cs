using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIOperationElement : MonoBehaviour
{
    [SerializeField]
    private Image _label;

    public Image Label
    {
        get
        {
            return _label;
        }

        set
        {
            _label = value;
        }
    }

    [SerializeField]
    private TextMeshProUGUI _nameArea;

    public TextMeshProUGUI NameArea
    {
        get
        {
            return _nameArea;
        }

        set
        {
            _nameArea = value;
        }
    }

    public void SetSelected(bool selected, Color selectedLabelColor, Color unselectedLabelColor)
    {
        if (Label != null)
        {
            if (selected)
            {
                _label.color = selectedLabelColor;
            }
            else
            {
                _label.color = unselectedLabelColor;
            }
        }
    }
}
