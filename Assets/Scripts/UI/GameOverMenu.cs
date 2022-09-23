using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverMenu : MonoBehaviour
{
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
        PlayAgainButton.onClick.AddListener(
            delegate
            {
                UIManager.Instance.ClearCanvas();

                foreach (Transform canvasChildTransform in UIManager.Instance.CurrentCanvas.transform)
                {
                    Destroy(canvasChildTransform.gameObject);
                }

                GameManager.Instance.StartCoroutine(GameManager.Instance.LoadScene("DemoInsects"));
            }
        );

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
}
