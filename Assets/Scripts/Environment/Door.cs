using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    private const string EnterMessageResourcesPath = "UI/EnterMessage";

    [SerializeField]
    private string _sceneToLoadName;

    [SerializeField]
    private bool _locked;

    private DynamicUIComponent _uiEnterMessage;
    
    void Start()
    {
        CustomUtilities.SetLayerRecursively(gameObject, LayerMask.NameToLayer(GameFormulas.DoorLayerName));

        Canvas currentCanvas = FindObjectOfType<Canvas>();

        if (currentCanvas != null)
        {
            DynamicUIComponent enterMessageResource = Resources.Load<DynamicUIComponent>(EnterMessageResourcesPath);

            if (enterMessageResource != null)
            {
                _uiEnterMessage = Instantiate(enterMessageResource, currentCanvas.transform);
                _uiEnterMessage.InitializeDynamic(transform, Vector3.zero);

                _uiEnterMessage.gameObject.SetActive(false);
            }
        }
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

    private void EnableEnterMessage(bool enabled)
    {
        if (_uiEnterMessage == null)
        {
            return;
        }

        _uiEnterMessage.gameObject.SetActive(enabled);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer(PlayerController.PlayerLayerName))
        {
            EnableEnterMessage(true);
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer(PlayerController.PlayerLayerName))
        {
            EnableEnterMessage(false);
        }
    }
}
