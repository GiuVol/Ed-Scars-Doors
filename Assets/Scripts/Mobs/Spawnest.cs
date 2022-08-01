using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnest : GenericMob
{
    /// <summary>
    /// The max scale that the egg must reach before the flydier comes out.
    /// </summary>
    private const float EggMaxScale = 1.75f;

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
    /// Stores the local position that the egg has at the start.
    /// </summary>
    private Vector3 _eggStartLocalPosition;
    
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

    protected new void Start()
    {
        base.Start();

        _maxSpawnableFlydiers = Mathf.Max(_maxSpawnableFlydiers, 1);

        _eggStartLocalPosition = _egg.transform.localPosition;
    }

    private void FixedUpdate()
    {
        if (_player != null)
        {
            float distanceFromPlayer = Vector3.Distance(transform.position, _player.transform.position);

            if (CanEscape && distanceFromPlayer < _dangerDistance)
            {
                Vector3 escapeDirection = (transform.position - _player.transform.position).normalized;
                escapeDirection.y = 0;
                _attachedRigidbody.AddForce(escapeDirection * _mass * _speed);
            }

            if (_canAttack && distanceFromPlayer < _attackRange)
            {
                StartCoroutine(HandleAttack(_player));
            }
        }
    }

    #region Behaviour

    protected override IEnumerator Attack(PlayerController target)
    {
        Flydier flydierResource = Resources.Load<Flydier>(Flydier.PrefabPath);

        if (flydierResource == null || _egg == null || _flydiersSpawnPoint == null)
        {
            yield break;
        }

        if (NumberOfSpawnedFlydiers >= Mathf.Max(_maxSpawnableFlydiers, 1))
        {
            yield break;
        }

        float currentScaleFactor = 0;

        while (currentScaleFactor < EggMaxScale)
        {
            currentScaleFactor += Time.fixedDeltaTime * Mathf.Clamp(_eggGrowingSpeed, .01f, .5f);
            Mathf.Clamp(currentScaleFactor, 0, EggMaxScale);
            ChangeEggScale(currentScaleFactor);

            yield return null;
        }

        ChangeEggScale(0);

        Flydier newFlydier = GameObject.Instantiate(flydierResource, 
                                                    _flydiersSpawnPoint.transform.position, 
                                                    Quaternion.identity);

        newFlydier.Spawner = this;
        newFlydier.RemainsOnPattern = _flydiersRemainOnPattern;

        SpawnedFlydiers.Add(newFlydier);
    }

    /// <summary>
    /// Method to scale the egg, changing its local position accordingly.
    /// </summary>
    /// <param name="desiredScaleFactor"></param>
    private void ChangeEggScale(float desiredScaleFactor)
    {
        _egg.transform.localScale = Vector3.one * desiredScaleFactor;
    }
    
    protected override void Die()
    {
        Destroy(gameObject);
    }

    #endregion
}
