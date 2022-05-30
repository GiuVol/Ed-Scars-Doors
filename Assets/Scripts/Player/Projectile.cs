using System.Collections;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float Power;
    public float Speed;
    public float DistanceToLast;

    private Vector3 StartPosition { get; set; }
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

    private IEnumerator HandleProjectileLifetime()
    {
        yield return null;

        StartPosition = transform.position;

        yield return new WaitUntil(() => Vector3.Distance(StartPosition, transform.position) >= DistanceToLast);

        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
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

            collidedHealthable.Health.DecreasetHealth(Mathf.FloorToInt(damage));

            Debug.Log(collidedHealthable.Health.CurrentHealth);
        }
    }
}
