using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    private const string EnterMessageResourcesPath = "UI/EnterMessage";

    [SerializeField]
    private string _sceneToLoadName;

    [SerializeField]
    private bool _locked;

    [SerializeField]
    private bool _useCustomPlayerPosition;

    [SerializeField]
    private NullableVector3 _desiredPlayerPosition;

    [SerializeField]
    private bool _useCustomCameraPosition;

    [SerializeField]
    private NullableVector3 _desiredCameraPosition;

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

        NullableVector3 playerPosition = (_useCustomPlayerPosition) ? _desiredPlayerPosition : null;
        NullableVector3 cameraPosition = (_useCustomCameraPosition) ? _desiredCameraPosition : null;

        GameManager.Instance.StartCoroutine(GameManager.Instance.LoadScene(_sceneToLoadName, 0, playerPosition, cameraPosition));
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
