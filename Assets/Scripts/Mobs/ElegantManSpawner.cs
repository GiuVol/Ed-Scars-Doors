using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class ElegantManSpawner : EventTrigger
{
    private const string ElegantManResourcePath = "Mobs/ElegantMan";

    [SerializeField]
    private Transform _spawnPoint;

    [SerializeField]
    private PatrolPointsGroup _ppGroup;

    [SerializeField]
    private bool _startTutorial;

    [SerializeField]
    private string _promptResourcePath;
    
    private void Start()
    {
        if (_ppGroup != null)
        {
            _ppGroup.gameObject.SetActive(false);
        }
    }

    protected override IEnumerator Action(PlayerController player)
    {
        if (_spawnPoint == null)
        {
            yield break;
        }

        ElegantMan elegantManResource = Resources.Load<ElegantMan>(ElegantManResourcePath);

        if (elegantManResource != null)
        {
            ElegantMan elegantMan = Instantiate(elegantManResource, _spawnPoint.position, Quaternion.identity);

            if (_ppGroup != null)
            {
                _ppGroup.gameObject.SetActive(true);
                elegantMan.PPGroup = _ppGroup;
            }
        }

        if (_startTutorial)
        {
            yield return new WaitForSeconds(.5f);

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

            Time.timeScale = 0;

            List<string> _dialogLines = new List<string>();
            _dialogLines.Add("Hope: Attento! Quell'essere sembra essere molto pericoloso!");
            _dialogLines.Add("Hope: Devi evitarlo a tutti i costi!");

            yield return StartCoroutine(prompt.PromptText(_dialogLines,
                                                          delegate {
                                                              Time.timeScale = 1;
                                                              GameManager.Instance.UI.PromptIsLoaded = false;
                                                              player.HasControl = true;
                                                          },
                                                          true));
        }
    }
}
