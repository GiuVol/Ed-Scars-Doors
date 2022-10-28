using UnityEngine;

public class DecadingPlatform : MonoBehaviour
{
    [SerializeField]
    private float _desiredGravityScale;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer(PlayerController.PlayerLayerName))
        {
            Rigidbody2D rigidbody = GetComponentInChildren<Rigidbody2D>();

            if (rigidbody != null)
            {
                rigidbody.gravityScale = _desiredGravityScale;
            }
        }
    }
}
