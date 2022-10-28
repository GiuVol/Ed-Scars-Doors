using UnityEngine;

public class Teleport : MonoBehaviour
{
    private const string EnterMessageResourcesPath = "UI/EnterMessage";
    private const string CantOpenDoorMessage = "UI/CantOpenDoorMessage";
    
    [SerializeField]
    private Transform _pointToTeleportTo;

    [SerializeField]
    protected bool _locked;
    
    protected bool Disabled
    {
        get
        {
            return _locked || _pointToTeleportTo == null;
        }
    }

    private PlayerController _player;

    private DynamicUIComponent _uiMessage;

    void Start()
    {
        CustomUtilities.SetLayerRecursively(gameObject, LayerMask.NameToLayer(GameFormulas.TeleportLayerName));

        Canvas currentCanvas = FindObjectOfType<Canvas>();

        if (currentCanvas != null)
        {
            DynamicUIComponent messageResource = null;

            if (Disabled)
            {
                messageResource = Resources.Load<DynamicUIComponent>(CantOpenDoorMessage);
            }
            else
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
    
    public void TeleportToPosition()
    {
        if (_player != null)
        {
            if (Disabled)
            {
                AudioClipHandler.PlayAudio("Audio/Disabled", 0, null, false, .15f);
            } else
            {
                _player.transform.position = _pointToTeleportTo.position;
            }
        }
    }

    private void EnableEnterMessage(bool enabled)
    {
        if (_uiMessage == null)
        {
            return;
        }

        _uiMessage.gameObject.SetActive(enabled);
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(PlayerController.PlayerLayerName))
        {
            _player = collision.gameObject.GetComponentInChildren<PlayerController>();

            if (_player == null)
            {
                _player = collision.gameObject.GetComponentInParent<PlayerController>();
            }

            EnableEnterMessage(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(PlayerController.PlayerLayerName))
        {
            _player = null;
            EnableEnterMessage(false);
        }
    }
}
