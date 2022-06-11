using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    /// <summary>
    /// Instantiates a projectile of the desired type, with the desired position and rotation. 
    /// If a wrong type is given in input, this method instantiates the default projectile.
    /// </summary>
    /// <param name="type">The type of the projectile you want to instantiate</param>
    /// <param name="position">The desired position</param>
    /// <param name="rotation">The desired rotation</param>
    /// <returns></returns>
    public static Projectile InstantiateProjectile(string type, Vector3 position, Quaternion rotation)
    {
        Projectile prefab = Resources.Load<Projectile>(GameFormulas.ProjectileResourcesPath + type);

        if (prefab == null)
        {
            prefab = Resources.Load<Projectile>(GameFormulas.ProjectileResourcesPath + GameFormulas.NormalProjectileName);
        }

        Projectile projectile = Instantiate(prefab, position, rotation);

        return projectile;
    }

    /// <summary>
    /// This method instantiates a projectile with position and rotation given by the transform in input.
    /// </summary>
    /// <param name="type">The type of the projectile you want to instantiate</param>
    /// <param name="spawnPoint">The desired spawn point</param>
    /// <returns></returns>
    public static Projectile InstantiateProjectile(string type, Transform spawnPoint)
    {
        return InstantiateProjectile(type, spawnPoint.position, spawnPoint.rotation);
    }

    /// <summary>
    /// Class that represents the additional effects that a projectile can have.
    /// </summary>
    [System.Serializable]
    public class AdditionalEffect
    {
        /// <summary>
        /// enum type that represents the type of the additional effect.
        /// </summary>
        [System.Serializable]
        public enum EffectType
        {
            IncreaseBlindnessLevel, 
            InflictCorrosion
        }

        /// <summary>
        /// The type of the additional effect.
        /// </summary>
        public EffectType Type;

        /// <summary>
        /// The value related to the additional effect. 
        /// May be unused for some effects.
        /// </summary>
        public float Value;
    }

    /// <summary>
    /// Stores the initial position from which the projectile has been instantiated.
    /// </summary>
    private Vector3 StartPosition { get; set; }

    /// <summary>
    /// The <c>Rigidbody2D</c> component attached to the projectile.
    /// </summary>
    private Rigidbody2D AttachedRigidbody { get; set; }

    /// <summary>
    /// Stores the power of the projectile.
    /// </summary>
    public float Power;
    
    /// <summary>
    /// Stores the attack of who/what has launched the projectile.
    /// </summary>
    public float AttackerAttack { get; set; }

    /// <summary>
    /// Stores the speed of the projectile.
    /// </summary>
    public float Speed;

    /// <summary>
    /// Stores the distance that the projectile can travel without hitting anything.
    /// </summary>
    public float DistanceToLast;

    /// <summary>
    /// Stores the additional effects of the projectile.
    /// </summary>
    public List<AdditionalEffect> AdditionalEffects;

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
        float basePower = Power;
        float attackerAttack = Mathf.Max(AttackerAttack, 1);
        float targetDefence = 1;
        
        IHealthable collidedHealthable = collision.gameObject.GetComponent<IHealthable>();
        IStatsable collidedStatsable = collision.gameObject.GetComponent<IStatsable>();
        IStatusable collidedStatusable = collision.gameObject.GetComponent<IStatusable>();

        if (collidedHealthable != null)
        {
            if (collidedStatsable != null)
            {
                targetDefence = collidedStatsable.Stats.Defence.CurrentValue;
            }

            int damage = GameFormulas.Damage(basePower, attackerAttack, targetDefence);

            collidedHealthable.Health.DecreaseHealth(damage);
        }

        foreach (AdditionalEffect effect in AdditionalEffects)
        {
            switch (effect.Type)
            {
                case AdditionalEffect.EffectType.IncreaseBlindnessLevel:

                    if (collidedStatusable != null)
                    {
                        collidedStatusable.Status.IncreaseBlindnessLevel(effect.Value);
                    }

                    break;
                case AdditionalEffect.EffectType.InflictCorrosion:

                    if (collidedStatusable != null)
                    {
                        collidedStatusable.Status.IncreaseCorrosionTime(effect.Value);
                    }
                    
                    break;
            }
        }

        StopAllCoroutines();
        Destroy(gameObject);
    }
}
