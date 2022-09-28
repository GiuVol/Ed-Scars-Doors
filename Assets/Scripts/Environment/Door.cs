using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField]
    private string _sceneToLoadName;

    [SerializeField]
    private bool _locked;

    void Start()
    {
        CustomUtilities.SetLayerRecursively(gameObject, LayerMask.NameToLayer(GameFormulas.DoorLayerName));
    }

    public void CrossDoor()
    {
        int buildIndex = SceneUtility.GetBuildIndexByScenePath(_sceneToLoadName);

        if (_locked || GameManager.Instance == null || buildIndex < 0)
        {
            return;
        }

        GameManager.Instance.StartCoroutine(GameManager.Instance.LoadScene(_sceneToLoadName));
    }
}
