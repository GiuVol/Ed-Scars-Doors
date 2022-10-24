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

    [SerializeField]
    private bool _returnToMainMenu;

    protected override IEnumerator Action(PlayerController player)
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

                                                          if (_returnToMainMenu)
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
                                                                  GameManager.Instance.StartCoroutine(GameManager.Instance.LoadMainMenu());
                                                              }
                                                          }
                                                      }, 
                                                      true));
    }
}
