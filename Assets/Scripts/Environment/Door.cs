using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    private const string EnterMessageResourcesPath = "UI/EnterMessage";
    private const string CantOpenDoorMessage = "UI/CantOpenDoorMessage";

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

    private DynamicUIComponent _uiMessage;

    private bool Disabled
    {
        get
        {
            return (_locked || GameManager.Instance == null || SceneUtility.GetBuildIndexByScenePath(_sceneToLoadName) < 0);
        }
    }
    
    void Start()
    {
        CustomUtilities.SetLayerRecursively(gameObject, LayerMask.NameToLayer(GameFormulas.DoorLayerName));

        Canvas currentCanvas = FindObjectOfType<Canvas>();

        if (currentCanvas != null)
        {
            DynamicUIComponent messageResource = null;

            if (Disabled)
            {
                messageResource = Resources.Load<DynamicUIComponent>(CantOpenDoorMessage);
            } else
            {
                messageResource = Resources.Load<DynamicUIComponent>(EnterMessageResourcesPath);
            }

            if (messageResource != null)
            {
                _uiMessage = Instantiate(messageResource, currentCanvas.transform);
                _uiMessage.InitializeDynamic(transform, Vector3.zero);

                _uiMessage.gameObject.SetActive(false);
            }
        }
    }

    public void CrossDoor()
    {
        int buildIndex = SceneUtility.GetBuildIndexByScenePath(_sceneToLoadName);

        if (Disabled)
        {
            AudioClipHandler.PlayAudio("Audio/Disabled", 0, null, false, .15f);
            return;
        }

        NullableVector3 playerPosition = (_useCustomPlayerPosition) ? _desiredPlayerPosition : null;
        NullableVector3 cameraPosition = (_useCustomCameraPosition) ? _desiredCameraPosition : null;

        GameManager.Instance.StartCoroutine(GameManager.Instance.LoadLevel(_sceneToLoadName, 0, playerPosition, cameraPosition));
    }

    private void EnableEnterMessage(bool enabled)
    {
        if (_uiMessage == null)
        {
            return;
        }

        _uiMessage.gameObject.SetActive(enabled);
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
