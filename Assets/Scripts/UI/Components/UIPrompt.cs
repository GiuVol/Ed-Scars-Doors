using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIPrompt : MonoBehaviour
{
    [SerializeField]
    private Image background;
    [SerializeField]
    private TextMeshProUGUI textComponent;

    /// <summary>
    /// Method that prompts texts on the screen.
    /// </summary>
    /// <param name="promptMessages">All the strings that have to be prompted in sequence</param>
    /// <param name="needsPlayerInput">Stores whether the prompt needs the input of the player to expire</param>
    /// <param name="refreshTime">Stores the time needed for the prompt to expire</param>
    /// <returns></returns>
    public IEnumerator PromptText(List<string> promptMessages, 
                                  bool needsPlayerInput = false, 
                                  float refreshTime = 1)
    {
        foreach (string promptMessage in promptMessages)
        {
            textComponent.text = promptMessage;

            if (needsPlayerInput)
            {
                yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Return));
            } else
            {
                yield return new WaitForSeconds(refreshTime);
            }

            textComponent.text = "";
        }

        Destroy(gameObject);
    }
}
