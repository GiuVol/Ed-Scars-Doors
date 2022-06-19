using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    private Button _playDemoButton;
    [SerializeField]
    private Button _newGameButton;
    [SerializeField]
    private Button _continueButton;

    public Button PlayDemoButton
    {
        get
        {
            return _playDemoButton;
        }
    }

    public Button NewGameButton
    {
        get
        {
            return _newGameButton;
        }
    }

    public Button ContinueButton
    {
        get
        {
            return _continueButton;
        }
    }
}
