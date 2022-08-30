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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer(PlayerController.PlayerLayerName))
        {
            return;
        }

        if (_currentEvent == null)
        {
            _currentEvent = StartCoroutine(HandleEvent());
        }
    }

    private IEnumerator HandleEvent()
    {
        if (Expired)
        {
            Destroy(gameObject);
            yield break;
        }

        _numberOfContacts++;

        yield return StartCoroutine(Action());

        if (Expired)
        {
            Destroy(gameObject);
            yield break;
        }

        _currentEvent = null;
    }

    protected abstract IEnumerator Action();
}
