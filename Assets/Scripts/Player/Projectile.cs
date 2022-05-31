using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    /// <summary>
    /// Stores the power of the projectile.
    /// </summary>
    public float Power;

    /// <summary>
    /// Stores the speed of the projectile.
    /// </summary>
    public float Speed;

    /// <summary>
    /// Stores the distance that the projectile can travel without hitting anything.
    /// </summary>
    public float DistanceToLast;
    
    /// <summary>
    /// Stores the initial position from which the projectile has been instantiated.
    /// </summary>
    private Vector3 StartPosition { get; set; }

    /// <summary>
    /// The <c>Rigidbody2D</c> component attached to the projectile.
    /// </summary>
    private Rigidbody2D AttachedRigidbody { get; set; }

    void Start()
    {
        if (gameObject.GetComponent<Rigidbody2D>() == null)
        {
            gameObject.AddComponent<Rigidbody2D>();
        }

        AttachedRigidbody = gameObject.GetComponent<Rigidbody2D>();
        AttachedRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        AttachedRigidbody.gravityScale = 0;

        StartCoroutine(HandleProjectileLifetime());
    }

    void FixedUpdate()
    {
        transform.Translate(transform.right * Speed, Space.World);
    }

    /// <summary>
    /// IEnumerator that destroys the projectile if it doesn't hit anything.
    /// </summary>
    private IEnumerator HandleProjectileLifetime()
    {
        yield return null;

        StartPosition = transform.position;

        yield return new WaitUntil(() => Vector3.Distance(StartPosition, transform.position) >= DistanceToLast);

        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        StopAllCoroutines();
        Destroy(gameObject);

        IHealthable collidedHealthable = collision.gameObject.GetComponent<IHealthable>();
        IStatsable collidedStatusable = collision.gameObject.GetComponent<IStatsable>();

        if (collidedHealthable != null)
        {
            float damage = Power;

            if (collidedStatusable != null)
            {
                damage /= collidedStatusable.Stats.Defence.CurrentValue;
            }

            collidedHealthable.Health.DecreaseHealth(Mathf.FloorToInt(damage));
        }
    }
}
