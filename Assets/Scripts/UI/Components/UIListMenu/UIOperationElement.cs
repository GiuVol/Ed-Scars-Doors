using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
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
        if (selected)
        {
            _label.color = selectedLabelColor;
        } else
        {
            _label.color = unselectedLabelColor;
        }
    }
}
