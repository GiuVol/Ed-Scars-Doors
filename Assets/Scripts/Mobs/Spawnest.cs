using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawnest : GenericMob
{
    private const float EggMaxScale = 2.5f;

    /// <summary>
    /// Stores how many flydiers spawned by this nest can be alive at the same time;
    /// after this threshold the spawnest will stop spawn, until some flydiers die.
    /// </summary>
    [SerializeField]
    private int _maxSpawnableFlydiers;
    
    /// <summary>
    /// Specifies how far from the player the nest must be.
    /// </summary>
    [SerializeField]
    private float _dangerDistance;

    /// <summary>
    /// Stores the egg gameObject.
    /// </summary>
    [SerializeField]
    private GameObject _egg;

    /// <summary>
    /// Stores the local position that the egg has at the start.
    /// </summary>
    private Vector3 _eggStartLocalPosition;
    
    /// <summary>
    /// Specifies where does the flydiers come from.
    /// </summary>
    [SerializeField]
    private Transform _flydiersSpawnPoint;

    protected new void Start()
    {
        base.Start();
        _eggStartLocalPosition = _egg.transform.localPosition;
    }

    private void FixedUpdate()
    {
        if (_player != null)
        {
            if (Vector3.Distance(transform.position, _player.transform.position) < _dangerDistance)
            {
                Vector3 escapeDirection = (transform.position - _player.transform.position).normalized;
                _attachedRigidbody.AddForce(escapeDirection * _mass * _speed);
            }
        }
    }

    protected override IEnumerator Attack(PlayerController target)
    {
        float currentScaleFactor = 0;

        while (currentScaleFactor < EggMaxScale)
        {
            currentScaleFactor += Time.fixedDeltaTime;
            ChangeEggScale(currentScaleFactor);

            yield return null;
        }

        ChangeEggScale(0);
    }

    private void ChangeEggScale(float desiredScaleFactor)
    {
        _egg.transform.localPosition = new Vector3(_egg.transform.localPosition.x, 
                                                   _eggStartLocalPosition.y - (desiredScaleFactor / 2),
                                                   _egg.transform.localPosition.z);
        _egg.transform.localScale = Vector3.one * desiredScaleFactor;
    }
    
    protected override void Die()
    {
        Destroy(gameObject);
    }
}
