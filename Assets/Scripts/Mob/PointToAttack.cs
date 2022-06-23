using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointToAttack : MonoBehaviour
{

    private int _damage;

    private Rigidbody2D _collider;

    private bool _activate;

    private float _damageInterval;

    private float _timeLeftToDisable;

    private Transform _player;

    public void Activate()
    {
        _activate = true;
    }

    public void Setup(int damage, float damageInterval)
    {
        _damage = damage;
        _damageInterval = damageInterval;
        _timeLeftToDisable = _damageInterval;
    }
    // Start is called before the first frame update
    void Start()
    {
        /*if (GetComponent<Rigidbody2D>() == null)
        {
            gameObject.AddComponent<Rigidbody2D>();
        }
        _collider = GetComponent<Rigidbody2D>();*/
        _activate = false;
        _player = MobAI.GetPlayerTarget();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (_activate == true)
        {
            if (_timeLeftToDisable < 0f)
            {
                _timeLeftToDisable = _damageInterval;
                _activate = false;
            }
            else
            {
                _timeLeftToDisable -= Time.deltaTime;
                float playerx = _player.position.x;
                float mobx = transform.position.x;
                Debug.Log(playerx + "," + mobx);
                if (Vector2.Distance(_player.position, transform.position) == 0)
                {
                    transform.gameObject.GetComponent<HealthComponent>().DecreaseHealth(_damage);
                }
            }
        }
        
    }

    /*public void OnCollisionEnter2D(Collision2D collision)
    {

        if (_activate == true)
        {
            Debug.Log("collisione avvenuta");
            if (collision.gameObject.tag == "Player")
            {
                Debug.Log("ho fatto danno");
                collision.gameObject.GetComponent<HealthComponent>().DecreaseHealth(_damage);        
            }
        }
    }*/
}
