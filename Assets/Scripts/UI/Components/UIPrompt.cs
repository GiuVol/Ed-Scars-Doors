using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPrompt : MonoBehaviour
{
    /// <summary>
    /// A new type of delegate.
    /// </summary>
    public delegate void Operation();

    /// <summary>
    /// The background of the prompt.
    /// </summary>
    [SerializeField]
    private Image _background;

    /// <summary>
    /// The text component of the prompt.
    /// </summary>
    [SerializeField]
    private TextMeshProUGUI _textComponent;

    [SerializeField]
    private AudioClip _clipToPlay;

    /// <summary>
    /// Method that prompts several messages on the screen.
    /// </summary>
    /// <param name="promptMessages">All the strings that have to be prompted in sequence</param>
    /// <param name="operation">The optional operation that has to be executed when the prompt expires</param>
    /// <param name="needsPlayerInput">Stores whether the prompt needs the input of the player to expire</param>
    /// <param name="refreshTime">Stores the time needed for the a message to expire</param>
    /// <returns></returns>
    public IEnumerator PromptText(List<string> promptMessages, 
                                  Operation operation = null, 
                                  bool needsPlayerInput = false, 
                                  float refreshTime = 1)
    {
        if (_textComponent == null)
        {
            Destroy(gameObject);
            yield break;
        }

        foreach (string promptMessage in promptMessages)
        {
            _textComponent.text = promptMessage;

            if (needsPlayerInput)
            {
                yield return new WaitUntil(() => InputHandler.Submit("Down"));
                AudioClipHandler.PlayAudio(_clipToPlay, 0, null, false, .5f);
            } else
            {
                yield return new WaitForSecondsRealtime(refreshTime);
            }

            _textComponent.text = "";

            yield return new WaitForEndOfFrame();
        }

        if (operation != null)
        {
            operation();
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// Method that prompts just a message on the screen.
    /// </summary>
    /// <param name="promptMessage">The message to prompt</param>
    /// <param name="needsPlayerInput">Stores whether the prompt needs the input of the player to expire</param>
    /// <param name="refreshTime">Stores the time needed for the prompt to expire</param>
    /// <returns></returns>
    public IEnumerator PromptText(string promptMessage, 
                                  Operation operation = null, 
                                  bool needsPlayerInput = false, 
                                  float refreshTime = 1)
    {
        List<string> promptMessages = new List<string>();
        promptMessages.Add(promptMessage);

        yield return PromptText(promptMessages, operation, needsPlayerInput, refreshTime);
    }
}
