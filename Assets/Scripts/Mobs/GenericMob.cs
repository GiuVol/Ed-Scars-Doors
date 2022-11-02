using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class GenericMob : MonoBehaviour, IHealthable, IStatsable, IStatusable, IPatroller
{
    public const string MobLayerName = "Mob";
    public const string MobProjectileLayerName = "MobProjectile";

    /// <summary>
    /// The <c>HealthComponent</c> that stores values and methods related to the health of the mob.
    /// </summary>
    public HealthComponent Health { get; protected set; }

    /// <summary>
    /// The <c>StatsComponent</c> that stores values and methods related to the stats of the mob.
    /// </summary>
    public StatsComponent Stats { get; protected set; }

    /// <summary>
    /// The <c>StatusComponent</c> that stores values and methods related to the status of the mob.
    /// </summary>
    public StatusComponent Status { get; protected set; }

    #region Parameters

    /// <summary>
    /// The name of the mob.
    /// </summary>
    [SerializeField]
    private string _name;
    
    /// <summary>
    /// The max value of the health, used to initialize the Health Component.
    /// </summary>
    [SerializeField]
    private int _maxHealthValue;
    
    /// <summary>
    /// The standard value of the attack.
    /// </summary>
    [SerializeField]
    private int _attackStandardValue;

    /// <summary>
    /// The min value that the attack can reach.
    /// </summary>
    [SerializeField]
    private int _attackMinValue;

    /// <summary>
    /// The max value that the attack can reach.
    /// </summary>
    [SerializeField]
    private int _attackMaxValue;

    /// <summary>
    /// The standard value of the defence.
    /// </summary>
    [SerializeField]
    private int _defenceStandardValue;

    /// <summary>
    /// The min value that the defence can reach.
    /// </summary>
    [SerializeField]
    private int _defenceMinValue;

    /// <summary>
    /// The max value that the defence can reach.
    /// </summary>
    [SerializeField]
    private int _defenceMaxValue;

    /// <summary>
    /// The blindness level threshold, after which the blinded status will be set.
    /// </summary>
    [SerializeField]
    private float _maxBlindnessLevel;

    [Range(0, 1)]
    /// <summary>
    /// How much the mob resists to the attacks which inflict blindness.
    /// </summary>
    [SerializeField]
    private float _blindnessResistence;

    [SerializeField]
    private float _blindnessDuration;

    /// <summary>
    /// How fast the blindness level must decrease.
    /// </summary>
    [SerializeField]
    private float _blindnessLevelDecrementSpeed;

    /// <summary>
    /// The max time that the corrosion can last on the mob.
    /// </summary>
    [SerializeField]
    private float _maxCorrosionTime;

    [Range(0, 1)]
    /// <summary>
    /// How much the mob gets damaged from corrosion.
    /// </summary>
    [SerializeField]
    private float _corrosionDamage;

    /// <summary>
    /// How much time passes between two damages inflicted from corrosion.
    /// </summary>
    [SerializeField]
    private float _corrosionDamageInterval;
    
    /// <summary>
    /// How much time does it take to update the mob's perception of the player.
    /// </summary>
    [SerializeField]
    protected float _playerCheckInterval;
    
    /// <summary>
    /// The distance that the mob must have from a target to hook it.
    /// </summary>
    [SerializeField]
    protected float _rangeToCheck;

    /// <summary>
    /// The distance that the mob must have from a target to attack it.
    /// </summary>
    [SerializeField]
    protected float _attackRange;
    
    /// <summary>
    /// How much the mob should wait to attack again after it has already attacked.
    /// </summary>
    [SerializeField]
    protected float _attackInterval;

    /// <summary>
    /// Attribute <c>Mass</c>
    /// Stores the mass of the rigidbody component that will be attached to the mob.
    /// </summary>
    [SerializeField]
    protected float _mass;

    /// <summary>
    /// Attribute <c>Speed</c>
    /// Represents the speed at which the mob moves.
    /// </summary>
    [SerializeField]
    protected float _speed;
    
    /// <summary>
    /// Stores whether the mob can float in the air, without the gravity force applied to it.
    /// </summary>
    [SerializeField]
    protected bool _canFloat;

    /// <summary>
    /// Stores the force with which the mob should repel the player.
    /// </summary>
    [SerializeField]
    protected float _repulsiveForce;

    /// <summary>
    /// Stores the damage that the mob inflicts to the player if they collide.
    /// </summary>
    [SerializeField]
    protected int _contactDamage;

    /// <summary>
    /// The width of the mob.
    /// </summary>
    [SerializeField]
    protected float _width;

    /// <summary>
    /// The height of the mob.
    /// </summary>
    [SerializeField]
    protected float _height;
    
    #endregion

    #region Patrolling

    [Header("Patrolling")]
    /// <summary>
    /// The group of patrol points that this mob will be able to cover.
    /// </summary>
    [SerializeField]
    private PatrolPointsGroup _patrolPointsGroup;
    /// <summary>
    /// Specifies the way in which patrol points will be covered.
    /// </summary>
    [SerializeField]
    private bool _circular;

    /// <summary>
    /// Specifies the order in which the patrol points list is being checked.
    /// </summary>
    private bool _descending;
    /// <summary>
    /// Stores the current target patrol point.
    /// </summary>
    private int _currentPatrolPointIndex;

    /// <summary>
    /// Property to access to the patrol points group in a controlled way.
    /// </summary>
    public PatrolPointsGroup PPGroup
    {
        get
        {
            return _patrolPointsGroup;
        }

        set
        {
            if (value != null)
            {
                if (!value.IsBusy)
                {
                    _patrolPointsGroup = value;
                    _patrolPointsGroup.Subscriber = this;
                    return;
                }
            }

            _patrolPointsGroup = null;
        }
    }

    /// <summary>
    /// Property that returns whether the mob can patrol or not.
    /// </summary>
    protected bool CanPatrol
    {
        get
        {
            bool canPatrol = false;

            if (PPGroup != null)
            {
                canPatrol = (PPGroup.FirstPatrolPoint != null);
            }

            return canPatrol;
        }
    }
    
    /// <summary>
    /// Property that returns the patrol points of the current patrol points group.
    /// </summary>
    protected List<Transform> PatrolPoints
    {
        get
        {
            if (_patrolPointsGroup == null)
            {
                return null;
            }

            return _patrolPointsGroup.PatrolPoints;
        }
    }
    
    /// <summary>
    /// Returns the current patrol point.
    /// </summary>
    protected Transform CurrentPatrolPoint
    {
        get
        {
            if (PatrolPoints == null)
            {
                return null;
            }
            
            if (PatrolPoints.Count == 0)
            {
                return null;
            }
            
            int index = Mathf.Clamp(_currentPatrolPointIndex, 0, PatrolPoints.Count - 1);

            return PatrolPoints[index];
        }
    }

    /// <summary>
    /// Method that increases the current patrol point index in a controlled manner.
    /// </summary>
    protected void IncreasePatrolPoint()
    {
        if (PatrolPoints == null)
        {
            return;
        }
        
        if (PatrolPoints.Count == 0)
        {
            return;
        }
        
        if (_descending)
        {
            _currentPatrolPointIndex--;
        }
        else
        {
            _currentPatrolPointIndex++;
        }

        if (_currentPatrolPointIndex >= PatrolPoints.Count)
        {
            if (_circular)
            {
                _currentPatrolPointIndex = 0;
            }
            else
            {
                _descending = true;
                _currentPatrolPointIndex = Mathf.Max(PatrolPoints.Count - 2, 0);
            }
        }

        if (_currentPatrolPointIndex < 0)
        {
            if (_circular)
            {
                _descending = false;
                _currentPatrolPointIndex = 0;
            }
            else
            {
                _descending = false;
                _currentPatrolPointIndex = Mathf.Min(PatrolPoints.Count - 1, 1);
            }
        }

        _currentPatrolPointIndex = Mathf.Clamp(_currentPatrolPointIndex, 0, PatrolPoints.Count - 1);
    }

    /// <summary>
    /// This method allows to search for the nearest patrol points group.
    /// </summary>
    /// <returns>Whether the patrol points group is found or not</returns>
    protected bool SearchForPatrolPoints()
    {
        List<PatrolPointsGroup> ppGroupsList = FindObjectsOfType<PatrolPointsGroup>().ToList();

        ppGroupsList.RemoveAll(ppGroup => ppGroup.IsBusy);

        IEnumerable<PatrolPointsGroup> orderedPPGroups = ppGroupsList
            .OrderBy(ppGroup => Vector2.Distance(ppGroup.transform.position, transform.position));

        bool foundResults = ppGroupsList.Count > 0;

        if (foundResults)
        {
            PPGroup = orderedPPGroups.ToList()[0];
            CancelInvoke("SearchForPatrolPoints");
        }

        return foundResults;
    }
    
    #endregion

    /// <summary>
    /// Returns the name of the mob.
    /// </summary>
    public virtual string Name
    {
        get
        {
            return _name;
        }
    }

    /// <summary>
    /// The <c>_mobAI</c> that stores values and method related to the Artificial Intelligence of the mob.
    /// </summary>
    protected MobAI _mobAI;

    /// <summary>
    /// The rigidbody attached to the character.
    /// </summary>
    protected Rigidbody2D _attachedRigidbody;

    /// <summary>
    /// Stores whether the mob is currently attacking.
    /// </summary>
    protected bool _isAttacking;

    /// <summary>
    /// Stores the coroutine that is handling the attack.
    /// </summary>
    protected Coroutine _attackCoroutine;

    /// <summary>
    /// Stores whether the mob can attack or not.
    /// </summary>
    protected bool _canAttack;

    /// <summary>
    /// Stores whether the mob is dying.
    /// </summary>
    protected bool _isDying;

    /// <summary>
    /// A boolean property (get only) that returns whether the mob has something beneath him or not.
    /// </summary>
    public bool IsGrounded
    {
        get
        {
            LayerMask toCast = ~(1 << gameObject.layer);

            Vector3 positionOffset = Vector3.up * .1f;
            Vector3 offsettedPosition = transform.position + positionOffset;

            float range = 1.15f;

            RaycastHit2D hit =
                Physics2D.Raycast(offsettedPosition, Vector3.down, range, toCast);

            return hit;
        }
    }
    
    /// <summary>
    /// Set that contains all the layers that the mob should ignore.
    /// </summary>
    protected HashSet<int> _layersToIgnore;

    /// <summary>
    /// Property that provides access to the layers to ignore in a controlled manner.
    /// </summary>
    protected HashSet<int> LayersToIgnore
    {
        get
        {
            if (_layersToIgnore == null)
            {
                _layersToIgnore = new HashSet<int>();
            }

            return _layersToIgnore;
        }

        set
        {
            if (_layersToIgnore == null)
            {
                _layersToIgnore = new HashSet<int>();
            }

            _layersToIgnore.Clear();

            if (value == null)
            {
                return;
            }

            foreach (int layer in value)
            {
                _layersToIgnore.Add(layer);
            }
        }
    }

    #region Player

    /// <summary>
    /// Stores the player, if the mob has found it, null otherwise.
    /// </summary>
    protected PlayerController _player;

    /// <summary>
    /// Updates the mob's perception of the player.
    /// </summary>
    protected void UpdatePlayer()
    {
        _player = _mobAI.FindPlayerInRadius(transform.position, _rangeToCheck);

        if (_player != null)
        {
            if (_player.IsHidden)
            {
                _player = null;
            }
        }

        if (_player != null)
        {
            if (!_player.MobsThatHookedThePlayer.Contains(this))
            {
                _player.MobsThatHookedThePlayer.Add(this);
            }
        } else
        {
            PlayerController actualPlayer = FindObjectOfType<PlayerController>();

            if (actualPlayer != null)
            {
                if (actualPlayer.MobsThatHookedThePlayer.Contains(this))
                {
                    actualPlayer.MobsThatHookedThePlayer.Remove(this);
                }
            }
        }
    }

    #endregion

    #region Graphics

    /// <summary>
    /// The list of the sprite renderers that make up the sprite.
    /// </summary>
    protected List<SpriteRenderer> _renderers;

    /// <summary>
    /// A property that provides access in a controlled way to the list of sprite renderers.
    /// </summary>
    protected virtual List<SpriteRenderer> Renderers
    {
        get
        {
            if (_renderers == null)
            {
                _renderers = new List<SpriteRenderer>();

                foreach (SpriteRenderer renderer in GetComponentsInChildren<SpriteRenderer>())
                {
                    _renderers.Add(renderer);
                }
            }

            return _renderers;
        }
    }

    /// <summary>
    /// Stores whether this mob is changing color.
    /// </summary>
    protected bool _isChangingColor;

    /// <summary>
    /// A property which returns the eventual health bar prefab of the mob.
    /// </summary>
    protected virtual UIBar HealthBarResource
    {
        get
        {
            return Resources.Load<UIBar>("UI/MobHealthbar");
        }
    }

    /// <summary>
    /// Stores the position offset of the healthbar.
    /// </summary>
    protected virtual Vector3 HealthBarPositionOffset
    {
        get
        {
            return Vector3.zero;
        }
    }
    
    /// <summary>
    /// A property which returns the eventual health bar of the mob.
    /// </summary>
    protected UIBar HealthBar { get; set; }

    /// <summary>
    /// A property which returns the eventual blindness bar prefab of the mob.
    /// </summary>
    protected virtual UIBar BlindnessBarResource
    {
        get
        {
            return Resources.Load<UIBar>("UI/MobBlindnessbar");
        }
    }

    /// <summary>
    /// Stores the position offset of the blindnessbar.
    /// </summary>
    protected virtual Vector3 BlindnessBarPositionOffset
    {
        get
        {
            return Vector3.zero;
        }
    }

    /// <summary>
    /// A property which returns the eventual blindness bar of the mob.
    /// </summary>
    protected UIBar BlindnessBar { get; set; }

    /// <summary>
    /// A property which returns the eventual corrosion bar prefab of the mob.
    /// </summary>
    protected virtual UIBar CorrosionBarResource
    {
        get
        {
            return Resources.Load<UIBar>("UI/MobCorrosionbar");
        }
    }

    /// <summary>
    /// Stores the position offset of the corrosion bar.
    /// </summary>
    protected virtual Vector3 CorrosionBarPositionOffset
    {
        get
        {
            return Vector3.zero;
        }
    }

    /// <summary>
    /// A property which returns the eventual corrosion bar of the mob.
    /// </summary>
    protected UIBar CorrosionBar { get; set; }

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
            return Vector3.zero;
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
            return Vector3.zero;
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

    /// <summary>
    /// An auto-implemented property which stores the animator controller of the mob.
    /// </summary>
    protected Animator AnimController { get; set; }

    #endregion

    #region Dropped Items

    [SerializeField]
    [Range(0, 1)]
    private float _dropProbability;

    [SerializeField]
    private List<PhysicalItem> _droppedItems;

    public List<PhysicalItem> DroppedItems
    {
        get
        {
            if (_droppedItems == null)
            {
                _droppedItems = new List<PhysicalItem>();
            }

            return _droppedItems;
        }
    }

    #endregion

    protected void Start()
    {
        Setup();

        if (PPGroup == null)
        {
            PPGroup = _patrolPointsGroup;
        }

        InvokeRepeating("UpdatePlayer", 0, Mathf.Max(_playerCheckInterval, .1f));
    }

    #region Setup

    /// <summary>
    /// Procedure <c>Setup</c> 
    /// It is used to setup the generic mob.
    /// </summary>
    private void Setup()
    {
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

        if (gameObject.GetComponent<MobAI>() == null)
        {
            gameObject.AddComponent<MobAI>();
        }

        if (gameObject.GetComponent<Rigidbody2D>() == null)
        {
            gameObject.AddComponent<Rigidbody2D>();
        }
        
        Health = gameObject.GetComponent<HealthComponent>();
        Stats = gameObject.GetComponent<StatsComponent>();
        Status = gameObject.GetComponent<StatusComponent>();

        _mobAI = gameObject.GetComponent<MobAI>();

        _attachedRigidbody = gameObject.GetComponent<Rigidbody2D>();

        SetupHealth();
        SetupStats();
        SetupStatus();
        SetupRigidbody();

        _isAttacking = false;
        _canAttack = true;

        SetupLayers();
        SetupBars();
    }

    /// <summary>
    /// Procedure <c>SetupHealth</c>
    /// It is used to setup the Health component.
    /// </summary>
    protected virtual void SetupHealth()
    {
        if (Health != null)
        {
            Health.Setup(_maxHealthValue, DieProcedure, 
                         delegate { ChangeColorTemporarily(Color.green, .15f); }, 
                         delegate { ChangeColorTemporarily(Color.red, .15f); });
        }
    }

    /// <summary>
    /// Procedure <c>SetupStats</c>
    /// It is used to setup the Stats component.
    /// </summary>
    protected virtual void SetupStats()
    {
        if (Stats != null)
        {
            Stats.Setup(_attackStandardValue, _attackMinValue, _attackMaxValue, 
                        _defenceStandardValue, _defenceMinValue, _defenceMaxValue);
        }
    }

    /// <summary>
    /// Procedure <c>SetupStatus</c>
    /// It is used to setup the Status component.
    /// </summary>
    protected virtual void SetupStatus()
    {
        if (Status != null)
        {
            Status.Setup(_maxBlindnessLevel, _blindnessResistence, _blindnessLevelDecrementSpeed, 
                         _maxCorrosionTime, _corrosionDamage, _corrosionDamageInterval, _blindnessDuration,
                         delegate { StartBlindness(); }, delegate { StartCorrosion(); });
        }
    }

    /// <summary>
    /// It is used to setup the Rigidbody component.
    /// </summary>
    protected virtual void SetupRigidbody()
    {
        if (_attachedRigidbody != null)
        {
            _attachedRigidbody.mass = Mathf.Max(_mass, 1);
            _attachedRigidbody.drag = 3;
            _attachedRigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;

            if (_canFloat)
            {
                _attachedRigidbody.gravityScale = 0;
            } else
            {
                _attachedRigidbody.gravityScale = 7;
            }
        }
    }

    /// <summary>
    /// Procedure needed to setup the layers.
    /// </summary>
    protected virtual void SetupLayers()
    {
        CustomUtilities.SetLayerRecursively(gameObject, LayerMask.NameToLayer(MobLayerName));
        LayersToIgnore.Add(LayerMask.NameToLayer(MobLayerName));
    }

    /// <summary>
    /// Procedure needed to setup the bars of the mob.
    /// </summary>
    protected virtual void SetupBars()
    {
        Canvas canvas = FindObjectOfType<Canvas>();

        if (canvas != null)
        {
            if (HealthBarResource != null)
            {
                HealthBar = Instantiate(HealthBarResource, canvas.transform);
                HealthBar.InitializeDynamic(transform, HealthBarPositionOffset, Health.MaxHealth);
            }

            if (BlindnessBarResource != null)
            {
                BlindnessBar = Instantiate(BlindnessBarResource, canvas.transform);
                BlindnessBar.InitializeDynamic(transform, BlindnessBarPositionOffset, Status.MaxBlindnesslevel);
            }

            if (CorrosionBarResource != null)
            {
                CorrosionBar = Instantiate(CorrosionBarResource, canvas.transform);
                CorrosionBar.InitializeDynamic(transform, CorrosionBarPositionOffset, Status.MaxCorrosionTime);
            }
        }
    }

    /// <summary>
    /// Procedure needed to update the bars of the mob.
    /// </summary>
    protected void UpdateBars()
    {
        if (HealthBar != null)
        {
            HealthBar.UpdateValue(Health.CurrentHealth);
        }

        if (BlindnessBar != null)
        {
            if (Status.CurrentBlindnesslevel <= 0)
            {
                if (BlindnessBar.gameObject.activeSelf)
                {
                    BlindnessBar.gameObject.SetActive(false);
                }
            }
            else
            {
                if (!BlindnessBar.gameObject.activeSelf)
                {
                    BlindnessBar.gameObject.SetActive(true);
                }
            }

            BlindnessBar.UpdateValue(Status.CurrentBlindnesslevel);
        }

        if (CorrosionBar != null)
        {
            if (Status.CorrosionTimeLeft <= 0)
            {
                if (CorrosionBar.gameObject.activeSelf)
                {
                    CorrosionBar.gameObject.SetActive(false);
                }
            }
            else
            {
                if (!CorrosionBar.gameObject.activeSelf)
                {
                    CorrosionBar.gameObject.SetActive(true);
                }
            }

            CorrosionBar.UpdateValue(Status.CorrosionTimeLeft);
        }
    }

    #endregion

    /// <summary>
    /// The IEnumerator that handles the attack of the mob.
    /// </summary>
    /// <param name="target">the player to attack</param>
    protected virtual IEnumerator HandleAttack(PlayerController target)
    {
        if (!_canAttack)
        {
            yield break;
        }

        _isAttacking = true;
        _canAttack = false;

        _attackCoroutine = StartCoroutine(Attack(target));

        yield return _attackCoroutine;

        _isAttacking = false;

        yield return new WaitForSeconds(_attackInterval);

        _canAttack = true;

        _attackCoroutine = null;
    }

    /// <summary>
    /// The IEnumerator that specifies how the mob attacks.
    /// </summary>
    /// <param name="target">the player to attack</param>
    protected abstract IEnumerator Attack(PlayerController target);

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
    /// The procedure called when the mob dies.
    /// </summary>
    protected void DieProcedure()
    {
        if (_isDying)
        {
            return;
        }

        StartCoroutine(HandleDeath());
    }

    /// <summary>
    /// The Coroutine started when the mob dies.
    /// </summary>
    protected IEnumerator HandleDeath()
    {
        if (_isDying)
        {
            yield break;
        }
        
        _isDying = true;

        PlayerController actualPlayer = FindObjectOfType<PlayerController>();

        if (actualPlayer != null)
        {
            if (actualPlayer.MobsThatHookedThePlayer.Contains(this))
            {
                actualPlayer.MobsThatHookedThePlayer.Remove(this);
            }
        }

        if (HealthBar != null)
        {
            Destroy(HealthBar.gameObject);
        }

        if (BlindnessBar != null)
        {
            Destroy(BlindnessBar.gameObject);
        }

        if (CorrosionBar != null)
        {
            Destroy(CorrosionBar.gameObject);
        }

        if (PPGroup != null)
        {
            if (PPGroup.Subscriber != null)
            {
                PPGroup.Subscriber = null;
            }
        }
        
        yield return StartCoroutine(Die());

        DropItem();

        _isDying = false;
    }

    /// <summary>
    /// It's used to destroy the mob and perform other additional actions when he dies.
    /// </summary>
    protected abstract IEnumerator Die();

    /// <summary>
    /// Method that drops items with a certain probability.
    /// </summary>
    private void DropItem()
    {
        bool canDrop = true;

        if (_dropProbability < 1)
        {
            float randomValue = Random.Range(0f, 1f);
            canDrop = randomValue < _dropProbability;
        }

        if (canDrop)
        {
            if (DroppedItems.Count > 0)
            {
                Instantiate(DroppedItems[Random.Range(0, DroppedItems.Count - 1)], transform.position, Quaternion.identity);
            }
        }
    }

    /// <summary>
    /// Allows to change the color of the player for a while.
    /// </summary>
    /// <param name="color">The new color</param>
    /// <param name="duration">The duration of the change</param>
    public virtual void ChangeColorTemporarily(Color color, float duration)
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
    protected virtual IEnumerator ChangeColorCoroutine(Color color, float duration)
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

    protected void OnCollisionEnter2D(Collision2D collision)
    {
        if (_isDying)
        {
            if (collision.gameObject.layer != LayerMask.NameToLayer(GameFormulas.TerrainLayerName))
            {
                Physics2D.IgnoreCollision(collision.collider, collision.otherCollider);
                return;
            }

            return;
        }

        if (LayersToIgnore.Contains(collision.gameObject.layer))
        {
            Physics2D.IgnoreCollision(collision.collider, collision.otherCollider);
            return;
        }

        Rigidbody2D rigidbody = collision.rigidbody;

        if (rigidbody == null)
        {
            return;
        }

        PlayerController player = rigidbody.GetComponent<PlayerController>();

        if (player == null)
        {
            return;
        }

        Vector3 offsettedPosition = new Vector3(transform.position.x, player.transform.position.y);
        Vector2 conjunctionLine = (player.transform.position - offsettedPosition).normalized;

        rigidbody.AddForce(conjunctionLine * _repulsiveForce);
        player.Health.Decrease(_contactDamage);
    }
}
