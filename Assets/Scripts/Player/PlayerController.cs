using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour, IHealthable, IStatsable, IStatusable
{
    public const int MaxNumberOfEquippableAbilities = 4;

    public const string PlayerLayerName = "Player";
    public const string PlayerProjectileLayerName = "PlayerProjectile";

    #region Movement Parameters

    /// <summary>
    /// The standard speed that the character should reach when walking.
    /// </summary>
    public float StandardWalkSpeed;

    /// <summary>
    /// The standard speed that the character should reach when running.
    /// </summary>
    public float StandardRunSpeed;

    /// <summary>
    /// The standard force with which the character should jump.
    /// </summary>
    public float StandardJumpForce;

    /// <summary>
    /// The standard force with which the character should dash.
    /// </summary>
    public float StandardDashForce;

    /// <summary>
    /// The standard gravity scale of the attached rigidbody.
    /// </summary>
    public float StandardGravityScale;

    /// <summary>
    /// The standard time that elapses between different series of jumps.
    /// </summary>
    public float StandardJumpInterval;

    /// <summary>
    /// The standard time that elapses between different dashes.
    /// </summary>
    public float StandardDashInterval;

    /// <summary>
    /// The standard time that elapses between different shots.
    /// </summary>
    public float StandardShootInterval;

    /// <summary>
    /// The standard number of jumps allowed in the air.
    /// </summary>
    public int StandardNumberOfJumpsAllowedInAir;

    /// <summary>
    /// The transform from which the projectiles must be instantiated.
    /// </summary>
    public Transform ProjectilesSpawnPoint;

    /// <summary>
    /// The target that the healthbar must follow.
    /// </summary>
    public Transform HealthBarTarget;

    #endregion

    #region MovementValues

    /// <summary>
    /// The current speed that the character reaches when walking.
    /// </summary>
    public float CurrentWalkSpeed { get; set; }

    /// <summary>
    /// The current speed that the character reaches when running.
    /// </summary>
    public float CurrentRunSpeed { get; set; }

    /// <summary>
    /// The current force with which the character jumps.
    /// </summary>
    public float CurrentJumpForce { get; set; }

    /// <summary>
    /// The current force with which the character dashes.
    /// </summary>
    public float CurrentDashForce { get; set; }

    /// <summary>
    /// The current gravity scale of the attached rigidbody.
    /// </summary>
    public float CurrentGravityScale { get; set; }

    /// <summary>
    /// The current time that elapses between different series of jumps.
    /// </summary>
    public float CurrentJumpInterval { get; set; }

    /// <summary>
    /// The current time that elapses between different dashes.
    /// </summary>
    public float CurrentDashInterval { get; set; }

    /// <summary>
    /// The current time that elapses between different shots.
    /// </summary>
    public float CurrentShootInterval { get; set; }

    /// <summary>
    /// The current number of jumps allowed in the air.
    /// </summary>
    public int CurrentNumberOfJumpsAllowedInAir { get; set; }

    #endregion

    /// <summary>
    /// Returns whether the character can be controlled through player's inpput or not.
    /// </summary>
    public bool HasControl { get; set; }
    
    /// <summary>
    /// The <c>MovementController2D</c> that moves the player.
    /// </summary>
    private MovementController2D MovementController { get; set; }

    /// <summary>
    /// The <c>HealthComponent</c> that stores values and methods related to the health of the player.
    /// </summary>
    public HealthComponent Health { get; private set; }

    /// <summary>
    /// The <c>StatsComponent</c> that stores values and methods related to the stats of the player.
    /// </summary>
    public StatsComponent Stats { get; private set; }

    /// <summary>
    /// The <c>StatusComponent</c> that stores values and methods related to the status of the player.
    /// </summary>
    public StatusComponent Status { get; private set; }

    /// <summary>
    /// Stores the number of jumps executed in the air.
    /// </summary>
    public int CurrentNumberOfJumpsInTheAir { get; set; }

    /// <summary>
    /// Stores the coroutine that handles the jump process.
    /// </summary>
    private Coroutine _jumpHandlingTask;

    /// <summary>
    /// Stores the time to wait to jump again.
    /// </summary>
    private float _timeToWaitToJump;
    
    /// <summary>
    /// Returns whether the player can jump or not.
    /// </summary>
    private bool CanJump
    {
        get
        {
            return (MovementController.IsGrounded || CurrentNumberOfJumpsInTheAir < CurrentNumberOfJumpsAllowedInAir) && 
                _timeToWaitToJump <= 0;
        }
    }

    /// <summary>
    /// Represents whether the player can dash or not.
    /// </summary>
    private bool CanDash { get; set; }

    /// <summary>
    /// Represents whether the player can shoot or not.
    /// </summary>
    private bool CanShoot { get; set; }

    /// <summary>
    /// The name of the projectile that the player will shoot. 
    /// It must match the name of the projectile prefab.
    /// </summary>
    public string ProjectileType { get; set; }

    /// <summary>
    /// This property returns the prefab of the current projectile equipped.
    /// </summary>
    private Projectile CurrentProjectile
    {
        get
        {
            Projectile currentProjectile = Resources.Load<Projectile>(Projectile.ProjectileResourcesPath + ProjectileType);

            if (currentProjectile == null)
            {
                currentProjectile = 
                    Resources.Load<Projectile>(Projectile.ProjectileResourcesPath + Projectile.NormalProjectileName);
            }

            return currentProjectile;
        }
    }

    /// <summary>
    /// Structure that stores the abilities currently equipped by the player.
    /// </summary>
    private List<GenericAbility> EquippedAbilities { get; set; }

    #region Graphics

    /// <summary>
    /// The components that render the sprite.
    /// </summary>
    private List<SpriteRenderer> Renderers { get; set; }

    /// <summary>
    /// Specifies if the player currently has a different color.
    /// </summary>
    private bool _isChangingColor;

    #endregion

    #region Data

    private Container<UsableItem> _inventory;

    public Container<UsableItem> Inventory
    {
        get
        {
            if (_inventory == null)
            {
                _inventory = new Container<UsableItem>();

                HealingPotion testItem1 = 
                    Resources.Load<HealingPotion>("Items/Usable Items/WeakPotion");
                HealingPotion testItem2 = 
                    Resources.Load<HealingPotion>("Items/Usable Items/MediumPotion");
                HealingPotion testItem3 = 
                    Resources.Load<HealingPotion>("Items/Usable Items/StrongPotion");
                HealingPotion testItem4 = 
                    Resources.Load<HealingPotion>("Items/Usable Items/HalfLifePotion");
                HealingPotion testItem5 = 
                    Resources.Load<HealingPotion>("Items/Usable Items/FullLifePotion");

                _inventory.AddItem(testItem1, 3);
                _inventory.AddItem(testItem2, 2);
                _inventory.AddItem(testItem3, 7);
                _inventory.AddItem(testItem4, 5);
                _inventory.AddItem(testItem5, 1);
            }

            return _inventory;
        }
    }

    private List<GenericAbility> _obtainedAbilities;

    public List<GenericAbility> ObtainedAbilities
    {
        get
        {
            if (_obtainedAbilities == null)
            {
                _obtainedAbilities = new List<GenericAbility>();

                GenericAbility ability1 = Resources.Load<GenericAbility>("Abilities/DarkShooter");
                GenericAbility ability2 = Resources.Load<GenericAbility>("Abilities/DoubleJumper");
                GenericAbility ability3 = Resources.Load<GenericAbility>("Abilities/x2Attack");
                GenericAbility ability4 = Resources.Load<GenericAbility>("Abilities/x2Defence");

                _obtainedAbilities.Add(ability1);
                _obtainedAbilities.Add(ability2);
                _obtainedAbilities.Add(ability3);
                _obtainedAbilities.Add(ability4);
            }

            return _obtainedAbilities;
        }

        set
        {
            if (_obtainedAbilities == null)
            {
                _obtainedAbilities = new List<GenericAbility>();
            }

            _obtainedAbilities.Clear();

            if (value == null)
            {
                return;
            }

            foreach (GenericAbility ability in value)
            {
                _obtainedAbilities.Add(ability);
            }
        }
    }

    public Container<CollectableItem> _collection;

    public Container<CollectableItem> Collection
    {
        get
        {
            if (_collection == null)
            {
                _collection = new Container<CollectableItem>();

                CollectableItem collectable1 = 
                    Resources.Load<CollectableItem>("Items/Collectable Items/OldScarf");
                CollectableItem collectable2 =
                    Resources.Load<CollectableItem>("Items/Collectable Items/Gramophone");

                _collection.AddItem(collectable1, 1);
                _collection.AddItem(collectable2, 1);
            }

            return _collection;
        }
    }

    #endregion

    void Start()
    {
        if (gameObject.GetComponent<MovementController2D>() == null)
        {
            gameObject.AddComponent<MovementController2D>();
        }

        if (gameObject.GetComponent<HealthComponent>() == null)
        {
            gameObject.AddComponent<HealthComponent>();
        }

        if (gameObject.GetComponent<StatsComponent>() == null)
        {
            gameObject.AddComponent<StatsComponent>();
        }

        if (gameObject.GetComponent<StatusComponent>() == null)
        {
            gameObject.AddComponent<StatusComponent>();
        }

        ResetMovementValues();

        HasControl = true;
        
        MovementController = gameObject.GetComponent<MovementController2D>();

        Health = gameObject.GetComponent<HealthComponent>();
        Stats = gameObject.GetComponent<StatsComponent>();
        Status = gameObject.GetComponent<StatusComponent>();

        Health.Setup(100, Die, 
                     delegate { ChangeColorTemporarily(Color.green, .25f); }, 
                     delegate { ChangeColorTemporarily(Color.red, .25f); });
        Stats.Setup(100, 50, 500, 100, 50, 500);
        Status.Setup(100, 5, 5, 0, 1, 20, 0);

        CanDash = true;
        CanShoot = true;

        ProjectileType = Projectile.NormalProjectileName;

        EquippedAbilities = new List<GenericAbility>(MaxNumberOfEquippableAbilities);

        Renderers = new List<SpriteRenderer>();

        foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            Renderers.Add(renderer);
        }
    }

    void Update()
    {
        if (HasControl)
        {
            if (InputHandler.Jump("Down"))
            {
                Jump();
            }

            if (InputHandler.Dash("Down") && CanDash)
            {
                StartCoroutine(Dash());
            }

            if (InputHandler.Shoot("Down") && CanShoot)
            {
                StartCoroutine(Shoot());
            }
        }
    }

    void FixedUpdate()
    {
        if (HasControl)
        {
            float horizontalInput = InputHandler.HorizontalInput;
            float movementSpeed = Input.GetKey(KeyCode.B) ? CurrentRunSpeed : CurrentWalkSpeed;

            MovementController.HandleMovementWithSpeed(horizontalInput, movementSpeed);
        }

        MovementController.GravityScale = CurrentGravityScale;

        _timeToWaitToJump = Mathf.Max(_timeToWaitToJump - Time.fixedDeltaTime, 0);
    }

    /// <summary>
    /// This method initializes all the values related to movement.
    /// </summary>
    public void ResetMovementValues()
    {
        CurrentWalkSpeed = StandardWalkSpeed;
        CurrentRunSpeed = StandardRunSpeed;
        CurrentJumpForce = StandardJumpForce;
        CurrentDashForce = StandardDashForce;
        CurrentGravityScale = StandardGravityScale;
        CurrentJumpInterval = StandardJumpInterval;
        CurrentDashInterval = StandardDashInterval;
        CurrentShootInterval = StandardShootInterval;
        CurrentNumberOfJumpsAllowedInAir = StandardNumberOfJumpsAllowedInAir;
    }
    
    /// <summary>
    /// Allows the player to jump, if it is possible.
    /// </summary>
    private void Jump()
    {
        if (!CanJump)
        {
            return;
        }

        bool isGrounded = MovementController.IsGrounded;

        if (_jumpHandlingTask == null)
        {
            _jumpHandlingTask = StartCoroutine(HandleJump());
        }

        Vector3 jumpDirection = Vector3.up;
        MovementController.GiveImpulse(jumpDirection, CurrentJumpForce);

        if (!isGrounded)
        {
            CurrentNumberOfJumpsInTheAir++;
        }
    }

    /// <summary>
    /// <c>IEnumerator</c> used to handle the jump process.
    /// </summary>
    private IEnumerator HandleJump()
    {
        Health.SetInvincibilityTemporarily(1);
        Status.SetImmunityTemporarily(1);

        yield return new WaitUntil(() => !MovementController.IsGrounded);
        
        yield return new WaitUntil(() => MovementController.IsGrounded);

        CurrentNumberOfJumpsInTheAir = 0;
        _timeToWaitToJump = CurrentJumpInterval;
        _jumpHandlingTask = null;
    }

    /// <summary>
    /// Allows the player to dash, if it is possible.
    /// </summary>
    private IEnumerator Dash()
    {
        if (!CanDash)
        {
            yield break;
        }

        Health.SetInvincibilityTemporarily(1);
        Status.SetImmunityTemporarily(1);
        
        MovementController.GiveImpulse(transform.right, CurrentDashForce);

        CanDash = false;

        yield return new WaitForSeconds(CurrentDashInterval);

        CanDash = true;
    }

    /// <summary>
    /// Allows the player to shoot, if it is possible.
    /// </summary>
    private IEnumerator Shoot()
    {
        if (!CanShoot)
        {
            yield break;
        }

        float chargeTime = 0;

        Projectile currentProjectileAsset = CurrentProjectile;

        if (currentProjectileAsset.IsChargeable)
        {
            while (InputHandler.Shoot() || Time.timeScale == 0)
            {
                if (Time.timeScale != 0)
                {
                    chargeTime += Time.deltaTime;
                }

                yield return null;
            }
        }

        Projectile projectile = 
            Instantiate(currentProjectileAsset, ProjectilesSpawnPoint.position, ProjectilesSpawnPoint.rotation);

        projectile.AttackerAttack = Stats.Attack.CurrentValue;

        projectile.ChargeTime = chargeTime;

        projectile.Layer = LayerMask.NameToLayer(PlayerProjectileLayerName);

        projectile.LayersToIgnore.Add(LayerMask.NameToLayer(PlayerLayerName));
        projectile.LayersToIgnore.Add(LayerMask.NameToLayer(PlayerProjectileLayerName));

        CanShoot = false;

        yield return new WaitForSeconds(CurrentShootInterval);

        CanShoot = true;
    }

    /// <summary>
    /// This procedure is called when the health of the player reaches 0.
    /// </summary>
    void Die()
    {
        //Destroy(gameObject);
    }

    /// <summary>
    /// This method enables the ability given in input on the player, if it is possible to equip it.
    /// </summary>
    /// <param name="newAbility">The ability you want to equip</param>
    public void EquipAbility(GenericAbility newAbility)
    {
        if (EquippedAbilities.Count >= MaxNumberOfEquippableAbilities)
        {
            throw new UnequippableAbilityException(UnequippableAbilityException
                .UnequippableAbilityExceptionType.NumberExceeded);
        }

        foreach (GenericAbility equippedAbility in EquippedAbilities)
        {
            if (equippedAbility.GetType().IsEquivalentTo(newAbility.GetType()))
            {
                throw new UnequippableAbilityException(UnequippableAbilityException
                    .UnequippableAbilityExceptionType.DuplicateType);
            }
        }

        EquippedAbilities.Add(newAbility);
        newAbility.Enable(this);
    }

    /// <summary>
    /// This method disables the ability given in input, only if it is already equipped.
    /// </summary>
    /// <param name="ability">The ability you want to unequip</param>
    public void UnequipAbility(GenericAbility ability)
    {
        if (!IsEquipped(ability))
        {
            return;
        }

        EquippedAbilities.Remove(ability);
        ability.Disable(this);
    }

    /// <summary>
    /// Returns whether the ability given in input is equipped or not.
    /// </summary>
    /// <param name="ability">The ability you want to check</param>
    /// <returns></returns>
    public bool IsEquipped(GenericAbility ability)
    {
        return EquippedAbilities.Contains(ability);
    }

    /// <summary>
    /// Allows to change the color of the player for a while.
    /// </summary>
    /// <param name="color">The new color</param>
    /// <param name="duration">The duration of the change</param>
    public void ChangeColorTemporarily(Color color, float duration)
    {
        if (_isChangingColor)
        {
            return;
        }

        StartCoroutine(ChangeColorCoroutine(color, duration));
    }

    /// <summary>
    /// Handles the player's colors change.
    /// </summary>
    /// <param name="color">The new color</param>
    /// <param name="duration">The duration of the change</param>
    private IEnumerator ChangeColorCoroutine(Color color, float duration)
    {
        if (_isChangingColor)
        {
            yield break;
        }

        _isChangingColor = true;

        Color oldColor = Color.white;

        foreach (SpriteRenderer renderer in Renderers)
        {
            renderer.color = color;
        }

        yield return new WaitForSeconds(duration);

        foreach (SpriteRenderer renderer in Renderers)
        {
            renderer.color = oldColor;
        }

        _isChangingColor = false;
    }
}
