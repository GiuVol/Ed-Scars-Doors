using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : MultiButtonsMenu
{
    [SerializeField]
    private TextMeshProUGUI _gameOverText;

    public string GameOverTextValue
    {
        set
        {
            if (_gameOverText == null)
            {
                return;
            }

            _gameOverText.text = value;
        }
    }

    /// <summary>
    /// This field stores the button to play again the demo.
    /// </summary>
    [SerializeField]
    private Button _playAgainButton;

    /// <summary>
    /// This field stores the button of to quit the game.
    /// </summary>
    [SerializeField]
    private Button _quitButton;

    /// <summary>
    /// This property returns the button to play again the demo.
    /// </summary>
    public Button PlayAgainButton
    {
        get
        {
            return _playAgainButton;
        }
    }

    /// <summary>
    /// This property returns the button of to quit the game.
    /// </summary>
    public Button QuitButton
    {
        get
        {
            return _quitButton;
        }
    }

    private void Start()
    {
        if (PlayAgainButton != null)
        {
            PlayAgainButton.onClick.AddListener(
                delegate
                {
                    if (UIManager.Instance == null)
                    {
                        return;
                    }

                    UIManager.Instance.ClearCanvas();

                    foreach (Transform canvasChildTransform in UIManager.Instance.CurrentCanvas.transform)
                    {
                        Destroy(canvasChildTransform.gameObject);
                    }

                    if (GameManager.Instance != null)
                    {
                        GameManager.Instance.StartCoroutine(GameManager.Instance.LoadScene("Corridor"));
                        GameManager.Instance.AudioManager.PlayOst("Audio/Ost/AStrangeTale");
                        GameManager.Instance.AudioManager.PlayAmbience("Audio/Ambience/ForestAmbience");
                    }
                }
            );
        }

        if (QuitButton != null)
        {
            QuitButton.onClick.AddListener(
                delegate
                {
                    #if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
                    #else
                    Application.Quit();
                    #endif
                }
            );
        }

        AudioClipHandler.PlayAudio("Audio/SelectButton", 0, transform.position);
        SelectedButtonIndex = 1;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            if (SelectedButtonIndex > 1)
            {
                AudioClipHandler.PlayAudio("Audio/SelectButton", 0, transform.position);
                SelectedButtonIndex--;
            }
            else
            {
                AudioClipHandler.PlayAudio("Audio/Disabled", 0, transform.position);
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            if (SelectedButtonIndex < NumberOfButtons)
            {
                AudioClipHandler.PlayAudio("Audio/SelectButton", 0, transform.position);
                SelectedButtonIndex++;
            }
            else
            {
                AudioClipHandler.PlayAudio("Audio/Disabled", 0, transform.position);
            }
        }

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (SelectedButton != null)
            {
                if (SelectedButtonInfo.Disabled)
                {
                    AudioClipHandler.PlayAudio("Audio/Disabled", 0, transform.position);
                } else
                {
                    AudioClipHandler.PlayAudio("Audio/PressButton", 0, transform.position, false, 1, false);
                    SelectedButton.onClick.Invoke();
                }
            }
        }
    }
}
