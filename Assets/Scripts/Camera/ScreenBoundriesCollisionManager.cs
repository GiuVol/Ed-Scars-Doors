using UnityEngine;

public class ScreenBoundriesCollisionManager : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.gameObject.layer != LayerMask.NameToLayer(PlayerController.PlayerLayerName))
        {
            Physics2D.IgnoreCollision(collision.collider, collision.otherCollider);
        }
    }
}
