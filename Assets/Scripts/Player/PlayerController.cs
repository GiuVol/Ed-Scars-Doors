using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerController : MonoBehaviour, IHealthable, IStatsable, IStatusable
{
    public const int MaxNumberOfEquippableAbilities = 4;

    private const float DashTime = .5f;

    private const float MaxHiddenTime = 150;
    private const float MinHidingRefreshTime = 2;
    private const float MaxHidingRefreshTime = 5;

    private const string BlindedStateName = "Blinded";
    
    private const string BlindedParameterName = "Blinded";
    private const string SpeedParameterName = "Speed";

    public const string PlayerLayerName = "Player";
    public const string PlayerProjectileLayerName = "PlayerProjectile";

    private static Color PlayerStandardColor = Color.white;

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
    public float CurrentGravityScale
    {
        get
        {
            float gravityScale = 0;

            if (MovementController != null)
            {
                gravityScale = MovementController.GravityScale;
            }

            return gravityScale;
        }

        set
        {
            if (MovementController != null)
            {
                MovementController.GravityScale = value;
            }
        }
    }

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

    [SerializeField]
    private LayerMask _groundCheckMask;

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
    /// If true, all events related to the trigger are overriden.
    /// </summary>
    private bool _overrideTriggerEvents;

    #region Jumping

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

    #endregion

    #region Dashing

    /// <summary>
    /// Represents whether the player can dash or not.
    /// </summary>
    private bool CanDash { get; set; }

    #endregion

    #region Shooting

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

    #endregion

    #region Hiding

    /// <summary>
    /// The hiding place that is currently near to the player.
    /// </summary>
    public HidingPlace CurrentHidingPlace { get; private set; }

    /// <summary>
    /// Returns whether the player is near an hiding place.
    /// </summary>
    private bool IsNearHidingPlace
    {
        get
        {
            return CurrentHidingPlace != null;
        }
    }

    /// <summary>
    /// Stores whether the player is hidden or not.
    /// </summary>
    public bool IsHidden { get; private set; }
    
    /// <summary>
    /// Stores for how long the player has been hidden.
    /// </summary>
    private float _hiddenTime;

    /// <summary>
    /// Stores the time that the player needs to wait before he can hide again.
    /// </summary>
    private float _timeToWaitToHide;

    /// <summary>
    /// Returns whether the player can hide or not.
    /// </summary>
    public bool CanHide
    {
        get
        {
            return IsNearHidingPlace && _timeToWaitToHide <= 0 && !IsHidden;
        }
    }

    #endregion

    private Door _currentDoor;

    /// <summary>
    /// Structure that stores the abilities currently equipped by the player.
    /// </summary>
    private List<GenericAbility> EquippedAbilities { get; set; }

    #region Graphics

    /// <summary>
    /// The animator that handles the animations of the player.
    /// </summary>
    private Animator AttachedAnimator;

    /// <summary>
    /// The components that render the sprite.
    /// </summary>
    private List<SpriteRenderer> Renderers { get; set; }

    /// <summary>
    /// The current color of the sprite.
    /// </summary>
    private Color _currentColor;

    /// <summary>
    /// Specifies if the player currently has a different color.
    /// </summary>
    private bool _isChangingColor;

    /// <summary>
    /// The current alpha of the sprite.
    /// </summary>
    private float _currentAlphaValue;

    /// <summary>
    /// Stores the coroutine that is currently changing player's alpha.
    /// </summary>
    private Coroutine _alphaChangingCoroutine;

    /// <summary>
    /// Returns whether the player's alpha is currently changing.
    /// </summary>
    private bool IsChangingAlpha
    {
        get
        {
            return _alphaChangingCoroutine != null;
        }
    }

    /// <summary>
    /// Returns the prefab of the blindness effect.
    /// </summary>
    protected virtual GameObject BlindnessEffectResource
    {
        get
        {
            return Resources.Load<GameObject>("Effects/BlindnessEffect");
        }
    }

    /// <summary>
    /// Returns the position offset of the blindness effect.
    /// </summary>
    protected virtual Vector3 BlindnessEffectPositionOffset
    {
        get
        {
            return new Vector3(0, 5, 0);
        }
    }

    /// <summary>
    /// Returns the scale of the blindness effect.
    /// </summary>
    protected virtual Vector3 BlindnessEffectScale
    {
        get
        {
            return Vector3.one;
        }
    }

    /// <summary>
    /// Returns the prefab of the corrosion effect.
    /// </summary>
    protected virtual GameObject CorrosionEffectResource
    {
        get
        {
            return Resources.Load<GameObject>("Effects/CorrosionEffect");
        }
    }

    /// <summary>
    /// Returns the position offset of the corrosion effect.
    /// </summary>
    protected virtual Vector3 CorrosionEffectPositionOffset
    {
        get
        {
            return new Vector3(0, 5, 0);
        }
    }

    /// <summary>
    /// Returns the scale of the corrosion effect.
    /// </summary>
    protected virtual Vector3 CorrosionEffectScale
    {
        get
        {
            return Vector3.one;
        }
    }

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
                GenericAbility ability2 = Resources.Load<GenericAbility>("Abilities/SwarmShooter");
                GenericAbility ability3 = Resources.Load<GenericAbility>("Abilities/DoubleJumper");
                GenericAbility ability4 = Resources.Load<GenericAbility>("Abilities/x2Attack");
                GenericAbility ability5 = Resources.Load<GenericAbility>("Abilities/x2Defence");

                _obtainedAbilities.Add(ability1);
                _obtainedAbilities.Add(ability2);
                _obtainedAbilities.Add(ability3);
                _obtainedAbilities.Add(ability4);
                _obtainedAbilities.Add(ability5);
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

    #region Mobs

    /// <summary>
    /// Stores all the mobs that are hooking the player in a given moment.
    /// </summary>
    private HashSet<GenericMob> _mobsThatHookedThePlayer;

    /// <summary>
    /// Stores all the mobs that are hooking the player in a given moment.
    /// </summary>
    public HashSet<GenericMob> MobsThatHookedThePlayer
    {
        get
        {
            if (_mobsThatHookedThePlayer == null)
            {
                _mobsThatHookedThePlayer = new HashSet<GenericMob>();
            }

            return _mobsThatHookedThePlayer;
        }
    }

    [SerializeField]
    /// <summary>
    /// Stores the time that the player has passed without being hooked.
    /// </summary>
    private float _timePassedBeingUnnoticed;

    /// <summary>
    /// Stores the time that has passed since the player has been hooked.
    /// </summary>
    private float _timePassedBeingCaught;
    
    #endregion

    public bool IsInitialized { get; private set; }

    private void Start()
    {
        if (!IsInitialized)
        {
            Setup();
        }
    }

    public void Setup()
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
        MovementController.Setup(_groundCheckMask);
        CurrentGravityScale = StandardGravityScale;

        Health = gameObject.GetComponent<HealthComponent>();
        Stats = gameObject.GetComponent<StatsComponent>();
        Status = gameObject.GetComponent<StatusComponent>();

        Health.Setup(250, Die, 
                     delegate {
                         ChangeColorTemporarily(Color.green, .25f); 
                     }, 
                     delegate { 
                         ChangeColorTemporarily(Color.red, .25f); 
                     });
        Stats.Setup(100, 50, 500, 100, 50, 500);
        Status.Setup(10, .5f, 3, .5f, 10, .05f, 5, delegate { StartBlindness(); }, delegate { StartCorrosion(); });

        CanDash = true;
        CanShoot = true;

        ProjectileType = Projectile.NormalProjectileName;

        EquippedAbilities = new List<GenericAbility>(MaxNumberOfEquippableAbilities);

        AttachedAnimator = GetComponentInChildren<Animator>();

        Renderers = new List<SpriteRenderer>();

        _currentColor = PlayerStandardColor;
        _currentAlphaValue = 1;

        foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
        {
            Renderers.Add(renderer);
            renderer.color = new Color(_currentColor.r, _currentColor.g, _currentColor.b, _currentAlphaValue);
        }

        IsInitialized = true;
    }

    void Update()
    {
        if (HasControl && !Status.IsBlinded)
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

            if (_currentDoor != null && InputHandler.Interact("Down"))
            {
                _currentDoor.CrossDoor();
            }
        }

        if (IsHidden)
        {
            _hiddenTime = Mathf.Clamp(_hiddenTime + Time.deltaTime, 0, MaxHiddenTime + 1);
        }
        else
        {
            _hiddenTime = 0;
        }

        if (CurrentHidingPlace != null)
        {
            if (_hiddenTime >= Mathf.Min(MaxHiddenTime, CurrentHidingPlace.MaxHidingTime))
            {
                GetOutOfHiding();
            }
        }
    }

    void FixedUpdate()
    {
        if (HasControl && !Status.IsBlinded)
        {
            float horizontalInput = InputHandler.HorizontalInput;
            float movementSpeed = InputHandler.Run() ? CurrentRunSpeed : CurrentWalkSpeed;

            MovementController.HandleMovementWithSpeed(horizontalInput, movementSpeed);
        }

        if (AttachedAnimator != null)
        {
            if (MovementController != null && MovementController.AttachedRigidbody != null)
            {
                Vector2 localSpaceVelocity = transform.InverseTransformDirection(MovementController.AttachedRigidbody.velocity);
                float normalizedSpeed = localSpaceVelocity.x / (CurrentRunSpeed);

                AttachedAnimator.SetFloat(SpeedParameterName, normalizedSpeed);
            }

            AttachedAnimator.SetBool(BlindedParameterName, Status.IsBlinded);
        }

        _timeToWaitToJump = Mathf.Max(_timeToWaitToJump - Time.fixedDeltaTime, 0);
        _timeToWaitToHide = Mathf.Max(_timeToWaitToHide - Time.fixedDeltaTime, 0);
        
        if (MobsThatHookedThePlayer.Count <= 0)
        {
            _timePassedBeingUnnoticed = Mathf.Clamp(_timePassedBeingUnnoticed + Time.fixedDeltaTime, 0, float.MaxValue - 100);
            _timePassedBeingCaught = 0;
        } else
        {
            _timePassedBeingCaught = Mathf.Clamp(_timePassedBeingCaught + Time.fixedDeltaTime, 0, float.MaxValue - 100);
            _timePassedBeingUnnoticed = 0;
        }
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

        Vector2 jumpDirection = Vector2.up;
        MovementController.GiveImpulse(jumpDirection, CurrentJumpForce);

        AudioClipHandler.PlayAudio("Audio/Whoosh", 1, transform.position, false, .2f);

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
        AttachedAnimator.SetTrigger("StartJumping");

        #region Animation handling

        /*

        if (MovementController.IsGrounded)
        {
            yield return new WaitUntil(() => AttachedAnimator.GetCurrentAnimatorStateInfo(0).IsName("Jump"));

            AnimatorStateInfo animatorInfo = AttachedAnimator.GetCurrentAnimatorStateInfo(0);
            float animationDuration = animatorInfo.length / animatorInfo.speed;

            yield return new WaitForSeconds(animationDuration * .2f);

            Vector2 jumpDirection = Vector2.up;
            MovementController.GiveImpulse(jumpDirection, CurrentJumpForce);
        }

        */

        #endregion

        //Health.SetInvincibilityTemporarily(1);
        //Status.SetImmunityTemporarily(1);

        yield return new WaitUntil(() => !MovementController.IsGrounded);
        
        yield return new WaitUntil(() => MovementController.IsGrounded);

        AttachedAnimator.SetTrigger("Land");
        
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

        CanDash = false;
        
        MovementController.GiveImpulse(transform.right, CurrentDashForce);
        AudioClipHandler.PlayAudio("Audio/Whoosh", 1, transform.position, false, .2f);

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(PlayerLayerName), LayerMask.NameToLayer(GenericMob.MobLayerName));
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(PlayerLayerName), LayerMask.NameToLayer(GenericMob.MobProjectileLayerName));

        yield return new WaitForSeconds(DashTime);

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(PlayerLayerName), LayerMask.NameToLayer(GenericMob.MobLayerName), false);
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer(PlayerLayerName), LayerMask.NameToLayer(GenericMob.MobProjectileLayerName), false);

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

        AudioClipHandler chargingClip = null;

        if (currentProjectileAsset.IsChargeable)
        {
            while (InputHandler.Shoot() || Time.timeScale == 0)
            {
                if (Time.timeScale != 0)
                {
                    chargeTime += Time.deltaTime;
                }

                yield return null;

                if (chargingClip == null)
                {
                    chargingClip = AudioClipHandler.PlayAudio("Audio/Charging", 1, transform.position, false, .2f);
                }
            }
        }

        if (chargingClip != null)
        {
            chargingClip.StopClip();
        }

        Projectile projectile = 
            Instantiate(currentProjectileAsset, ProjectilesSpawnPoint.position, transform.rotation);
        AudioClipHandler.PlayAudio("Audio/Fireball", 1, transform.position, false, .05f);

        projectile.AttackerAttack = Stats.Attack.CurrentValue;

        projectile.ChargeTime = chargeTime;

        projectile.Layer = LayerMask.NameToLayer(PlayerProjectileLayerName);

        projectile.LayersToIgnore.Add(LayerMask.NameToLayer(PlayerLayerName));
        projectile.LayersToIgnore.Add(LayerMask.NameToLayer(PlayerProjectileLayerName));
        projectile.LayersToIgnore.Add(LayerMask.NameToLayer(GameFormulas.HidingPlaceLayerName));

        CanShoot = false;

        yield return new WaitForSeconds(CurrentShootInterval);

        CanShoot = true;
    }

    #region Hiding Methods

    public void Hide()
    {
        if (CanHide && !IsChangingAlpha)
        {
            _alphaChangingCoroutine = StartCoroutine(HideCoroutine());
        }
    }

    public void GetOutOfHiding()
    {
        if (IsHidden && !IsChangingAlpha)
        {
            _alphaChangingCoroutine = StartCoroutine(GetOutOfHidingCoroutine());
        }
    }

    private IEnumerator HideCoroutine()
    {
        if (!CanHide || IsChangingAlpha)
        {
            _alphaChangingCoroutine = null;
            yield break;
        }

        AudioClipHandler.PlayAudio("Audio/Hide", 0, transform.position);

        float currentTime = 0;
        float timeItTakesToFade = .1f;
        float fadeConst = Renderers[0].color.a;

        IsHidden = true;
        HasControl = false;
        MovementController.AttachedRigidbody.velocity = Vector2.zero;
        MovementController.AttachedRigidbody.isKinematic = true;

        _overrideTriggerEvents = true;

        foreach (Collider2D collider in GetComponentsInChildren<Collider2D>())
        {
            collider.enabled = false;
        }

        while (_currentAlphaValue > 0)
        {
            _currentAlphaValue = fadeConst - (currentTime / timeItTakesToFade);

            foreach (SpriteRenderer renderer in Renderers)
            {
                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, _currentAlphaValue);
            }

            yield return new WaitForFixedUpdate();

            currentTime += Time.fixedDeltaTime;
        }

        _alphaChangingCoroutine = null;
    }

    private IEnumerator GetOutOfHidingCoroutine()
    {
        if (!IsHidden || IsChangingAlpha)
        {
            _alphaChangingCoroutine = null;
            yield break;
        }

        AudioClipHandler.PlayAudio("Audio/Hide", 0, transform.position);
        
        float currentTime = 0;
        float timeItTakesToVisible = .1f;
        float fadeConst = Renderers[0].color.a;

        while (_currentAlphaValue < 1)
        {
            _currentAlphaValue = fadeConst + (currentTime / timeItTakesToVisible);

            foreach (SpriteRenderer renderer in Renderers)
            {
                renderer.color = new Color(renderer.color.r, renderer.color.g, renderer.color.b, _currentAlphaValue);
            }

            yield return new WaitForFixedUpdate();
            currentTime += Time.deltaTime;
        }

        IsHidden = false;
        _timeToWaitToHide = Mathf.Max((_hiddenTime / MaxHiddenTime) * MaxHidingRefreshTime, MinHidingRefreshTime);
        HasControl = true;
        MovementController.AttachedRigidbody.velocity = Vector2.zero;
        MovementController.AttachedRigidbody.isKinematic = false;

        foreach (Collider2D collider in GetComponentsInChildren<Collider2D>())
        {
            collider.enabled = true;
        }

        _overrideTriggerEvents = false;

        _alphaChangingCoroutine = null;
    }

    #endregion

    #region Corrosion

    private Coroutine _corrosionCoroutine;

    protected void StartCorrosion()
    {
        if (_corrosionCoroutine != null)
        {
            return;
        }

        _corrosionCoroutine = StartCoroutine(HandleCorrosion());
    }

    protected void StopCorrosion()
    {
        if (_corrosionCoroutine != null)
        {
            StopCoroutine(_corrosionCoroutine);
            _corrosionCoroutine = null;
        }
    }

    private IEnumerator HandleCorrosion()
    {
        GameObject corrosionEffect = null;

        if (CorrosionEffectResource != null)
        {
            corrosionEffect = Instantiate(CorrosionEffectResource, transform);
            corrosionEffect.transform.localPosition = CorrosionEffectPositionOffset;
            corrosionEffect.transform.localScale = CorrosionEffectScale;
        }

        while (Status.IsCorroded)
        {
            yield return new WaitForSeconds(Status.CorrosionDamageInterval);
            Health.DecreasePercentage(Status.CorrosionDamage, false);
        }

        yield return new WaitForSeconds(.5f);

        if (corrosionEffect != null)
        {
            Destroy(corrosionEffect.gameObject);
        }

        _corrosionCoroutine = null;
    }

    #endregion

    #region Blindness

    private Coroutine _blindnessCoroutine;

    protected void StartBlindness()
    {
        if (_blindnessCoroutine != null)
        {
            return;
        }

        _blindnessCoroutine = StartCoroutine(HandleBlindness());
    }

    protected void StopBlindness()
    {
        if (_blindnessCoroutine != null)
        {
            StopCoroutine(_blindnessCoroutine);
            _blindnessCoroutine = null;
        }
    }

    private IEnumerator HandleBlindness()
    {
        GameObject blindnessEffect = null;

        if (BlindnessEffectResource != null)
        {
            blindnessEffect = Instantiate(BlindnessEffectResource, transform);
            blindnessEffect.transform.localPosition = BlindnessEffectPositionOffset;
            blindnessEffect.transform.localScale = BlindnessEffectScale;
        }

        yield return new WaitUntil(() => !Status.IsBlinded);

        if (blindnessEffect != null)
        {
            Destroy(blindnessEffect.gameObject);
        }

        _blindnessCoroutine = null;
    }

    #endregion
    
    /// <summary>
    /// This procedure is called when the health of the player reaches 0.
    /// </summary>
    void Die()
    {
        if (UIManager.Instance.CurrentHUD != null)
        {
            UIManager.Instance.CurrentHUD.ResetBars();
            UIManager.Instance.UnloadHUD();
        }
        UIManager.Instance.LoadGameOverMenu("Game Over... You lost.");
        Destroy(gameObject);
    }

    #region Abilities Methods

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

    #endregion
    
    #region Graphics Methods

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

        Color oldColor = _currentColor;

        _currentColor = new Color(color.r, color.g, color.b, _currentAlphaValue);

        foreach (SpriteRenderer renderer in Renderers)
        {
            renderer.color = _currentColor;
        }

        yield return new WaitForSecondsRealtime(duration);

        _currentColor = new Color(oldColor.r, oldColor.g, oldColor.b, _currentAlphaValue);
        
        foreach (SpriteRenderer renderer in Renderers)
        {
            renderer.color = _currentColor;
        }

        _isChangingColor = false;
    }

    #endregion

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (_overrideTriggerEvents)
        {
            return;
        }
        
        if (col.gameObject.layer == LayerMask.NameToLayer(GameFormulas.HidingPlaceLayerName))
        {
            HidingPlace hidingPlace = col.GetComponent<HidingPlace>();

            if (hidingPlace != null)
            {
                CurrentHidingPlace = hidingPlace;
            }
        }

        if (col.gameObject.layer == LayerMask.NameToLayer(GameFormulas.ItemLayerName))
        {
            PhysicalItem physicalItem = col.gameObject.GetComponentInParent<PhysicalItem>();

            if (physicalItem != null)
            {
                Item itemData = physicalItem.ItemData;

                if (itemData != null)
                {
                    if (itemData is UsableItem)
                    {
                        AudioClipHandler.PlayAudio("Audio/CollectedItem", 0, transform.position);
                        Inventory.AddItem((UsableItem) itemData, 1);
                    }
                    else if (itemData is CollectableItem)
                    {
                        AudioClipHandler.PlayAudio("Audio/CollectedItem2", 0, transform.position);
                        Collection.AddItem((CollectableItem) itemData, 1);
                    }
                }

                Destroy(physicalItem.gameObject);
            }
        }

        if (col.gameObject.layer == LayerMask.NameToLayer(GameFormulas.DoorLayerName))
        {
            Door door = col.GetComponent<Door>();

            if (door != null)
            {
                _currentDoor = door;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (_overrideTriggerEvents)
        {
            return;
        }

        if (col.gameObject.layer == LayerMask.NameToLayer(GameFormulas.HidingPlaceLayerName))
        {
            CurrentHidingPlace = null;
        }

        if (col.gameObject.layer == LayerMask.NameToLayer(GameFormulas.DoorLayerName))
        {
            _currentDoor = null;
        }
    }
}
