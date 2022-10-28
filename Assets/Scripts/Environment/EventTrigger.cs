using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public abstract class EventTrigger : MonoBehaviour
{
    #region Serialized

    [SerializeField]
    private bool _expires;
    
    [SerializeField]
    private int _maxNumberOfContacts;

    [SerializeField]
    private bool _disabled;
    
    #endregion

    private int _numberOfContacts;

    private Coroutine _currentEvent;

    private bool Expired
    {
        get
        {
            return _expires && _numberOfContacts >= _maxNumberOfContacts;
        }
    }

    public bool Disabled
    {
        get; set;
    }

    private void Start()
    {
        Disabled = _disabled;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Disabled)
        {
            return;
        }

        if (collision.gameObject.layer != LayerMask.NameToLayer(PlayerController.PlayerLayerName))
        {
            return;
        }

        PlayerController player = collision.GetComponentInChildren<PlayerController>();

        if (player == null)
        {
            player = collision.GetComponentInParent<PlayerController>();

            if (player == null)
            {
                return;
            }
        }

        if (_currentEvent == null)
        {
            _currentEvent = StartCoroutine(HandleEvent(player));
        }
    }

    private IEnumerator HandleEvent(PlayerController player)
    {
        if (Expired)
        {
            Destroy(gameObject);
            yield break;
        }

        _numberOfContacts++;

        yield return StartCoroutine(Action(player));

        if (Expired)
        {
            Destroy(gameObject);
            yield break;
        }

        _currentEvent = null;
    }

    protected abstract IEnumerator Action(PlayerController player);
}
