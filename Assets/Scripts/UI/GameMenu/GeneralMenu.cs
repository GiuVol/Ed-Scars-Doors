using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GeneralMenu : MultiButtonsMenu, ITabContent
{
    private const string ControlsMenuResourcePath = "UI/ControlsMenu";

    [SerializeField]
    private Button _controlsButton;

    [SerializeField]
    private Button _restartDemoButton;

    [SerializeField]
    private Button _quitButton;
    
    private bool HasControl { get; set; }

    private void Start()
    {
        if (_controlsButton != null)
        {
            _controlsButton.onClick.AddListener(
                delegate
                {
                    if (UIManager.Instance == null)
                    {
                        return;
                    }

                    ControlsMenu controlsMenuResource = Resources.Load<ControlsMenu>(ControlsMenuResourcePath);

                    if (UIManager.Instance.CurrentCanvas != null && controlsMenuResource != null)
                    {
                        AudioClipHandler.PlayAudio("Audio/SelectButton", 0, transform.position, false, .5f);
                        ControlsMenu controlsMenu = Instantiate(controlsMenuResource, UIManager.Instance.CurrentCanvas.transform);
                    }
                }
            );
        }

        if (_restartDemoButton != null)
        {
            _restartDemoButton.onClick.AddListener(
                delegate
                {
                    if (GameManager.Instance != null)
                    {
                        if (GameManager.Instance.Player != null)
                        {
                            Destroy(GameManager.Instance.Player.gameObject);
                        }
                    }

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
                        GameManager.Instance.StartCoroutine(GameManager.Instance.LoadLevel("Corridor"));
                    }
                }
            );
        }

        if (_quitButton != null)
        {
            _quitButton.onClick.AddListener(
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
        
        SelectedButtonIndex = 1;
    }

    private void Update()
    {
        if (HasControl)
        {
            if (InputHandler.Up("Down"))
            {
                if (SelectedButtonIndex > 1)
                {
                    AudioClipHandler.PlayAudio("Audio/SelectButton", 0, transform.position, false, .5f);
                    SelectedButtonIndex--;
                }
                else
                {
                    AudioClipHandler.PlayAudio("Audio/Disabled", 0, transform.position, false, .8f);
                }
            }

            if (InputHandler.Down("Down"))
            {
                if (SelectedButtonIndex < NumberOfButtons)
                {
                    AudioClipHandler.PlayAudio("Audio/SelectButton", 0, transform.position, false, .5f);
                    SelectedButtonIndex++;
                }
                else
                {
                    AudioClipHandler.PlayAudio("Audio/Disabled", 0, transform.position, false, .8f);
                }
            }

            if (InputHandler.Submit("Down"))
            {
                if (SelectedButton != null)
                {
                    if (SelectedButtonInfo.Disabled)
                    {
                        AudioClipHandler.PlayAudio("Audio/Disabled", 0, transform.position, false, .8f);
                    } else
                    {
                        AudioClipHandler.PlayAudio("Audio/PressButton", 0, transform.position, false, .5f);
                        SelectedButton.onClick.Invoke();
                    }
                }
            }
        }
    }

    #region Override and Implementation

    public void Activate(bool active)
    {
        HasControl = active;
    }

    #endregion
}
