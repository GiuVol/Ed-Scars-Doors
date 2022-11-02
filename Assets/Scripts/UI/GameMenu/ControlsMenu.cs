using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ControlsMenu : MonoBehaviour
{
    [SerializeField]
    private Image _controlsImageArea;

    [SerializeField]
    private Image _previousArrowImage;

    [SerializeField]
    private Image _nextArrowImage;

    [SerializeField]
    private TextMeshProUGUI _titleArea;
    
    [SerializeField]
    private List<Sprite> _controlsSheets;

    [SerializeField]
    private List<string> _sheetsTitles;

    private List<Sprite> ControlsSheets
    {
        get
        {
            if (_controlsSheets == null)
            {
                _controlsSheets = new List<Sprite>();
            }

            return _controlsSheets;
        }
    }

    private int NumberOfSheets
    {
        get
        {
            return ControlsSheets.Count;
        }
    }

    private List<string> SheetsTitles
    {
        get
        {
            if (_sheetsTitles == null)
            {
                _sheetsTitles = new List<string>();
            }

            return _sheetsTitles;
        }
    }

    private int _selectedSheetIndex;

    public int SelectedSheetIndex
    {
        get
        {
            if (ControlsSheets.Count <= 0)
            {
                return 0;
            }

            return Mathf.Clamp(_selectedSheetIndex, 1, ControlsSheets.Count);
        }

        set
        {
            if (ControlsSheets.Count <= 0)
            {
                _selectedSheetIndex = 0;
                return;
            }

            int oldValue = _selectedSheetIndex;
            _selectedSheetIndex = Mathf.Clamp(value, 1, ControlsSheets.Count);

            if (_selectedSheetIndex == oldValue)
            {
                return;
            }

            Sprite selectedSheet = ControlsSheets[_selectedSheetIndex - 1];

            if (_controlsImageArea != null)
            {
                if (selectedSheet != null)
                {
                    _controlsImageArea.sprite = selectedSheet;
                    _controlsImageArea.color = new Color(1, 1, 1, 1);
                } else
                {
                    _controlsImageArea.sprite = null;
                    _controlsImageArea.color = new Color(1, 1, 1, 0);
                }
            }

            UpdateArrows();

            if (SheetsTitles.Count >= SelectedSheetIndex)
            {
                if (_titleArea != null)
                {
                    _titleArea.text = SheetsTitles[SelectedSheetIndex - 1];
                }
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        if (UIManager.Instance.GameMenu)
        {
            UIManager.Instance.GameMenu.gameObject.SetActive(false);
            UIManager.Instance.GameMenu.HasControl = false;
            Time.timeScale = 0;
        }

        if (GameManager.Instance != null)
        {
            if (GameManager.Instance.Player != null)
            {
                GameManager.Instance.Player.HasControl = false;
            }
        }
        
        if (_controlsImageArea != null)
        {
            _controlsImageArea.color = new Color(1, 1, 1, 0);
        }

        if (_previousArrowImage != null)
        {
            if (_previousArrowImage.sprite == null)
            {
                _previousArrowImage.color = new Color(1, 1, 1, 0);
            }
        }

        if (_nextArrowImage != null)
        {
            if (_nextArrowImage.sprite == null)
            {
                _nextArrowImage.color = new Color(1, 1, 1, 0);
            }
        }
        
        SelectedSheetIndex = 1;
    }

    private void Update()
    {
        if (InputHandler.Left("Down"))
        {
            if (SelectedSheetIndex > 1)
            {
                AudioClipHandler.PlayAudio("Audio/SelectTab", 0, transform.position, false, .5f);
                SelectedSheetIndex--;
            } else
            {
                AudioClipHandler.PlayAudio("Audio/Disabled", 0, transform.position, false, .8f);
            }
        }

        if (InputHandler.Right("Down"))
        {
            if (SelectedSheetIndex < NumberOfSheets)
            {
                AudioClipHandler.PlayAudio("Audio/SelectTab", 0, transform.position, false, .5f);
                SelectedSheetIndex++;
            } else
            {
                AudioClipHandler.PlayAudio("Audio/Disabled", 0, transform.position, false, .8f);
            }
        }

        if (InputHandler.CloseMenu("Down"))
        {
            AudioClipHandler.PlayAudio("Audio/SelectButton", 0, transform.position, false, .5f);
            Destroy(gameObject);
        }
    }

    private void UpdateArrows()
    {
        if (SelectedSheetIndex <= 1)
        {
            if (_previousArrowImage != null)
            {
                _previousArrowImage.color = new Color(1, 1, 1, 0);
            }
        } else
        {
            if (_previousArrowImage != null)
            {
                if (_previousArrowImage.sprite == null)
                {
                    _previousArrowImage.color = new Color(1, 1, 1, 0);
                } else
                {
                    _previousArrowImage.color = new Color(1, 1, 1, 1);
                }
            }
        }

        if (SelectedSheetIndex >= NumberOfSheets)
        {
            if (_nextArrowImage != null)
            {
                _nextArrowImage.color = new Color(1, 1, 1, 0);
            }
        } else
        {
            if (_nextArrowImage.sprite == null)
            {
                _nextArrowImage.color = new Color(1, 1, 1, 0);
            }
            else
            {
                _nextArrowImage.color = new Color(1, 1, 1, 1);
            }
        }
    }

    private void OnDisable()
    {
        if (UIManager.Instance.GameMenu)
        {
            UIManager.Instance.GameMenu.gameObject.SetActive(true);
            UIManager.Instance.GameMenu.HasControl = true;
        }
    }

    private void OnDestroy()
    {
        if (UIManager.Instance.GameMenu)
        {
            UIManager.Instance.GameMenu.gameObject.SetActive(true);
            UIManager.Instance.GameMenu.HasControl = true;
        }
    }
}
