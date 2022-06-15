using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    /// <summary>
    /// The min scale that a Projectile can have.
    /// </summary>
    private const float MinScale = 1;

    /// <summary>
    /// The max scale that a Projectile can have.
    /// </summary>
    private const float MaxScale = 3f;

    /// <summary>
    /// Instantiates a projectile of the desired type, with the desired position and rotation. 
    /// If a wrong type is given in input, this method instantiates the default projectile.
    /// </summary>
    /// <param name="type">The type of the projectile you want to instantiate</param>
    /// <param name="chargeTime">The charge time of the projectile you want to instantiate</param>
    /// <param name="position">The desired position</param>
    /// <param name="rotation">The desired rotation</param>
    /// <returns></returns>
    public static Projectile InstantiateProjectile(string type, float chargeTime, Vector3 position, Quaternion rotation)
    {
        Projectile prefab = Resources.Load<Projectile>(GameFormulas.ProjectileResourcesPath + type);

        if (prefab == null)
        {
            prefab = Resources.Load<Projectile>(GameFormulas.ProjectileResourcesPath + GameFormulas.NormalProjectileName);
        }

        Projectile projectile = Instantiate(prefab, position, rotation);

        projectile.ChargeTime = chargeTime;

        return projectile;
    }

    /// <summary>
    /// This method instantiates a projectile with position and rotation given by the transform in input.
    /// </summary>
    /// <param name="type">The type of the projectile you want to instantiate</param>
    /// <param name="chargeTime">The charge time of the projectile you want to instantiate</param>
    /// <param name="spawnPoint">The desired spawn point</param>
    /// <returns></returns>
    public static Projectile InstantiateProjectile(string type, float chargeTime, Transform spawnPoint)
    {
        return InstantiateProjectile(type, chargeTime, spawnPoint.position, spawnPoint.rotation);
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
    /// Stores the base power of the projectile.
    /// </summary>
    public float BasePower;
    
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
    
    /// <summary>
    /// Stores the maximum charge time allowed with this projectile.
    /// </summary>
    public float MaxChargeTime;

    /// <summary>
    /// Returns wether this projectile can be charged or not.
    /// </summary>
    public bool IsChargeable
    {
        get
        {
            return (MaxChargeTime > 0);
        }
    }

    /// <summary>
    /// Stores the charge of the projectile.
    /// </summary>
    private float _chargeTime;

    /// <summary>
    /// Provides access to <c>_chargeTime</c> in a controlled manner.
    /// </summary>
    public float ChargeTime
    {
        get
        {
            return Mathf.Clamp(_chargeTime, 0, MaxChargeTime);
        }
        set
        {
            _chargeTime = Mathf.Clamp(value, 0, MaxChargeTime);
        }
    }

    /// <summary>
    /// Returns the power of the projectile, considering its charge time.
    /// </summary>
    public float Power
    {
        get
        {
            return GameFormulas.ChargedAttackPower(BasePower, ChargeTime);
        }
    }

    /// <summary>
    /// Stores the <c>GameObject</c> of the character who shot the projectile.
    /// </summary>
    public GameObject Shooter { get; set; }

    void Start()
    {
        if (gameObject.GetComponent<Rigidbody2D>() == null)
        {
            gameObject.AddComponent<Rigidbody2D>();
        }

        AttachedRigidbody = gameObject.GetComponent<Rigidbody2D>();
        AttachedRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        AttachedRigidbody.gravityScale = 0;

        Collider2D attachedCollider = gameObject.GetComponentInChildren<Collider2D>();

        if (attachedCollider != null)
        {
            attachedCollider.isTrigger = true;
        }

        if (IsChargeable)
        {
            float desiredScale = 
                GameFormulas.ChargedProjectileSize(ChargeTime, MaxChargeTime, MinScale, MaxScale);
            transform.localScale = transform.localScale * desiredScale;
        }

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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Rigidbody2D attachedRigidbody = collision.GetComponentInParent<Rigidbody2D>();

        if (attachedRigidbody != null && attachedRigidbody.gameObject == Shooter)
        {
            return;
        }

        Hit(collision.gameObject);
    }

    /// <summary>
    /// This method is called when the projectile hits something hittable.
    /// </summary>
    /// <param name="collided">The <c>GameObject</c> with which the projectile collided</param>
    private void Hit(GameObject collided)
    {
        float basePower = Power;
        float attackerAttack = Mathf.Max(AttackerAttack, 1);
        float targetDefence = 1;

        IHealthable collidedHealthable = collided.GetComponent<IHealthable>();
        IStatsable collidedStatsable = collided.GetComponent<IStatsable>();
        IStatusable collidedStatusable = collided.GetComponent<IStatusable>();

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
