using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnest : GenericMob
{
    private const float MaxSpeed = 7;
    
    /// <summary>
    /// Consts useful for the Animator's handling.
    /// </summary>
    #region Animator's consts

    private const string WalkCycleStateName = "WalkCycle";
    private const string DieStateName = "Die";
    private const string BlindedStateName = "Blinded";

    private const string SpeedParameterName = "Speed";
    private const string DieParameterName = "Die";
    private const string BlindedParameterName = "Blinded";

    private const float DieWaitPercentage = .25f;
    private const float DieScaleLerpingSpeed = 1.5f;

    #endregion

    /// <summary>
    /// The max scale that the egg must reach before the flydier comes out.
    /// </summary>
    private const float EggMaxScale = 1.75f;

    protected override Vector3 HealthBarPositionOffset => new Vector3(0, _height, 0);

    protected override Vector3 BlindnessBarPositionOffset => new Vector3(0, _height + 1, 0);

    protected override Vector3 CorrosionBarPositionOffset => new Vector3(0, _height + 2, 0);

    protected override Vector3 BlindnessEffectPositionOffset => new Vector3(0, _height / 2, 0);

    protected override Vector3 CorrosionEffectPositionOffset => new Vector3(0, _height / 2, 0);
    
    /// <summary>
    /// Stores how many flydiers spawned by this nest can be alive at the same time;
    /// after the spawnest reaches this threshold, it will stop spawning, until some flydiers die.
    /// </summary>
    [SerializeField]
    private int _maxSpawnableFlydiers;

    /// <summary>
    /// Stores the egg sprite renderer.
    /// </summary>
    [SerializeField]
    private SpriteRenderer _egg;

    /// <summary>
    /// Stores how fast the egg must grow.
    /// </summary>
    [SerializeField]
    [Range(.01f, .5f)]
    private float _eggGrowingSpeed;

    /// <summary>
    /// Specifies where does the flydiers come from.
    /// </summary>
    [SerializeField]
    private Transform _flydiersSpawnPoint;

    /// <summary>
    /// Specifies if the flydiers spawned must remain on pattern.
    /// </summary>
    [SerializeField]
    private bool _flydiersRemainOnPattern;

    /// <summary>
    /// Stores whether the spawnest should escape from the player or not.
    /// </summary>
    [SerializeField]
    private bool _shouldEscape;

    /// <summary>
    /// Specifies how far from the player the nest must be.
    /// </summary>
    [SerializeField]
    private float _dangerDistance;

    /// <summary>
    /// Stores how long the spawnest should be farther from the player than the danger distance before it stops escaping.
    /// </summary>
    private float _timeSafe;
    
    /// <summary>
    /// Returns whether the spawnest can escape from the player or not.
    /// </summary>
    private bool CanEscape
    {
        get
        {
            return _shouldEscape && _dangerDistance > 0;
        }
    }
    
    /// <summary>
    /// A set which contains all the flydiers spawned by this mob.
    /// </summary>
    private HashSet<Flydier> _spawnedFlydiers;

    /// <summary>
    /// A property to provide access to the set of spawned flydiers in a controlled way.
    /// </summary>
    public HashSet<Flydier> SpawnedFlydiers
    {
        get
        {
            if (_spawnedFlydiers == null)
            {
                _spawnedFlydiers = new HashSet<Flydier>();
            }

            return _spawnedFlydiers;
        }
    }

    /// <summary>
    /// Returns the number of the spawned flydiers.
    /// </summary>
    private int NumberOfSpawnedFlydiers
    {
        get
        {
            return SpawnedFlydiers.Count;
        }
    }

    private Coroutine _spawningFlydiersCoroutine;

    protected new void Start()
    {
        base.Start();

        AnimController = GetComponentInChildren<Animator>();

        _maxSpawnableFlydiers = Mathf.Max(_maxSpawnableFlydiers, 1);

        ChangeEggScale(0);
    }

    private void FixedUpdate()
    {
        UpdateBars();

        Vector2 localSpaceVelocity = transform.InverseTransformDirection(_attachedRigidbody.velocity);
        float normalizedSpeed = localSpaceVelocity.x / (_speed / _attachedRigidbody.drag);

        AnimController.SetFloat(SpeedParameterName, normalizedSpeed);

        AnimController.SetBool(BlindedParameterName, Status.IsBlinded);
        
        if (_isDying || Status.IsBlinded)
        {
            return;
        }

        float timeThatMustBeSafe = 2;
        _timeSafe = Mathf.Clamp(_timeSafe + Time.deltaTime, 0, timeThatMustBeSafe + 1);

        if (_player != null)
        {
            float distanceFromPlayer = Vector2.Distance(transform.position, _player.transform.position);

            #region Escaping

            if (distanceFromPlayer < _dangerDistance)
            {
                _timeSafe = 0;
            }

            if (CanEscape && _timeSafe < timeThatMustBeSafe && IsGrounded)
            {
                Vector2 escapeDirection = (transform.position - _player.transform.position).normalized;
                escapeDirection.y = 0;
                _attachedRigidbody.AddForce(escapeDirection * _mass * _speed);

                #region Rotating

                if (escapeDirection.x > 0)
                {
                    transform.rotation = Quaternion.Euler(0, 0, 0);
                }
                else if (escapeDirection.x < 0)
                {
                    transform.rotation = Quaternion.Euler(0, -180, 0);
                }

                #endregion
            }

            #endregion

            if (_canAttack && distanceFromPlayer < _attackRange)
            {
                _spawningFlydiersCoroutine = StartCoroutine(HandleAttack(_player));
            }
        }
    }

    #region Behaviour

    protected override IEnumerator Attack(PlayerController target)
    {
        #region Checking pre-conditions

        if (NumberOfSpawnedFlydiers >= Mathf.Max(_maxSpawnableFlydiers, 1))
        {
            yield break;
        }
        
        Flydier flydierResource = Resources.Load<Flydier>(Flydier.PrefabPath);

        if (flydierResource == null || _egg == null || _flydiersSpawnPoint == null)
        {
            yield break;
        }

        #endregion

        AudioClipHandler _eggGrowingClip = AudioClipHandler.PlayAudio("Audio/SpawnestEggGrowing", .75f, transform.position, true, .1f);

        if (_eggGrowingClip != null)
        {
            _eggGrowingClip.transform.parent = transform;
        }

        float currentScaleFactor = 0;

        while (currentScaleFactor < EggMaxScale)
        {
            currentScaleFactor += Time.fixedDeltaTime * Mathf.Clamp(_eggGrowingSpeed, .01f, .5f);
            ChangeEggScale(Mathf.Clamp(currentScaleFactor, 0, EggMaxScale));

            yield return null;
        }

        AudioClipHandler.PlayAudio("Audio/SpawnestEggHatching", .75f, transform.position, false, .1f);
        
        if (_eggGrowingClip != null)
        {
            _eggGrowingClip.StopClip();
        }

        ChangeEggScale(0);

        Flydier newFlydier = GameObject.Instantiate(flydierResource, 
                                                    _flydiersSpawnPoint.transform.position, 
                                                    Quaternion.identity);

        newFlydier.Spawner = this;
        newFlydier.RemainsOnPattern = _flydiersRemainOnPattern;

        SpawnedFlydiers.Add(newFlydier);

        _spawningFlydiersCoroutine = null;
    }

    /// <summary>
    /// Method to scale the egg, changing its local position accordingly.
    /// </summary>
    /// <param name="desiredScaleFactor"></param>
    private void ChangeEggScale(float desiredScaleFactor)
    {
        if (_egg == null)
        {
            return;
        }
        
        _egg.transform.localScale = Vector3.one * desiredScaleFactor;
    }

    protected override IEnumerator Die()
    {
        if (_spawningFlydiersCoroutine != null)
        {
            StopCoroutine(_spawningFlydiersCoroutine);
        }

        _attachedRigidbody.velocity = Vector3.zero;

        AnimController.SetTrigger(DieParameterName);

        AudioClipHandler.PlayAudio("Audio/SpawnestDying", .75f, transform.position, false, .9f);

        yield return new WaitUntil(() => AnimController.GetCurrentAnimatorStateInfo(0).IsName(DieStateName));

        AnimatorStateInfo info = AnimController.GetCurrentAnimatorStateInfo(0);

        float animationDuration = info.length / info.speed;

        yield return new WaitForSeconds(animationDuration * DieWaitPercentage);

        Vector3 startScale = transform.localScale;
        float lerpFactor = 0;

        while (transform.localScale != Vector3.zero)
        {
            lerpFactor = Mathf.Clamp01(lerpFactor + (Time.fixedDeltaTime * DieScaleLerpingSpeed));
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, lerpFactor);

            yield return new WaitForFixedUpdate();
        }

        Destroy(gameObject);
    }
    
    #endregion
}
