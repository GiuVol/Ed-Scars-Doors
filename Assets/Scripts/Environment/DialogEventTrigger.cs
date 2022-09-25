using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogEventTrigger : EventTrigger
{
    [SerializeField]
    private List<string> _dialogLines;

    [SerializeField]
    private string _promptResourcePath;

    [SerializeField]
    private bool _setTimeScaleToZero;

    protected override IEnumerator Action()
    {
        UIPrompt promptResource = Resources.Load<UIPrompt>(_promptResourcePath);

        if (promptResource == null || GameManager.Instance == null)
        {
            yield break;
        }

        if (GameManager.Instance.UI == null || GameManager.Instance.Player == null)
        {
            yield break;
        }

        if (GameManager.Instance.UI.CurrentCanvas == null)
        {
            yield break;
        }

        PlayerController player = GameManager.Instance.Player;
        Canvas canvas = GameManager.Instance.UI.CurrentCanvas;

        GameManager.Instance.UI.PromptIsLoaded = true;
        player.HasControl = false;

        UIPrompt prompt = Instantiate(promptResource, canvas.transform);

        if (_setTimeScaleToZero)
        {
            Time.timeScale = 0;
        }

        yield return StartCoroutine(prompt.PromptText(_dialogLines, 
                                                      delegate {
                                                          if (_setTimeScaleToZero)
                                                          {
                                                              Time.timeScale = 1;
                                                          }
                                                          GameManager.Instance.UI.PromptIsLoaded = false;
                                                          player.HasControl = true; 
                                                      }, 
                                                      true));
    }
}
