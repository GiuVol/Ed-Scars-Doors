using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InsectsPlatform : MonoBehaviour
{
    private const float TimeWithoutPlayerToResetTimer = 5;

    [SerializeField]
    private int _damage;

    [SerializeField]
    private float _interval;

    [SerializeField]
    private ParticleSystemForceField _insectsForceField;

    private PlayerController _player;

    private float _timer;

    private float _timeWithoutPlayer;

    // Update is called once per frame
    void Update()
    {
        if (_player == null)
        {
            _timeWithoutPlayer = Mathf.Clamp(_timeWithoutPlayer + Time.deltaTime, 0, TimeWithoutPlayerToResetTimer + 1);

            if (_insectsForceField != null)
            {
                _insectsForceField.gameObject.SetActive(false);
            }
        } else
        {
            _timeWithoutPlayer = 0;

            if (_insectsForceField != null)
            {
                _insectsForceField.gameObject.SetActive(true);
                _insectsForceField.transform.position = new Vector3(_player.transform.position.x,
                                                                    _insectsForceField.transform.position.y,
                                                                    _insectsForceField.transform.position.z);
            }
        }

        if (_timeWithoutPlayer >= 5)
        {
            _timer = 0;
            return;
        }

        if (_player != null)
        {
            _timer += Time.deltaTime;
            
            if (_timer >= _interval)
            {
                _timer = 0;
                _player.Health.Decrease(_damage);
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer(PlayerController.PlayerLayerName))
        {
            return;
        }

        _player = collision.gameObject.GetComponentInChildren<PlayerController>() ?? collision.gameObject.GetComponentInParent<PlayerController>();
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer != LayerMask.NameToLayer(PlayerController.PlayerLayerName))
        {
            return;
        }

        _player = null;
    }
}
