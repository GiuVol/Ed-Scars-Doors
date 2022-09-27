using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlsMenu : MonoBehaviour
{
    [SerializeField]
    private Image _controlsImageArea;

    [SerializeField]
    private List<Sprite> _controlsSheets;

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

        SelectedSheetIndex = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            AudioClipHandler.PlayAudio("Audio/SelectTab", 0, transform.position);
            SelectedSheetIndex--;
        }

        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            AudioClipHandler.PlayAudio("Audio/SelectTab", 0, transform.position);
            SelectedSheetIndex++;
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Destroy(gameObject);
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
