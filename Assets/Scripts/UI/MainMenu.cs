using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    /// <summary>
    /// This field stores the button of the main menu to play the demo.
    /// </summary>
    [SerializeField]
    private Button _playDemoButton;

    /// <summary>
    /// This field stores the button of the main menu to start a new game.
    /// </summary>
    [SerializeField]
    private Button _newGameButton;

    /// <summary>
    /// This field stores the button of the main menu to continue an existing game.
    /// </summary>
    [SerializeField]
    private Button _continueButton;

    /// <summary>
    /// This property provides access to <c>_playDemoButton</c> in a controlled way.
    /// </summary>
    public Button PlayDemoButton
    {
        get
        {
            return _playDemoButton;
        }
    }

    /// <summary>
    /// This property provides access to <c>_newGameButton</c> in a controlled way.
    /// </summary>
    public Button NewGameButton
    {
        get
        {
            return _newGameButton;
        }
    }

    /// <summary>
    /// This property provides access to <c>_continueButton</c> in a controlled way.
    /// </summary>
    public Button ContinueButton
    {
        get
        {
            return _continueButton;
        }
    }
}
