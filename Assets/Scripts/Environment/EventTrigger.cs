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

    private bool Expired
    {
        get
        {
            return _expires && _numberOfContacts > _maxNumberOfContacts;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer(PlayerController.PlayerLayerName))
        {
            return;
        }

        if (Expired)
        {
            Destroy(gameObject);
            return;
        }

        _numberOfContacts++;

        Action();

        if (Expired)
        {
            Destroy(gameObject);
            return;
        }
    }

    protected abstract void Action();
}
